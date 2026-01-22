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
    }    