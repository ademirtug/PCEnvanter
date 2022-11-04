using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PCEnvanter
{
	public partial class PrefixBuilder : Form
	{
		public PrefixBuilder()
		{
			InitializeComponent();
		}
		public PrefixBuilder(string plaka) : this()
		{
			tb_prefix.Text = $"P{plaka};A{plaka};L{plaka}";
		}

		public string prefixList
		{
			get
			{
				return tb_prefix.Text;
			}
		}
	}
}
