namespace PCEnvanter
{
    partial class Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.dosyaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.yeniToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.prepareNewPcList = new System.Windows.Forms.ToolStripMenuItem();
            this.açToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pCListeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.windowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.yardımToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.destekAlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.işlemKütüğüToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dosyaToolStripMenuItem,
            this.windowToolStripMenuItem,
            this.yardımToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.MdiWindowListItem = this.windowToolStripMenuItem;
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1401, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // dosyaToolStripMenuItem
            // 
            this.dosyaToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.yeniToolStripMenuItem,
            this.açToolStripMenuItem,
            this.toolStripMenuItem1});
            this.dosyaToolStripMenuItem.Name = "dosyaToolStripMenuItem";
            this.dosyaToolStripMenuItem.Size = new System.Drawing.Size(51, 20);
            this.dosyaToolStripMenuItem.Text = "Dosya";
            // 
            // yeniToolStripMenuItem
            // 
            this.yeniToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.prepareNewPcList});
            this.yeniToolStripMenuItem.Name = "yeniToolStripMenuItem";
            this.yeniToolStripMenuItem.Size = new System.Drawing.Size(96, 22);
            this.yeniToolStripMenuItem.Text = "Yeni";
            // 
            // prepareNewPcList
            // 
            this.prepareNewPcList.Name = "prepareNewPcList";
            this.prepareNewPcList.Size = new System.Drawing.Size(163, 22);
            this.prepareNewPcList.Text = "PC Listesi Hazırla";
            this.prepareNewPcList.Click += new System.EventHandler(this.yeniPcListesiHazırlaToolStripMenuItem_Click);
            // 
            // açToolStripMenuItem
            // 
            this.açToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pCListeToolStripMenuItem});
            this.açToolStripMenuItem.Name = "açToolStripMenuItem";
            this.açToolStripMenuItem.Size = new System.Drawing.Size(96, 22);
            this.açToolStripMenuItem.Text = "Aç";
            // 
            // pCListeToolStripMenuItem
            // 
            this.pCListeToolStripMenuItem.Name = "pCListeToolStripMenuItem";
            this.pCListeToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.pCListeToolStripMenuItem.Text = "PC Liste";
            this.pCListeToolStripMenuItem.Click += new System.EventHandler(this.pCListeToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(93, 6);
            // 
            // windowToolStripMenuItem
            // 
            this.windowToolStripMenuItem.Name = "windowToolStripMenuItem";
            this.windowToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.windowToolStripMenuItem.Text = "Pencere";
            // 
            // yardımToolStripMenuItem
            // 
            this.yardımToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.destekAlToolStripMenuItem,
            this.işlemKütüğüToolStripMenuItem});
            this.yardımToolStripMenuItem.Name = "yardımToolStripMenuItem";
            this.yardımToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
            this.yardımToolStripMenuItem.Text = "Yardım";
            // 
            // destekAlToolStripMenuItem
            // 
            this.destekAlToolStripMenuItem.Name = "destekAlToolStripMenuItem";
            this.destekAlToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.destekAlToolStripMenuItem.Text = "Destek Al";
            this.destekAlToolStripMenuItem.Click += new System.EventHandler(this.destekAlToolStripMenuItem_Click);
            // 
            // işlemKütüğüToolStripMenuItem
            // 
            this.işlemKütüğüToolStripMenuItem.Name = "işlemKütüğüToolStripMenuItem";
            this.işlemKütüğüToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.işlemKütüğüToolStripMenuItem.Text = "İşlem Kütüğü";
            this.işlemKütüğüToolStripMenuItem.Click += new System.EventHandler(this.işlemKütüğüToolStripMenuItem_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1401, 502);
            this.Controls.Add(this.menuStrip1);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Main";
            this.Text = "PC Envanter v5.0";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem yardımToolStripMenuItem;
        private ToolStripMenuItem destekAlToolStripMenuItem;
		private ToolStripMenuItem dosyaToolStripMenuItem;
		private ToolStripSeparator toolStripMenuItem1;
		private ToolStripMenuItem yeniToolStripMenuItem;
		private ToolStripMenuItem açToolStripMenuItem;
		private ToolStripMenuItem pCListeToolStripMenuItem;
		private ToolStripMenuItem prepareNewPcList;
        private ToolStripMenuItem işlemKütüğüToolStripMenuItem;
        private ToolStripMenuItem windowToolStripMenuItem;
    }
}