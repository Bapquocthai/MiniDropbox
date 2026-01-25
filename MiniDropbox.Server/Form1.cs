using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Windows.Forms;
using MiniDropbox.Shared;

namespace MiniDropbox.Server
{
    public partial class Form1 : Form
    {
        private Socket? _serverSocket;
        private List<ClientSocket> _clients = new List<ClientSocket>();
        private Thread? _serverThread;

        // Đường dẫn cố định của Server
        private const string SERVER_PATH = @"D:\DropboxServer";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lblServerPath.Text = "Kho dữ liệu: " + SERVER_PATH;
            // Tạo thư mục nếu chưa có
            if (!Directory.Exists(SERVER_PATH)) Directory.CreateDirectory(SERVER_PATH);
            ReloadServerFiles();
            _serverThread = new Thread(StartServer);
            _serverThread.IsBackground = true;
            _serverThread.Start();
        }
        private void ReloadServerFiles()
        {
            if (lvServerFiles.InvokeRequired)
            {
                lvServerFiles.Invoke(new Action(ReloadServerFiles));
                return;
            }

            try
            {
                lvServerFiles.Items.Clear();
                DirectoryInfo d = new DirectoryInfo(SERVER_PATH);
                FileInfo[] files = d.GetFiles();

                foreach (FileInfo file in files)
                {
                    ListViewItem item = new ListViewItem(file.Name);
                    item.SubItems.Add(FormatSize(file.Length));
                    item.SubItems.Add(file.LastWriteTime.ToString("HH:mm dd/MM"));

                    lvServerFiles.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                UpdateLog("Lỗi đọc file Server: " + ex.Message);
            }
        }

        private string FormatSize(long bytes)
        {
            if (bytes < 1024) return bytes + " B";
            if (bytes < 1024 * 1024) return (bytes / 1024) + " KB";
            return (bytes / 1024 / 1024) + " MB";
        }
        private void StartServer()
        {
            try
            {
                UpdateLog("Đang khởi động Server...");
                _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _serverSocket.Bind(new IPEndPoint(IPAddress.Any, Protocol.PORT));
                _serverSocket.Listen(10);

                UpdateLog($"Server đã sẵn sàng tại Port: {Protocol.PORT}");
                UpdateStatus("Server Running");

                while (true)
                {
                    Socket socket = _serverSocket.Accept();
                    ClientSocket client = new ClientSocket();
                    client.Socket = socket;
                    client.ClientID = socket.RemoteEndPoint?.ToString();

                    _clients.Add(client);
                    UpdateLog($"Client mới kết nối: {client.ClientID}");
                    RefreshClientList();

                    Thread clientHandler = new Thread(() => HandleClient(client));
                    clientHandler.IsBackground = true;
                    clientHandler.Start();
                }
            }
            catch (Exception ex)
            {
                UpdateLog("Lỗi Server: " + ex.Message);
            }
        }

        private void HandleClient(ClientSocket client)
        {
            NetworkStream stream = null;
            BinaryReader reader = null;

            try
            {
                stream = new NetworkStream(client.Socket);
                reader = new BinaryReader(stream);
                UpdateLog($"Bắt đầu đồng bộ dữ liệu cũ cho {client.ClientID}...");
                SyncExistingFilesToNewClient(client);
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
                            HandleFileReceive(packet, reader, SERVER_PATH, client);
                            break;
                        case CommandType.FileDelete:
                            HandleFileDelete(packet, SERVER_PATH, client);
                            break;

                        case CommandType.FileRename:
                            HandleFileRename(packet, SERVER_PATH, client);
                            break;
                        case CommandType.Handshake:
                            UpdateLog($"[{client.ClientID}] gửi PING Handshake.");
                            break;
                    }
                }
            }
            catch (EndOfStreamException)
            {
                UpdateLog($"Client {client.ClientID} ngắt kết nối.");
            }
            catch (Exception ex)
            {
                UpdateLog($"Lỗi kết nối {client.ClientID}: {ex.Message}");
            }
            finally
            {
                if (client.Socket != null) client.Socket.Close();
                _clients.Remove(client);
                RefreshClientList();
            }
        }

        private void SyncExistingFilesToNewClient(ClientSocket client)
        {
            try
            {
                string[] allFiles = Directory.GetFiles(SERVER_PATH);

                if (allFiles.Length == 0) return;

                foreach (string filePath in allFiles)
                {
                    FileInfo fi = new FileInfo(filePath);
                    FileSyncEvent fileInfo = new FileSyncEvent
                    {
                        FileName = fi.Name,
                        FileSize = fi.Length,
                        LastModified = fi.LastWriteTime,
                        RelativePath = fi.Name,
                        Checksum = PacketUtils.GetMD5Checksum(filePath)
                    };
                    byte[] fileContent = File.ReadAllBytes(filePath);
                    byte[] packet = PacketUtils.CreatePacket(CommandType.FileCreate, fileInfo, fileContent);
                    if (client.Socket != null && client.Socket.Connected)
                    {
                        client.Socket.Send(packet);
                        Thread.Sleep(50);
                    }
                }
                UpdateLog($"-> Đã đồng bộ {allFiles.Length} file cũ cho {client.ClientID}");
            }
            catch (Exception ex)
            {
                UpdateLog($"Lỗi đồng bộ ban đầu: {ex.Message}");
            }
        }

        private void HandleFileReceive(MessageHeader packet, BinaryReader reader, string saveFolder, ClientSocket sender)
        {
            try
            {
                var fileInfo = JsonSerializer.Deserialize<FileSyncEvent>(packet.PayloadJson);
                if (fileInfo == null) return;
                // Đọc file
                byte[] fileData = reader.ReadBytes((int)fileInfo.FileSize);
                string savePath = Path.Combine(saveFolder, fileInfo.FileName);
                // Xử lý xung đột
                if (packet.Command == CommandType.FileCreate && File.Exists(savePath))
                {
                    // Đổi tên file mới thành "TenFile (Conflict).txt"
                    string fileNameNoExt = Path.GetFileNameWithoutExtension(fileInfo.FileName);
                    string ext = Path.GetExtension(fileInfo.FileName);
                    string newFileName = $"{fileNameNoExt} (Conflict){ext}";
                    fileInfo.FileName = newFileName;
                    savePath = Path.Combine(saveFolder, newFileName);
                    // Gửi cảnh báo Conflict ngược lại cho người gửi 
                    SendConflictNotification(sender, fileInfo.FileName);

                    UpdateLog($"[XUNG ĐỘT] Đã đổi tên file từ {sender.ClientID} thành: {newFileName}");
                }
                // Lưu file
                File.WriteAllBytes(savePath, fileData);
                UpdateLog($"[NHẬN TỪ {sender.ClientID}] Đã lưu: {fileInfo.FileName} ({FormatSize(fileInfo.FileSize)})");
                ReloadServerFiles();
                BroadcastFile(savePath, fileInfo, sender);
            }
            catch (Exception ex)
            {
                UpdateLog($"Lỗi lưu file: {ex.Message}");
            }
        }

        private void SendConflictNotification(ClientSocket client, string newName)
        {
            try
            {
                FileSyncEvent info = new FileSyncEvent { FileName = newName };
                byte[] packet = PacketUtils.CreatePacket(CommandType.Conflict, info, new byte[0]);
                client.Socket.Send(packet);
            }
            catch { }
        }

        private void BroadcastFile(string filePath, FileSyncEvent fileInfo, ClientSocket sender)
        {
            try
            {
                byte[] fileContent = File.ReadAllBytes(filePath);
                byte[] packet = PacketUtils.CreatePacket(CommandType.FileCreate, fileInfo, fileContent);

                foreach (var client in _clients)
                {
                    if (client != sender && client.Socket != null && client.Socket.Connected)
                    {
                        try
                        {
                            client.Socket.Send(packet);
                            UpdateLog($"-> Đã chuyển tiếp tới {client.ClientID}");
                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                UpdateLog($"Lỗi Broadcast: {ex.Message}");
            }
        }
        private void UpdateLog(string msg)
        {
            if (lbLog.InvokeRequired) { lbLog.Invoke(new Action(() => UpdateLog(msg))); return; }
            lbLog.Items.Add($"{DateTime.Now:HH:mm:ss}: {msg}");
            lbLog.TopIndex = lbLog.Items.Count - 1;
        }

        private void UpdateStatus(string status)
        {
            if (lblStatus.InvokeRequired) { lblStatus.Invoke(new Action(() => UpdateStatus(status))); return; }
            lblStatus.Text = status;
        }
        private void HandleFileDelete(MessageHeader packet, string rootPath, ClientSocket sender)
        {
            try
            {
                var fileInfo = JsonSerializer.Deserialize<FileSyncEvent>(packet.PayloadJson);
                string path = Path.Combine(rootPath, fileInfo.FileName);

                if (File.Exists(path))
                {
                    File.Delete(path);
                    UpdateLog($"[XÓA] {sender.ClientID} đã xóa {fileInfo.FileName}");
                    ReloadServerFiles();
                    BroadcastCommand(CommandType.FileDelete, fileInfo, sender);
                }
            }
            catch (Exception ex) { UpdateLog("Lỗi xóa file: " + ex.Message); }
        }
        private void HandleFileRename(MessageHeader packet, string rootPath, ClientSocket sender)
        {
            try
            {
                var fileInfo = JsonSerializer.Deserialize<FileSyncEvent>(packet.PayloadJson);
                string oldPath = Path.Combine(rootPath, fileInfo.OldFileName);
                string newPath = Path.Combine(rootPath, fileInfo.FileName);
                if (File.Exists(oldPath))
                {
                    File.Move(oldPath, newPath);
                    UpdateLog($"[ĐỔI TÊN] {sender.ClientID}: {fileInfo.OldFileName} -> {fileInfo.FileName}");
                    ReloadServerFiles();
                    BroadcastCommand(CommandType.FileRename, fileInfo, sender);
                }
            }
            catch (Exception ex) { UpdateLog("Lỗi đổi tên: " + ex.Message); }
        }

        private void BroadcastCommand(CommandType cmd, FileSyncEvent fileInfo, ClientSocket sender)
        {
            try
            {
                byte[] packet = PacketUtils.CreatePacket(cmd, fileInfo, new byte[0]);

                foreach (var client in _clients)
                {
                    if (client != sender && client.Socket != null && client.Socket.Connected)
                    {
                        try
                        {
                            client.Socket.Send(packet);
                            UpdateLog($"-> Đã chuyển lệnh {cmd} tới {client.ClientID}");
                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex) { UpdateLog("Lỗi Broadcast lệnh: " + ex.Message); }
        }

        private void RefreshClientList()
        {
            if (dgvClients.InvokeRequired) { dgvClients.Invoke(new Action(RefreshClientList)); return; }
            dgvClients.DataSource = null;
            var displayList = new List<object>();
            foreach (var client in _clients)
            {
                displayList.Add(new { IP = client.ClientID, TrangThai = "Online" });
            }
            dgvClients.DataSource = displayList;
        }
    }
}