using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace PCEnvanter
{
	public partial class LogWindow : Form
	{
		public LogWindow()
		{
			InitializeComponent();


			new Thread(() =>
			{
				while (this != null)
				{
					string? line;
					while (Main.log.TryDequeue(out line))
					{
						Invoke(() => { this.richTextBox1.Text += line.Split(Environment.NewLine)[0] + Environment.NewLine + Environment.NewLine; });
					}
					Thread.Sleep(1000);
				}
				
			}).Start();
		}
	}
}
