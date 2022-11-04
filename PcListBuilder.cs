using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.DirectoryServices.AccountManagement;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PCEnvanter
{
	public partial class PcListBuilder : Form
	{
		PcList pcl = new PcList();

		string fileName = "";
		int pclc = 0;
		public PcListBuilder()
		{
			InitializeComponent();
		}
		public PcListBuilder(List<string> prefixList)
		{
			pcl.PrefixList = prefixList;

			InitializeComponent();

			//List<string> pcl = GetPCList(prefixList);

			List<string> pcnameslist = new List<string>();
			pcnameslist.Add("AKN-PC");

			foreach (string pcname in pcnameslist)
			{
				BackgroundWorker backgroundWorker = new BackgroundWorker();
				backgroundWorker.DoWork += new DoWorkEventHandler(this.Worker_DoWork);
				backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.Worker_RunWorkerCompleted);
				backgroundWorker.RunWorkerAsync((object)pcname);
			}
		}

		private List<string> GetPCList(List<string> prefixes)
		{
			PrincipalContext context = getPrincipalContext();
			ConcurrentBag<string> pcnamelist = new ConcurrentBag<string>();

			foreach (string prefix in prefixes)
			{
				ComputerPrincipal queryFilter = new ComputerPrincipal(context);
				queryFilter.Name = prefix + "*";
				foreach (Principal principal in new PrincipalSearcher((Principal)queryFilter).FindAll())
				{
					if (principal is ComputerPrincipal computerPrincipal)
						pcnamelist.Add(computerPrincipal.Name);
				}
			}
			return pcnamelist.ToList();
		}

		private PrincipalContext getPrincipalContext()
		{
#if DEBUG
			return new PrincipalContext(ContextType.Domain, "csb.local", "akin.demirtug", "Sandalye22");
#else
			return new PrincipalContext(ContextType.Domain, "csb.local");
#endif
		}

		private void Worker_DoWork(object? sender, DoWorkEventArgs e)
		{
			PC pc = new PC() { Name = e.Argument?.ToString() ?? "PC71" };
			pc.RetrieveInfo();
			e.Result = pc;
		}
		private void Worker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Result == null)
				return;
			
			PC? pc = (PC)e.Result;

			ListViewItem lvi = new ListViewItem(Interlocked.Increment(ref pclc).ToString());
			lvi.SubItems.Add(pc.Name);
			lvi.SubItems.Add(pc.IP);
			lvi.SubItems.Add(pc.User?.Name);
			lvi.SubItems.Add(pc.User?.Title);
			lvi.SubItems.Add(pc.Cpu?.Score.ToString());
			lvi.SubItems.Add(pc.Fluency.ToString());
			lvi.SubItems.Add(pc.Model);
			lvi.SubItems.Add(pc.Enclosure);
			lvi.SubItems.Add(pc.Cpu?.Model);
			lvi.SubItems.Add(pc.Memory?.ToString());
			lvi.SubItems.Add(pc.Disk?.ToString());
			lvi.SubItems.Add(pc.VideoCard?.Name);

			lock (lv_pcl)
			{
				lv_pcl.Items.Add(lvi);
			}
		}

		private void kaydetToolStripMenuItem_Click(object sender, EventArgs e)
		{
			
		}
	}
}
