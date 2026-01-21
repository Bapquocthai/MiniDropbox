namespace MiniDropbox.Client
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        // Các biến giao diện
        private System.Windows.Forms.Label lblServerIP;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.Button btnSend; // Nút Ping

        private System.Windows.Forms.Label lblFolder;
        private System.Windows.Forms.TextBox txtFolderPath;
        private System.Windows.Forms.Button btnBrowse;

        private System.Windows.Forms.ListView lvFiles;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colSize;
        private System.Windows.Forms.ColumnHeader colTime;

        private System.Windows.Forms.ListBox lbLog;
        private System.Windows.Forms.GroupBox grpMain; // GroupBox chính bao quanh

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            lblServerIP = new Label();
            txtIP = new TextBox();
            btnConnect = new Button();
            btnDisconnect = new Button();
            btnSend = new Button();
            lbLog = new ListBox();
            grpMain = new GroupBox();
            lblFolder = new Label();
            txtFolderPath = new TextBox();
            btnBrowse = new Button();
            lvFiles = new ListView();
            colName = new ColumnHeader();
            colSize = new ColumnHeader();
            colTime = new ColumnHeader();
            grpMain.SuspendLayout();
            SuspendLayout();
            // 
            // lblServerIP
            // 
            lblServerIP.AutoSize = true;
            lblServerIP.Location = new Point(20, 40);
            lblServerIP.Name = "lblServerIP";
            lblServerIP.Size = new Size(92, 28);
            lblServerIP.TabIndex = 0;
            lblServerIP.Text = "Server IP:";
            // 
            // txtIP
            // 
            txtIP.Location = new Point(100, 37);
            txtIP.Name = "txtIP";
            txtIP.Size = new Size(200, 34);
            txtIP.TabIndex = 1;
            txtIP.Text = "127.0.0.1";
            // 
            // btnConnect
            // 
            btnConnect.BackColor = Color.LightGreen;
            btnConnect.Cursor = Cursors.Hand;
            btnConnect.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnConnect.Location = new Point(320, 35);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(120, 35);
            btnConnect.TabIndex = 2;
            btnConnect.Text = "KẾT NỐI";
            btnConnect.UseVisualStyleBackColor = false;
            btnConnect.Click += btnConnect_Click;
            // 
            // btnDisconnect
            // 
            btnDisconnect.BackColor = Color.LightSalmon;
            btnDisconnect.Cursor = Cursors.Hand;
            btnDisconnect.Enabled = false;
            btnDisconnect.Location = new Point(450, 35);
            btnDisconnect.Name = "btnDisconnect";
            btnDisconnect.Size = new Size(120, 35);
            btnDisconnect.TabIndex = 3;
            btnDisconnect.Text = "Ngắt";
            btnDisconnect.UseVisualStyleBackColor = false;
            btnDisconnect.Click += btnDisconnect_Click;
            // 
            // btnSend
            // 
            btnSend.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSend.Location = new Point(880, 35);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(120, 35);
            btnSend.TabIndex = 4;
            btnSend.Text = "Ping Test";
            btnSend.UseVisualStyleBackColor = true;
            btnSend.Click += btnSend_Click;
            // 
            // lbLog
            // 
            lbLog.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lbLog.BackColor = Color.Black;
            lbLog.Font = new Font("Consolas", 11F);
            lbLog.ForeColor = Color.Lime;
            lbLog.FormattingEnabled = true;
            lbLog.Location = new Point(20, 556);
            lbLog.Name = "lbLog";
            lbLog.Size = new Size(980, 180);
            lbLog.TabIndex = 9;
            lbLog.SelectedIndexChanged += lbLog_SelectedIndexChanged;
            // 
            // grpMain
            // 
            grpMain.Controls.Add(lblServerIP);
            grpMain.Controls.Add(txtIP);
            grpMain.Controls.Add(btnConnect);
            grpMain.Controls.Add(btnDisconnect);
            grpMain.Controls.Add(btnSend);
            grpMain.Controls.Add(lblFolder);
            grpMain.Controls.Add(txtFolderPath);
            grpMain.Controls.Add(btnBrowse);
            grpMain.Controls.Add(lvFiles);
            grpMain.Controls.Add(lbLog);
            grpMain.Dock = DockStyle.Fill;
            grpMain.Location = new Point(0, 0);
            grpMain.Name = "grpMain";
            grpMain.Padding = new Padding(10);
            grpMain.Size = new Size(1024, 764);
            grpMain.TabIndex = 0;
            grpMain.TabStop = false;
            grpMain.Text = "Bảng điều khiển";
            // 
            // lblFolder
            // 
            lblFolder.AutoSize = true;
            lblFolder.Location = new Point(20, 90);
            lblFolder.Name = "lblFolder";
            lblFolder.Size = new Size(91, 28);
            lblFolder.TabIndex = 5;
            lblFolder.Text = "Thư mục:";
            // 
            // txtFolderPath
            // 
            txtFolderPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtFolderPath.BackColor = Color.WhiteSmoke;
            txtFolderPath.Location = new Point(100, 87);
            txtFolderPath.Name = "txtFolderPath";
            txtFolderPath.ReadOnly = true;
            txtFolderPath.Size = new Size(760, 34);
            txtFolderPath.TabIndex = 6;
            // 
            // btnBrowse
            // 
            btnBrowse.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowse.Location = new Point(880, 85);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(120, 35);
            btnBrowse.TabIndex = 7;
            btnBrowse.Text = "Chọn...";
            btnBrowse.UseVisualStyleBackColor = true;
            btnBrowse.Click += btnBrowse_Click;
            // 
            // lvFiles
            // 
            lvFiles.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lvFiles.Columns.AddRange(new ColumnHeader[] { colName, colSize, colTime });
            lvFiles.FullRowSelect = true;
            lvFiles.GridLines = true;
            lvFiles.Location = new Point(20, 140);
            lvFiles.Name = "lvFiles";
            lvFiles.Size = new Size(980, 396);
            lvFiles.TabIndex = 8;
            lvFiles.UseCompatibleStateImageBehavior = false;
            lvFiles.View = View.Details;
            lvFiles.SelectedIndexChanged += lvFiles_SelectedIndexChanged;
            // 
            // colName
            // 
            colName.Text = "Tên File";
            colName.Width = 500;
            // 
            // colSize
            // 
            colSize.Text = "Kích thước";
            colSize.Width = 250;
            // 
            // colTime
            // 
            colTime.Text = "Ngày sửa đổi";
            colTime.Width = 250;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(11F, 28F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1024, 764);
            Controls.Add(grpMain);
            Font = new Font("Segoe UI", 12F);
            Name = "Form1";
            Text = "MiniDropbox Client - Big Screen Edition";
            Load += Form1_Load;
            grpMain.ResumeLayout(false);
            grpMain.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
    }
}