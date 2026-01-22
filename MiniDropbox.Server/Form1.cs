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
            // Hiển thị đường dẫn lên giao diện
            lblServerPath.Text = "Kho dữ liệu: " + SERVER_PATH;

            // Tạo thư mục nếu chưa có
            if (!Directory.Exists(SERVER_PATH)) Directory.CreateDirectory(SERVER_PATH);

            // Load danh sách file hiện có ngay khi mở app
            ReloadServerFiles();

            // Chạy Server
            _serverThread = new Thread(StartServer);
            _serverThread.IsBackground = true;
            _serverThread.Start();
        }

        // --- hIỂN THỊ FILE SERVER ---
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
                    // Tạo dòng mới
                    ListViewItem item = new ListViewItem(file.Name);

                    // Cột Size (Đổi sang KB/MB cho đẹp)
                    item.SubItems.Add(FormatSize(file.Length));

                    // Cột Thời gian
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

        // --- CÁC HÀM XỬ LÝ MẠNG ---
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
    }