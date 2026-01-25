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
            txtFolderPath.Text = currentFolder;
            ReloadFileList();
        }

        // Chọn thu mục làm việc
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                if (Directory.Exists(currentFolder)) fbd.SelectedPath = currentFolder;

                if (fbd.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    currentFolder = fbd.SelectedPath;
                    txtFolderPath.Text = currentFolder;

                    Log("Đã đổi thư mục làm việc: " + currentFolder);
                    if (clientSocket != null && clientSocket.Connected)
                    {
                        StartWatching();
                    }
                    ReloadFileList();
                }
            }
        }


        void ReloadFileList()
        {
            if (lvFiles.InvokeRequired)
            {
                lvFiles.Invoke(new Action(ReloadFileList));
                return;
            }

            try
            {
                lvFiles.Items.Clear();
                DirectoryInfo d = new DirectoryInfo(currentFolder);
                FileInfo[] files = d.GetFiles(); 

                foreach (FileInfo file in files)
                {
                    if (file.Name.StartsWith("~$")) continue;
                    ListViewItem item = new ListViewItem(file.Name);
                    item.SubItems.Add(FormatSize(file.Length)); 
                    item.SubItems.Add(file.LastWriteTime.ToString("dd/MM HH:mm")); 

                    lvFiles.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Log("Lỗi đọc danh sách file: " + ex.Message);
            }
        }

        string FormatSize(long bytes)
        {
            if (bytes < 1024) return bytes + " B";
            if (bytes < 1024 * 1024) return (bytes / 1024) + " KB";
            return (bytes / 1024 / 1024) + " MB";
        }

        // Kết nối tới Server
        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint ep = new IPEndPoint(IPAddress.Parse(txtIP.Text), Protocol.PORT);
                clientSocket.Connect(ep);

                Log("Đã kết nối Server!");
                ToggleButtons(true);
                StartWatching();

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

        // Dùng FileSystemWatcher để theo dõi thay đổi trong thư mục
        void StartWatching()
        {
            if (watcher != null) watcher.Dispose();

            watcher = new FileSystemWatcher();
            watcher.Path = currentFolder; 
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
            ReloadFileList();

            if (e.Name.StartsWith("~$")) return;
            Log($"[File {e.ChangeType}]: {e.Name}");
            CommandType cmd = CommandType.FileCreate;
            if (e.ChangeType == WatcherChangeTypes.Changed) cmd = CommandType.FileUpdate;
            if (e.ChangeType == WatcherChangeTypes.Deleted)
            {
                Log($"[Đã xóa]: {e.Name}");

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
            if (isUpdating) return;
            ReloadFileList(); 
            Log($"[Rename]: {e.OldName} -> {e.Name}");
            SendSyncCommand(e.Name, e.OldName, CommandType.FileRename);
        }


        private void SendSyncCommand(string fileName, string oldFileName, CommandType cmd)
        {
            try
            {
                if (clientSocket != null && clientSocket.Connected)
                {
                    FileSyncEvent fileInfo = new FileSyncEvent
                    {
                        FileName = fileName,
                        OldFileName = oldFileName, 
                        FileSize = 0
                    };
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
                    FileSyncEvent testEvent = new FileSyncEvent
                    {
                        FileName = "TestPing.txt", 
                        FileSize = 0,              
                        RelativePath = "Test",
                        Checksum = "TEST"
                    };

                    // Đóng gói và gửi theo protocol
                    byte[] packet = PacketUtils.CreatePacket(CommandType.Handshake, testEvent, new byte[0]);
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

        void ReceiveData()
        {
            NetworkStream stream = null;
            BinaryReader reader = null;

            try
            {
                stream = new NetworkStream(clientSocket);
                reader = new BinaryReader(stream);

                while (true)
                {
                    int headerLength = reader.ReadInt32();
                    byte[] headerBytes = reader.ReadBytes(headerLength);
                    string jsonHeader = Encoding.UTF8.GetString(headerBytes);
                    var packet = JsonSerializer.Deserialize<MessageHeader>(jsonHeader);

                    if (packet == null) continue;
                    switch (packet.Command)
                    {
                        case CommandType.FileCreate:
                        case CommandType.FileUpdate:
                            SaveReceivedFile(packet, reader);
                            break;
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
                    bool wasWatching = (watcher != null && watcher.EnableRaisingEvents);
                    if (watcher != null) watcher.EnableRaisingEvents = false;

                    try
                    {
                        File.Delete(path);
                        Log($"<-- Server yêu cầu xóa: {fileInfo.FileName}");
                    }
                    finally
                    {
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
                    bool wasWatching = (watcher != null && watcher.EnableRaisingEvents);
                    if (watcher != null) watcher.EnableRaisingEvents = false;

                    try
                    {
                        File.Move(oldPath, newPath);
                        Log($"<-- Server đổi tên: {fileInfo.OldFileName} -> {fileInfo.FileName}");
                    }
                    finally
                    {
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
                byte[] fileContent = reader.ReadBytes((int)fileInfo.FileSize);
                string savePath = Path.Combine(currentFolder, fileInfo.FileName);
                bool wasWatching = (watcher != null && watcher.EnableRaisingEvents);
                if (watcher != null) watcher.EnableRaisingEvents = false;

                try
                {
                    File.WriteAllBytes(savePath, fileContent);
                    File.SetLastWriteTime(savePath, fileInfo.LastModified);

                    Log($"<-- Đã nhận file mới: {fileInfo.FileName}");
                }
                finally
                {
                    if (watcher != null && wasWatching)
                    {
                        watcher.EnableRaisingEvents = true;
                    }
                }
                ReloadFileList();
            }
            catch (Exception ex)
            {
                Log("Lỗi lưu file: " + ex.Message);
            }
        }

        void ToggleButtons(bool connected)
        {
            if (InvokeRequired) { Invoke(new Action(() => ToggleButtons(connected))); return; }
            btnConnect.Enabled = !connected;
            btnBrowse.Enabled = !connected;
            btnDisconnect.Enabled = connected;
            btnSend.Enabled = connected;
        }

        void Log(string msg)
        {
            if (lbLog.InvokeRequired) { lbLog.Invoke(new Action(() => Log(msg))); return; }
            lbLog.Items.Add($"{DateTime.Now:HH:mm:ss}: {msg}");
            lbLog.TopIndex = lbLog.Items.Count - 1;
        }
    }
}