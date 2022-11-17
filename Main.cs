using System.Management;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.ComponentModel;
using NPOI.XWPF.UserModel;
using System.Globalization;

namespace PCEnvanter
{
	public partial class Main : Form
	{
		public static List<CPU> cpuList = new List<CPU>();
        public static List<Disk> diskList = new List<Disk>();
        public static ConcurrentQueue<string> log = new();

#if DEBUG
        public static string dpath = "../../../data/";
#else
		public static string dpath = "data/";

#endif

        public Main()
		{
			InitializeComponent();



            Main.log.Enqueue($"{System.Text.Encoding.Default.ToString()}");

            string[] cpuLines = Properties.Resources.cpuData.Split("\r\n");
            foreach (string line in cpuLines)
			{
				string[] sp = line.Split("\t");
				Main.cpuList.Add(new CPU() { Name = sp[0], Score = Convert.ToDouble(sp[1], new NumberFormatInfo() { NumberDecimalSeparator = ".", NumberGroupSeparator = "," }) });
			}

            string[] diskLines = Properties.Resources.hddData.Split("\r\n");
            foreach (string lx in diskLines)
            {
                string[] sp = lx.Split("\t");
				double dcap = Convert.ToDouble(sp[1].Split(" ")[0], new NumberFormatInfo() { NumberDecimalSeparator = ".", NumberGroupSeparator = ","} );
                double sib = dcap * (sp[1].Split(" ")[1] == "GB" ? 1024*1024*1024 : (double)1024*1024*1024*1024);
				double ds = Convert.ToDouble(sp[2], new NumberFormatInfo() { NumberDecimalSeparator = ".", NumberGroupSeparator = "," });

                Main.diskList.Add(new Disk() { Model = sp[0], scap=sp[1], Capacity=Convert.ToUInt64(sib), Score = ds });
            }
        }

		private void destekAlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            MessageBox.Show("Sorularınız için akin.demirtug@csb.gov.tr adresine mesaj gönderebilirsiniz.", "Destek Al");
        }

        private void yeniPcListesiHazırlaToolStripMenuItem_Click(object sender, EventArgs e)
        {

			try
			{
				PrefixBuilder pb = new PrefixBuilder(Environment.MachineName.Substring(1, 2));
				if (pb.ShowDialog() != DialogResult.OK)
					return;

				PcListBuilder pcb = new PcListBuilder(pb.prefixList.Split(";").ToList());
				pcb.MdiParent = this;
				pcb.Show();
			}
			catch (Exception)
			{

			}

		}

		private void pCListeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "PC Liste | *.json";
            ofd.DefaultExt = "json";

            if (ofd.ShowDialog() != DialogResult.OK)
				return;

			PcListBuilder pcb = new PcListBuilder(ofd.FileName);
			pcb.MdiParent = this;
			pcb.Show();
		}

        private void işlemKütüğüToolStripMenuItem_Click(object sender, EventArgs e)
        {
			LogWindow lg = new LogWindow();
			lg.MdiParent = this;
			lg.Show();
        }
    }
}