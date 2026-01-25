using MiniDropbox.Shared;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Windows.Forms;

namespace MiniDropbox.Client
{
    public partial class Form1 : Form
    {
        Socket? clientSocket;
        Thread? clientThread;
        FileSystemWatcher? watcher;

        // Mặc định thư mục
        string currentFolder = @"D:\DropboxTest";
        bool isUpdating = false;

        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!Directory.Exists(currentFolder)) Directory.CreateDirectory(currentFolder);

            // Hiển thị đường dẫn lên giao diện
            txtFolderPath.Text = currentFolder;

            // Load danh sách file lần đầu
            ReloadFileList();
        }

        // --- 1. CHỨC NĂNG CHỌN THƯ MỤC (MỚI) ---
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                if (Directory.Exists(currentFolder)) fbd.SelectedPath = currentFolder;

                if (fbd.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    // Cập nhật đường dẫn mới
                    currentFolder = fbd.SelectedPath;
                    txtFolderPath.Text = currentFolder;

                    Log("Đã đổi thư mục làm việc: " + currentFolder);

                    // Reset Watcher theo đường dẫn mới
                    if (clientSocket != null && clientSocket.Connected)
                    {
                        StartWatching();
                    }

                    // Load lại danh sách file
                    ReloadFileList();
                }
            }
        }

        // --- 2. HIỂN THỊ DANH SÁCH FILE (MỚI) ---
        void ReloadFileList()
        {
            // Đảm bảo chạy trên luồng giao diện
            if (lvFiles.InvokeRequired)
            {
                lvFiles.Invoke(new Action(ReloadFileList));
                return;
            }

            try
            {
                lvFiles.Items.Clear();
                DirectoryInfo d = new DirectoryInfo(currentFolder);
                FileInfo[] files = d.GetFiles(); // Lấy tất cả file

                foreach (FileInfo file in files)
                {
                    // Bỏ qua file tạm
                    if (file.Name.StartsWith("~$")) continue;

                    // Tạo dòng mới cho ListView
                    ListViewItem item = new ListViewItem(file.Name);
                    item.SubItems.Add(FormatSize(file.Length)); // Cột kích thước
                    item.SubItems.Add(file.LastWriteTime.ToString("dd/MM HH:mm")); // Cột ngày giờ

                    lvFiles.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Log("Lỗi đọc danh sách file: " + ex.Message);
            }
        }

        // Hàm tiện ích đổi Byte sang KB/MB cho đẹp
        string FormatSize(long bytes)
        {
            if (bytes < 1024) return bytes + " B";
            if (bytes < 1024 * 1024) return (bytes / 1024) + " KB";
            return (bytes / 1024 / 1024) + " MB";
        }

        // --- 3. KẾT NỐI SERVER ---
        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint ep = new IPEndPoint(IPAddress.Parse(txtIP.Text), Protocol.PORT);
                clientSocket.Connect(ep);

                Log("Đã kết nối Server!");
                ToggleButtons(true);
                StartWatching(); // Bắt đầu theo dõi thư mục hiện tại

                clientThread = new Thread(ReceiveData);
                clientThread.IsBackground = true;
                clientThread.Start();
            }
            catch (Exception ex)
            {
                Log("Lỗi kết nối: " + ex.Message);
                Disconnect();
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            Disconnect();
            Log("Đã ngắt kết nối.");
        }

        void Disconnect()
        {
            if (clientSocket != null) { clientSocket.Close(); clientSocket = null; }
            if (watcher != null) { watcher.Dispose(); watcher = null; }
            ToggleButtons(false);
    
        }
        void StartWatching()
        {
            if (watcher != null) watcher.Dispose();

            watcher = new FileSystemWatcher();
            watcher.Path = currentFolder; // Theo dõi folder đang chọn
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size | NotifyFilters.LastWrite;
            watcher.Filter = "*.*";

            watcher.Created += OnFileChanged;
            watcher.Changed += OnFileChanged;
            watcher.Deleted += OnFileChanged;
            watcher.Renamed += OnFileRenamed;

            watcher.EnableRaisingEvents = true;
            Log($"Đang theo dõi: {currentFolder}");
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            if (isUpdating) return;
            // Cập nhật lại danh sách hiển thị
            ReloadFileList();

            if (e.Name.StartsWith("~$")) return;
            Log($"[File {e.ChangeType}]: {e.Name}");

            // Xác định lệnh
            CommandType cmd = CommandType.FileCreate;
            if (e.ChangeType == WatcherChangeTypes.Changed) cmd = CommandType.FileUpdate;

            // Nếu là xóa
            if (e.ChangeType == WatcherChangeTypes.Deleted)
            {
                Log($"[Đã xóa]: {e.Name}");
                // Gửi lệnh xóa (không cần gửi nội dung file)
                SendSyncCommand(e.Name, null, CommandType.FileDelete);
                return;
            }

            ReloadFileList();
            if (e.Name.StartsWith("~$")) return;
            Log($"[File {e.ChangeType}]: {e.Name}");

            // Gửi file đi
            SendFile(e.FullPath, cmd);
        }

        private void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            // Nếu đang update từ server thì không gửi ngược lại
            if (isUpdating) return;
            ReloadFileList(); // Cập nhật danh sách
            Log($"[Rename]: {e.OldName} -> {e.Name}");
            // Gửi lệnh đổi tên kèm theo tên cũ (OldName)
            SendSyncCommand(e.Name, e.OldName, CommandType.FileRename);
        }

        // Hàm gửi lệnh mà không cần nội dung file (Dùng cho Xóa, Đổi tên)
        private void SendSyncCommand(string fileName, string oldFileName, CommandType cmd)
        {
            try
            {
                if (clientSocket != null && clientSocket.Connected)
                {
                    FileSyncEvent fileInfo = new FileSyncEvent
                    {
                        FileName = fileName,
                        OldFileName = oldFileName, // Quan trọng cho lệnh Rename
                        FileSize = 0
                    };

                    // Tạo gói tin với nội dung rỗng (new byte[0])
                    byte[] packet = PacketUtils.CreatePacket(cmd, fileInfo, new byte[0]);
                    clientSocket.Send(packet);
                }
            }
            catch (Exception ex) { Log("Lỗi gửi lệnh: " + ex.Message); }
        }

        private void SendFile(string fullPath, CommandType command)
        {
            try
            {
                if (!File.Exists(fullPath)) return;
                Thread.Sleep(100);

                FileInfo fi = new FileInfo(fullPath);
                FileSyncEvent fileEvent = new FileSyncEvent
                {
                    FileName = fi.Name,
                    RelativePath = fi.Name,
                    FileSize = fi.Length,
                    LastModified = fi.LastWriteTime,
                    Checksum = PacketUtils.GetMD5Checksum(fullPath)
                };

                byte[] fileContent = File.ReadAllBytes(fullPath);
                byte[] packet = PacketUtils.CreatePacket(command, fileEvent, fileContent);

                if (clientSocket != null && clientSocket.Connected)
                {
                    clientSocket.Send(packet);
                    Log($"-> Đã gửi: {fi.Name}");
                }
            }
            catch (Exception ex) { Log("Lỗi gửi: " + ex.Message); }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (clientSocket != null && clientSocket.Connected)
            {
                try
                {
                    // 1. Tạo một sự kiện giả (Dummy Event)
                    FileSyncEvent testEvent = new FileSyncEvent
                    {
                        FileName = "TestPing.txt", // Tên giả
                        FileSize = 0,              // Không có nội dung
                        RelativePath = "Test",
                        Checksum = "TEST"
                    };

                    // 2. Đóng gói theo chuẩn Protocol (Dùng CommandType.Handshake để test)
                    // Lưu ý: new byte[0] nghĩa là không có nội dung file đi kèm
                    byte[] packet = PacketUtils.CreatePacket(CommandType.Handshake, testEvent, new byte[0]);

                    // 3. Gửi đi
                    clientSocket.Send(packet);
                    Log("Me: Test Ping");
                }
                catch (Exception ex)
                {
                    Log("Lỗi gửi Ping: " + ex.Message);
                }
            }
            else
            {
                Log("Chưa kết nối Server!");
            }
        }

        // --- HELPER UI ---
        void ReceiveData()
        { /* Logic nhận giữ nguyên hoặc mở rộng sau */
            NetworkStream stream = null;
            BinaryReader reader = null;

            try
            {
                stream = new NetworkStream(clientSocket);
                reader = new BinaryReader(stream);

                while (true)
                {
                    // 1. Đọc độ dài Header (4 byte)
                    int headerLength = reader.ReadInt32();

                    // 2. Đọc Header JSON
                    byte[] headerBytes = reader.ReadBytes(headerLength);
                    string jsonHeader = Encoding.UTF8.GetString(headerBytes);
                    var packet = JsonSerializer.Deserialize<MessageHeader>(jsonHeader);

                    if (packet == null) continue;

                    // 3. Xử lý lệnh
                    switch (packet.Command)
                    {
                        case CommandType.FileCreate:
                        case CommandType.FileUpdate:
                            SaveReceivedFile(packet, reader);
                            break;

                        // --- THÊM CASE MỚI ---
                        case CommandType.FileDelete:
                            HandleDeleteCommand(packet);
                            break;

                        case CommandType.FileRename:
                            HandleRenameCommand(packet);
                            break;
                    }
                }
            }
            catch (Exception)
            {
                Log("Mất kết nối với Server.");
                Disconnect();
            }
        }

        // Xử lý khi Server yêu cầu xóa file
        private void HandleDeleteCommand(MessageHeader packet)
        {
            try
            {
                var fileInfo = JsonSerializer.Deserialize<FileSyncEvent>(packet.PayloadJson);
                string path = Path.Combine(currentFolder, fileInfo.FileName);

                if (File.Exists(path))
                {
                    // Tắt Watcher để tránh vòng lặp vô tận
                    bool wasWatching = (watcher != null && watcher.EnableRaisingEvents);
                    if (watcher != null) watcher.EnableRaisingEvents = false;

                    try
                    {
                        File.Delete(path);
                        Log($"<-- Server yêu cầu xóa: {fileInfo.FileName}");
                    }
                    finally
                    {
                        // Bật lại Watcher
                        if (watcher != null && wasWatching) watcher.EnableRaisingEvents = true;
                    }
                    ReloadFileList();
                }
            }
            catch (Exception ex) { Log("Lỗi xóa file: " + ex.Message); }
        }

        // Xử lý khi Server yêu cầu đổi tên file
        private void HandleRenameCommand(MessageHeader packet)
        {
            try
            {
                var fileInfo = JsonSerializer.Deserialize<FileSyncEvent>(packet.PayloadJson);
                string oldPath = Path.Combine(currentFolder, fileInfo.OldFileName);
                string newPath = Path.Combine(currentFolder, fileInfo.FileName);

                if (File.Exists(oldPath))
                {
                    // Tắt Watcher
                    bool wasWatching = (watcher != null && watcher.EnableRaisingEvents);
                    if (watcher != null) watcher.EnableRaisingEvents = false;

                    try
                    {
                        File.Move(oldPath, newPath);
                        Log($"<-- Server đổi tên: {fileInfo.OldFileName} -> {fileInfo.FileName}");
                    }
                    finally
                    {
                        // Bật lại Watcher
                        if (watcher != null && wasWatching) watcher.EnableRaisingEvents = true;
                    }
                    ReloadFileList();
                }
            }
            catch (Exception ex) { Log("Lỗi đổi tên file: " + ex.Message); }
        }

        private void SaveReceivedFile(MessageHeader packet, BinaryReader reader)
        {
            try
            {
                var fileInfo = JsonSerializer.Deserialize<FileSyncEvent>(packet.PayloadJson);
                if (fileInfo == null) return;

                // 1. Đọc nội dung file từ mạng vào bộ nhớ đệm (RAM) trước
                // Đọc xong mới bắt đầu ghi đĩa để giảm thời gian chiếm dụng file
                byte[] fileContent = reader.ReadBytes((int)fileInfo.FileSize);

                string savePath = Path.Combine(currentFolder, fileInfo.FileName);

                // --- KHẮC PHỤC TRIỆT ĐỂ VÒNG LẶP TẠI ĐÂY ---

                // Bước A: Tạm thời "bịt mắt" Watcher lại
                // Để nó không nhìn thấy việc chúng ta sắp ghi file
                bool wasWatching = (watcher != null && watcher.EnableRaisingEvents);
                if (watcher != null) watcher.EnableRaisingEvents = false;

                try
                {
                    // Bước B: Ghi đè file xuống ổ cứng an toàn
                    File.WriteAllBytes(savePath, fileContent);

                    // Cập nhật lại thời gian sửa đổi cho khớp với Server
                    File.SetLastWriteTime(savePath, fileInfo.LastModified);

                    Log($"<-- Đã nhận file mới: {fileInfo.FileName}");
                }
                finally
                {
                    // Bước C: Dù ghi thành công hay lỗi, BẮT BUỘC phải mở mắt lại
                    // Để tiếp tục theo dõi các file khác
                    if (watcher != null && wasWatching)
                    {
                        watcher.EnableRaisingEvents = true;
                    }
                }

                // Reload lại giao diện danh sách
                ReloadFileList();
            }
            catch (Exception ex)
            {
                Log("Lỗi lưu file: " + ex.Message);
            }
        }
}    