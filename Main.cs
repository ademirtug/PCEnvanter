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

		ConcurrentBag<PC> pclist = new ConcurrentBag<PC>();
		public static List<CPU> cpuList = new List<CPU>();

#if DEBUG
		public static string dpath = "../../../data/";
		string datafile = "../../../data/data.txt";
#else
		public static string dpath = "./data/";
		string datafile = "./data/data.txt"
#endif

		public Main()
		{
			InitializeComponent();
			Main.cpuList = new List<CPU>();

			string[] lines = File.ReadAllLines(datafile);
			foreach (string line in lines)
			{
				string[] sp = line.Split("\t");
				Main.cpuList.Add(new CPU() { Name = sp[0], Score = Convert.ToDouble(sp[1]) });
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
			catch (Exception ex)
			{

			}

		}
	}
}