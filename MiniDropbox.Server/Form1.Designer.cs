namespace MiniDropbox.Server
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ListBox lbLog;
        private System.Windows.Forms.DataGridView dgvClients;

        // --- CÁC CONTROL MỚI ---
        private System.Windows.Forms.ListView lvServerFiles;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colSize;
        private System.Windows.Forms.ColumnHeader colTime;
        private System.Windows.Forms.GroupBox grpClients;
        private System.Windows.Forms.GroupBox grpFiles;
        private System.Windows.Forms.Label lblServerPath;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lblStatus = new System.Windows.Forms.Label();
            this.lbLog = new System.Windows.Forms.ListBox();
            this.dgvClients = new System.Windows.Forms.DataGridView();
            this.lvServerFiles = new System.Windows.Forms.ListView();
            this.colName = new System.Windows.Forms.ColumnHeader();
            this.colSize = new System.Windows.Forms.ColumnHeader();
            this.colTime = new System.Windows.Forms.ColumnHeader();
            this.grpClients = new System.Windows.Forms.GroupBox();
            this.grpFiles = new System.Windows.Forms.GroupBox();
            this.lblServerPath = new System.Windows.Forms.Label();

            ((System.ComponentModel.ISupportInitialize)(this.dgvClients)).BeginInit();
            this.grpClients.SuspendLayout();
            this.grpFiles.SuspendLayout();
            this.SuspendLayout();

            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblStatus.ForeColor = System.Drawing.Color.Blue;
            this.lblStatus.Location = new System.Drawing.Point(12, 9);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(127, 21);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "Server Stopped";

            // 
            // lblServerPath
            // 
            this.lblServerPath.AutoSize = true;
            this.lblServerPath.Location = new System.Drawing.Point(200, 14);
            this.lblServerPath.Name = "lblServerPath";
            this.lblServerPath.Size = new System.Drawing.Size(80, 15);
            this.lblServerPath.Text = "Kho dữ liệu: ...";

            // 
            // grpClients (Khung bên trái)
            // 
            this.grpClients.Controls.Add(this.dgvClients);
            this.grpClients.Location = new System.Drawing.Point(12, 40);
            this.grpClients.Name = "grpClients";
            this.grpClients.Size = new System.Drawing.Size(280, 250);
            this.grpClients.TabIndex = 1;
            this.grpClients.TabStop = false;
            this.grpClients.Text = "Danh sách Client đang Online";

            // 
            // dgvClients
            // 
            this.dgvClients.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvClients.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvClients.Location = new System.Drawing.Point(3, 19);
            this.dgvClients.Name = "dgvClients";
            this.dgvClients.RowHeadersVisible = false;
            this.dgvClients.Size = new System.Drawing.Size(274, 228);
            this.dgvClients.TabIndex = 0;

            // 
            // grpFiles (Khung bên phải - MỚI)
            // 
            this.grpFiles.Controls.Add(this.lvServerFiles);
            this.grpFiles.Location = new System.Drawing.Point(300, 40);
            this.grpFiles.Name = "grpFiles";
            this.grpFiles.Size = new System.Drawing.Size(400, 250);
            this.grpFiles.TabIndex = 2;
            this.grpFiles.TabStop = false;
            this.grpFiles.Text = "Kho dữ liệu trên Server";

            // 
            // lvServerFiles
            // 
            this.lvServerFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colSize,
            this.colTime});
            this.lvServerFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvServerFiles.FullRowSelect = true;
            this.lvServerFiles.GridLines = true;
            this.lvServerFiles.Location = new System.Drawing.Point(3, 19);
            this.lvServerFiles.Name = "lvServerFiles";
            this.lvServerFiles.Size = new System.Drawing.Size(394, 228);
            this.lvServerFiles.TabIndex = 0;
            this.lvServerFiles.View = System.Windows.Forms.View.Details;

            // Cột của ListView
            this.colName.Text = "Tên File"; this.colName.Width = 180;
            this.colSize.Text = "Size"; this.colSize.Width = 80;
            this.colTime.Text = "Ngày cập nhật"; this.colTime.Width = 180;

            // 
            // lbLog (Log ở dưới cùng)
            // 
            this.lbLog.FormattingEnabled = true;
            this.lbLog.ItemHeight = 15;
            this.lbLog.Location = new System.Drawing.Point(12, 300);
            this.lbLog.Name = "lbLog";
            this.lbLog.Size = new System.Drawing.Size(688, 124);
            this.lbLog.TabIndex = 3;

            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(720, 440);
            this.Controls.Add(this.lbLog);
            this.Controls.Add(this.grpFiles);
            this.Controls.Add(this.grpClients);
            this.Controls.Add(this.lblServerPath);
            this.Controls.Add(this.lblStatus);
            this.Name = "Form1";
            this.Text = "MiniDropbox Server Dashboard";
            this.Load += new System.EventHandler(this.Form1_Load);

            ((System.ComponentModel.ISupportInitialize)(this.dgvClients)).EndInit();
            this.grpClients.ResumeLayout(false);
            this.grpFiles.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}