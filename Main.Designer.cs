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
			this.prepareNewPcList = new System.Windows.Forms.ToolStripMenuItem();
			this.varOlanListeyiTamamlaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.yardımToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.destekAlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dosyaToolStripMenuItem,
            this.yardımToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(1699, 24);
			this.menuStrip1.TabIndex = 0;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// dosyaToolStripMenuItem
			// 
			this.dosyaToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.prepareNewPcList,
            this.varOlanListeyiTamamlaToolStripMenuItem,
            this.toolStripMenuItem1});
			this.dosyaToolStripMenuItem.Name = "dosyaToolStripMenuItem";
			this.dosyaToolStripMenuItem.Size = new System.Drawing.Size(51, 20);
			this.dosyaToolStripMenuItem.Text = "Dosya";
			// 
			// prepareNewPcList
			// 
			this.prepareNewPcList.Name = "prepareNewPcList";
			this.prepareNewPcList.Size = new System.Drawing.Size(205, 22);
			this.prepareNewPcList.Text = "Yeni PC Listesi Hazırla";
			this.prepareNewPcList.Click += new System.EventHandler(this.yeniPcListesiHazırlaToolStripMenuItem_Click);
			// 
			// varOlanListeyiTamamlaToolStripMenuItem
			// 
			this.varOlanListeyiTamamlaToolStripMenuItem.Name = "varOlanListeyiTamamlaToolStripMenuItem";
			this.varOlanListeyiTamamlaToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
			this.varOlanListeyiTamamlaToolStripMenuItem.Text = "Var Olan Listeyi Tamamla";
			// 
			// yardımToolStripMenuItem
			// 
			this.yardımToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.destekAlToolStripMenuItem});
			this.yardımToolStripMenuItem.Name = "yardımToolStripMenuItem";
			this.yardımToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
			this.yardımToolStripMenuItem.Text = "Yardım";
			// 
			// destekAlToolStripMenuItem
			// 
			this.destekAlToolStripMenuItem.Name = "destekAlToolStripMenuItem";
			this.destekAlToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
			this.destekAlToolStripMenuItem.Text = "Destek Al";
			this.destekAlToolStripMenuItem.Click += new System.EventHandler(this.destekAlToolStripMenuItem_Click);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(202, 6);
			// 
			// Main
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1699, 917);
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
		private ToolStripMenuItem prepareNewPcList;
		private ToolStripMenuItem varOlanListeyiTamamlaToolStripMenuItem;
		private ToolStripSeparator toolStripMenuItem1;
	}
}