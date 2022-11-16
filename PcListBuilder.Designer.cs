namespace PCEnvanter
{
	partial class PcListBuilder
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.dosyaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.kaydetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.lv_pcl = new System.Windows.Forms.ListView();
            this.No = new System.Windows.Forms.ColumnHeader();
            this.PcName = new System.Windows.Forms.ColumnHeader();
            this.IP = new System.Windows.Forms.ColumnHeader();
            this.User = new System.Windows.Forms.ColumnHeader();
            this.Title = new System.Windows.Forms.ColumnHeader();
            this.PP = new System.Windows.Forms.ColumnHeader();
            this.DiskSpeed = new System.Windows.Forms.ColumnHeader();
            this.FE = new System.Windows.Forms.ColumnHeader();
            this.Brand = new System.Windows.Forms.ColumnHeader();
            this.Enclosure = new System.Windows.Forms.ColumnHeader();
            this.CPU = new System.Windows.Forms.ColumnHeader();
            this.Memory = new System.Windows.Forms.ColumnHeader();
            this.Disk = new System.Windows.Forms.ColumnHeader();
            this.Monitor = new System.Windows.Forms.ColumnHeader();
            this.Graphics = new System.Windows.Forms.ColumnHeader();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.buBilgisayarıYenileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listeEksikleriniTamamlaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.excelOlarakDışaAktarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dosyaToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1064, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.Visible = false;
            // 
            // dosyaToolStripMenuItem
            // 
            this.dosyaToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.kaydetToolStripMenuItem});
            this.dosyaToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
            this.dosyaToolStripMenuItem.Name = "dosyaToolStripMenuItem";
            this.dosyaToolStripMenuItem.Size = new System.Drawing.Size(51, 20);
            this.dosyaToolStripMenuItem.Text = "Dosya";
            // 
            // kaydetToolStripMenuItem
            // 
            this.kaydetToolStripMenuItem.Name = "kaydetToolStripMenuItem";
            this.kaydetToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.kaydetToolStripMenuItem.Text = "Kaydet...";
            this.kaydetToolStripMenuItem.Click += new System.EventHandler(this.kaydetToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.progressBar});
            this.statusStrip1.Location = new System.Drawing.Point(0, 408);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1064, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // progressBar
            // 
            this.progressBar.Maximum = 1000000;
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(100, 16);
            this.progressBar.Step = 1;
            // 
            // lv_pcl
            // 
            this.lv_pcl.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.No,
            this.PcName,
            this.IP,
            this.User,
            this.Title,
            this.PP,
            this.DiskSpeed,
            this.FE,
            this.Brand,
            this.Enclosure,
            this.CPU,
            this.Memory,
            this.Disk,
            this.Monitor,
            this.Graphics});
            this.lv_pcl.ContextMenuStrip = this.contextMenuStrip1;
            this.lv_pcl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lv_pcl.FullRowSelect = true;
            this.lv_pcl.Location = new System.Drawing.Point(0, 24);
            this.lv_pcl.MultiSelect = false;
            this.lv_pcl.Name = "lv_pcl";
            this.lv_pcl.Size = new System.Drawing.Size(1064, 384);
            this.lv_pcl.TabIndex = 3;
            this.lv_pcl.UseCompatibleStateImageBehavior = false;
            this.lv_pcl.View = System.Windows.Forms.View.Details;
            this.lv_pcl.VirtualMode = true;
            this.lv_pcl.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lv_pcl_ColumnClick);
            this.lv_pcl.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.lv_pcl_RetrieveVirtualItem);
            // 
            // No
            // 
            this.No.Text = "#";
            this.No.Width = 40;
            // 
            // PcName
            // 
            this.PcName.Text = "Bilgisayar Adı";
            this.PcName.Width = 150;
            // 
            // IP
            // 
            this.IP.Text = "IP";
            this.IP.Width = 100;
            // 
            // User
            // 
            this.User.Text = "Kullanıcı";
            this.User.Width = 200;
            // 
            // Title
            // 
            this.Title.Text = "Ünvan";
            this.Title.Width = 120;
            // 
            // PP
            // 
            this.PP.Text = "İşlemci Hızı";
            this.PP.Width = 80;
            // 
            // DiskSpeed
            // 
            this.DiskSpeed.Text = "Disk Hızı";
            // 
            // FE
            // 
            this.FE.Text = "Ferahlık";
            this.FE.Width = 80;
            // 
            // Brand
            // 
            this.Brand.Text = "Marka/Model";
            this.Brand.Width = 100;
            // 
            // Enclosure
            // 
            this.Enclosure.Text = "Kasa Tipi";
            this.Enclosure.Width = 100;
            // 
            // CPU
            // 
            this.CPU.Text = "İşlemci";
            this.CPU.Width = 100;
            // 
            // Memory
            // 
            this.Memory.Text = "Bellek";
            this.Memory.Width = 100;
            // 
            // Disk
            // 
            this.Disk.Text = "Disk";
            this.Disk.Width = 100;
            // 
            // Monitor
            // 
            this.Monitor.Text = "Monitör";
            // 
            // Graphics
            // 
            this.Graphics.Text = "Ekran Kartı";
            this.Graphics.Width = 140;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buBilgisayarıYenileToolStripMenuItem,
            this.listeEksikleriniTamamlaToolStripMenuItem,
            this.excelOlarakDışaAktarToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(205, 70);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // buBilgisayarıYenileToolStripMenuItem
            // 
            this.buBilgisayarıYenileToolStripMenuItem.Name = "buBilgisayarıYenileToolStripMenuItem";
            this.buBilgisayarıYenileToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.buBilgisayarıYenileToolStripMenuItem.Text = "Bu Bilgisayarı Yenile";
            this.buBilgisayarıYenileToolStripMenuItem.Click += new System.EventHandler(this.buBilgisayarıYenileToolStripMenuItem_Click);
            // 
            // listeEksikleriniTamamlaToolStripMenuItem
            // 
            this.listeEksikleriniTamamlaToolStripMenuItem.Name = "listeEksikleriniTamamlaToolStripMenuItem";
            this.listeEksikleriniTamamlaToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.listeEksikleriniTamamlaToolStripMenuItem.Text = "Liste Eksiklerini Tamamla";
            this.listeEksikleriniTamamlaToolStripMenuItem.Click += new System.EventHandler(this.listeEksikleriniTamamlaToolStripMenuItem_Click);
            // 
            // excelOlarakDışaAktarToolStripMenuItem
            // 
            this.excelOlarakDışaAktarToolStripMenuItem.Name = "excelOlarakDışaAktarToolStripMenuItem";
            this.excelOlarakDışaAktarToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.excelOlarakDışaAktarToolStripMenuItem.Text = "Excel Olarak Dışa Aktar...";
            this.excelOlarakDışaAktarToolStripMenuItem.Click += new System.EventHandler(this.excelOlarakDışaAktarToolStripMenuItem_Click);
            // 
            // PcListBuilder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1064, 430);
            this.Controls.Add(this.lv_pcl);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "PcListBuilder";
            this.ShowInTaskbar = false;
            this.Text = "Pc Liste Oluşturucu";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private MenuStrip menuStrip1;
		private ToolStripMenuItem dosyaToolStripMenuItem;
		private ToolStripMenuItem kaydetToolStripMenuItem;
        private StatusStrip statusStrip1;
        private ToolStripProgressBar progressBar;
        private ListView lv_pcl;
        private ColumnHeader No;
        private ColumnHeader PcName;
        private ColumnHeader IP;
        private ColumnHeader User;
        private ColumnHeader Title;
        private ColumnHeader PP;
        private ColumnHeader FE;
        private ColumnHeader Brand;
        private ColumnHeader Enclosure;
        private ColumnHeader CPU;
        private ColumnHeader Memory;
        private ColumnHeader Disk;
        private ColumnHeader Monitor;
        private ColumnHeader Graphics;
        private ColumnHeader DiskSpeed;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem buBilgisayarıYenileToolStripMenuItem;
        private ToolStripMenuItem listeEksikleriniTamamlaToolStripMenuItem;
        private ToolStripMenuItem excelOlarakDışaAktarToolStripMenuItem;
    }
}