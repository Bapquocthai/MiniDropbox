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
            this.txtIP = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.btnSend = new System.Windows.Forms.Button();
            this.lbLog = new System.Windows.Forms.ListBox();
            this.lblIP = new System.Windows.Forms.Label();
            this.grpStatus = new System.Windows.Forms.GroupBox();
            this.lblFolder = new System.Windows.Forms.Label();
            this.txtFolderPath = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.lvFiles = new System.Windows.Forms.ListView();
            this.colName = new System.Windows.Forms.ColumnHeader();
            this.colSize = new System.Windows.Forms.ColumnHeader();
            this.colTime = new System.Windows.Forms.ColumnHeader();
            this.grpStatus.SuspendLayout();
            this.SuspendLayout();
            this.lblIP.AutoSize = true;
            this.lblIP.Location = new System.Drawing.Point(20, 30);
            this.lblIP.Text = "Server IP:";
            this.txtIP.Location = new System.Drawing.Point(90, 27);
            this.txtIP.Size = new System.Drawing.Size(120, 23);
            this.txtIP.Text = "127.0.0.1";
            this.btnConnect.Location = new System.Drawing.Point(220, 25);
            this.btnConnect.Size = new System.Drawing.Size(90, 28);
            this.btnConnect.Text = "Kết Nối";
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            this.btnDisconnect.Location = new System.Drawing.Point(320, 25);
            this.btnDisconnect.Size = new System.Drawing.Size(90, 28);
            this.btnDisconnect.Text = "Ngắt";
            this.btnDisconnect.Enabled = false;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            this.btnSend.Location = new System.Drawing.Point(420, 25);
            this.btnSend.Size = new System.Drawing.Size(90, 28);
            this.btnSend.Text = "Ping Test";
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            this.lblFolder.AutoSize = true;
            this.lblFolder.Location = new System.Drawing.Point(20, 70);
            this.lblFolder.Text = "Thư mục:";
            this.txtFolderPath.Location = new System.Drawing.Point(90, 67);
            this.txtFolderPath.Size = new System.Drawing.Size(320, 23);
            this.txtFolderPath.ReadOnly = true; 
            this.btnBrowse.Location = new System.Drawing.Point(420, 65);
            this.btnBrowse.Size = new System.Drawing.Size(90, 28);
            this.btnBrowse.Text = "Chọn...";
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            this.lvFiles.Location = new System.Drawing.Point(20, 110);
            this.lvFiles.Size = new System.Drawing.Size(490, 150);
            this.lvFiles.View = System.Windows.Forms.View.Details;
            this.lvFiles.GridLines = true;
            this.lvFiles.FullRowSelect = true;
            this.lvFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { this.colName, this.colSize, this.colTime });
            this.colName.Text = "Tên File"; this.colName.Width = 200;
            this.colSize.Text = "Kích thước"; this.colSize.Width = 100;
            this.colTime.Text = "Ngày sửa"; this.colTime.Width = 150;
            this.lbLog.Location = new System.Drawing.Point(20, 270);
            this.lbLog.Size = new System.Drawing.Size(490, 80);
            this.grpStatus.Controls.Add(this.lblIP);
            this.grpStatus.Controls.Add(this.txtIP);
            this.grpStatus.Controls.Add(this.btnConnect);
            this.grpStatus.Controls.Add(this.btnDisconnect);
            this.grpStatus.Controls.Add(this.btnSend);
            this.grpStatus.Controls.Add(this.lblFolder);
            this.grpStatus.Controls.Add(this.txtFolderPath);
            this.grpStatus.Controls.Add(this.btnBrowse);
            this.grpStatus.Controls.Add(this.lvFiles);
            this.grpStatus.Controls.Add(this.lbLog);
            this.grpStatus.Location = new System.Drawing.Point(12, 12);
            this.grpStatus.Size = new System.Drawing.Size(530, 370);
            this.grpStatus.Text = "Client Control Panel";
            this.ClientSize = new System.Drawing.Size(560, 400);
            this.Controls.Add(this.grpStatus);
            this.Name = "Form1";
            this.Text = "MiniDropbox Client v2.0";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.grpStatus.ResumeLayout(false);
            this.grpStatus.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion
    }
}