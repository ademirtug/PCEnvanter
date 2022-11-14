using System.Management;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.ComponentModel;

namespace PCEnvanter
{
	public partial class Main : Form
	{
		public static List<CPU> cpuList = new List<CPU>();
        public static List<Disk> diskList = new List<Disk>();

#if DEBUG
        public static string dpath = "../../../data/";
		string cpuDataFile = "../../../data/data.txt";
        string diskDataFile = "../../../data/hdd_data.txt";
#else
		public static string dpath = "./data/";
		string cpuDataFile = "./data/data.txt";
		string diskDataFile = "./data/hdd_data.txt";

#endif

        public Main()
		{
			InitializeComponent();

            foreach (string line in File.ReadAllLines(cpuDataFile))
			{
				string[] sp = line.Split("\t");
				Main.cpuList.Add(new CPU() { Name = sp[0], Score = Convert.ToDouble(sp[1]) });
			}

			string[] dlines = File.ReadAllLines(diskDataFile);
            foreach (string lx in File.ReadAllLines(diskDataFile))
            {
                string[] sp = lx.Split("\t");
                string scap = sp[1].Split(" ")[0];

                double sib = Convert.ToDouble(scap) * (sp[1].Split(" ")[1] == "GB" ? 1024*1024*1024 : (double)1024*1024*1024*1024);

                Main.diskList.Add(new Disk() { Model = sp[0], scap=sp[1], Capacity=Convert.ToUInt64(sib), Score = Convert.ToDouble(sp[2]) });
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
			if (ofd.ShowDialog() != DialogResult.OK)
				return;

			PcListBuilder pcb = new PcListBuilder(ofd.FileName);
			pcb.MdiParent = this;
			pcb.Show();
		}
	}
}