namespace MiniDropbox.Client
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.ListBox lbLog;
        private System.Windows.Forms.Label lblIP;
        private System.Windows.Forms.GroupBox grpStatus;
        private System.Windows.Forms.Label lblFolder;
        private System.Windows.Forms.TextBox txtFolderPath;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.ListView lvFiles;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colSize;
        private System.Windows.Forms.ColumnHeader colTime;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            txtIP = new TextBox();
            btnConnect = new Button();
            btnDisconnect = new Button();
            btnSend = new Button();
            lbLog = new ListBox();
            lblIP = new Label();
            grpStatus = new GroupBox();
            lblFolder = new Label();
            txtFolderPath = new TextBox();
            btnBrowse = new Button();
            lvFiles = new ListView();
            colName = new ColumnHeader();
            colSize = new ColumnHeader();
            colTime = new ColumnHeader();
            grpStatus.SuspendLayout();
            SuspendLayout();
            // 
            // txtIP
            // 
            txtIP.Location = new Point(90, 27);
            txtIP.Name = "txtIP";
            txtIP.Size = new Size(124, 23);
            txtIP.TabIndex = 1;
            txtIP.Text = "127.0.0.1";
            // 
            // btnConnect
            // 
            btnConnect.Location = new Point(404, 27);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(133, 27);
            btnConnect.TabIndex = 2;
            btnConnect.Text = "Kết Nối";
            btnConnect.Click += btnConnect_Click;
            // 
            // btnDisconnect
            // 
            btnDisconnect.Enabled = false;
            btnDisconnect.Location = new Point(569, 27);
            btnDisconnect.Name = "btnDisconnect";
            btnDisconnect.Size = new Size(139, 27);
            btnDisconnect.TabIndex = 3;
            btnDisconnect.Text = "Ngắt";
            btnDisconnect.Click += btnDisconnect_Click;
            // 
            // btnSend
            // 
            btnSend.Location = new Point(730, 27);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(126, 27);
            btnSend.TabIndex = 4;
            btnSend.Text = "Ping Test";
            btnSend.Click += btnSend_Click;
            // 
            // lbLog
            // 
            lbLog.Location = new Point(20, 299);
            lbLog.Name = "lbLog";
            lbLog.Size = new Size(836, 94);
            lbLog.TabIndex = 9;
            // 
            // lblIP
            // 
            lblIP.AutoSize = true;
            lblIP.Location = new Point(20, 30);
            lblIP.Name = "lblIP";
            lblIP.Size = new Size(55, 15);
            lblIP.TabIndex = 0;
            lblIP.Text = "Server IP:";
            // 
            // grpStatus
            // 
            grpStatus.Controls.Add(lblIP);
            grpStatus.Controls.Add(txtIP);
            grpStatus.Controls.Add(btnConnect);
            grpStatus.Controls.Add(btnDisconnect);
            grpStatus.Controls.Add(btnSend);
            grpStatus.Controls.Add(lblFolder);
            grpStatus.Controls.Add(txtFolderPath);
            grpStatus.Controls.Add(btnBrowse);
            grpStatus.Controls.Add(lvFiles);
            grpStatus.Controls.Add(lbLog);
            grpStatus.Location = new Point(12, 12);
            grpStatus.Name = "grpStatus";
            grpStatus.Size = new Size(876, 403);
            grpStatus.TabIndex = 0;
            grpStatus.TabStop = false;
            grpStatus.Text = "Client Control Panel";
            // 
            // lblFolder
            // 
            lblFolder.AutoSize = true;
            lblFolder.Location = new Point(20, 70);
            lblFolder.Name = "lblFolder";
            lblFolder.Size = new Size(58, 15);
            lblFolder.TabIndex = 5;
            lblFolder.Text = "Thư mục:";
            // 
            // txtFolderPath
            // 
            txtFolderPath.Location = new Point(90, 67);
            txtFolderPath.Name = "txtFolderPath";
            txtFolderPath.ReadOnly = true;
            txtFolderPath.Size = new Size(320, 23);
            txtFolderPath.TabIndex = 6;
            // 
            // btnBrowse
            // 
            btnBrowse.Location = new Point(420, 65);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(90, 28);
            btnBrowse.TabIndex = 7;
            btnBrowse.Text = "Chọn...";
            btnBrowse.Click += btnBrowse_Click;
            // 
            // lvFiles
            // 
            lvFiles.Columns.AddRange(new ColumnHeader[] { colName, colSize, colTime });
            lvFiles.FullRowSelect = true;
            lvFiles.GridLines = true;
            lvFiles.Location = new Point(20, 110);
            lvFiles.Name = "lvFiles";
            lvFiles.Size = new Size(836, 161);
            lvFiles.TabIndex = 8;
            lvFiles.UseCompatibleStateImageBehavior = false;
            lvFiles.View = View.Details;
            // 
            // colName
            // 
            colName.Text = "Tên File";
            colName.Width = 200;
            // 
            // colSize
            // 
            colSize.Text = "Kích thước";
            colSize.Width = 100;
            // 
            // colTime
            // 
            colTime.Text = "Ngày sửa";
            colTime.Width = 150;
            // 
            // Form1
            // 
            ClientSize = new Size(900, 427);
            Controls.Add(grpStatus);
            Name = "Form1";
            Text = "MiniDropbox Client v2.0";
            Load += Form1_Load;
            grpStatus.ResumeLayout(false);
            grpStatus.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
    }
}