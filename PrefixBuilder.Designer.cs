namespace PCEnvanter
{
	partial class PrefixBuilder
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
			this.tb_prefix = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.search = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// tb_prefix
			// 
			this.tb_prefix.Location = new System.Drawing.Point(12, 32);
			this.tb_prefix.Name = "tb_prefix";
			this.tb_prefix.Size = new System.Drawing.Size(631, 23);
			this.tb_prefix.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 14);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(183, 15);
			this.label1.TabIndex = 1;
			this.label1.Text = "Bilgisayar Ön Adları( ; ile ayrılmış)";
			// 
			// search
			// 
			this.search.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.search.Location = new System.Drawing.Point(194, 61);
			this.search.Name = "search";
			this.search.Size = new System.Drawing.Size(312, 23);
			this.search.TabIndex = 2;
			this.search.Text = "İle Başlayan İsimler Verilmiş Bilgisayarları Ara";
			this.search.UseVisualStyleBackColor = true;
			// 
			// PrefixBuilder
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(655, 91);
			this.Controls.Add(this.search);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.tb_prefix);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PrefixBuilder";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "PrefixBuilder";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private TextBox tb_prefix;
		private Label label1;
		private Button search;
	}
}