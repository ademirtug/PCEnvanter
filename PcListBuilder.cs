using NPOI.SS.Formula.Functions;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;


namespace PCEnvanter
{
	public partial class PcListBuilder : Form
	{
		PcList pcl = new PcList();
		ConcurrentQueue<PC> cpc = new ConcurrentQueue<PC>();
		List<ListViewItem> Cache = new List<ListViewItem>();
		ListViewItem? selectedLvi;

        int sortColumn = -1;
		string fileName = "";
		int pclc = 0;
		public PcListBuilder()
		{
			InitializeComponent();
        }

        //WDC WD5000AAKX-22ERMA0
        public PcListBuilder(string fileName) : this()
		{
			pcl = PcList.LoadFromFile(fileName);

			foreach (PC pc in pcl.pcl)
			{
				addToListView(pc);
			}
		}

		public PcListBuilder(List<string> prefixList) : this()
		{
			pcl.PrefixList = prefixList;
			progressBar.Value = 0;

            BackgroundWorker bw = new BackgroundWorker();
			bw.DoWork += Bw_DoWork;
			bw.RunWorkerCompleted += Bw_RunWorkerCompleted;
			bw.RunWorkerAsync(prefixList);
		}

        private void Bw_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
			if (e.Result == null)
				return;
			List<string> pcNamesList = (List<string>)e.Result;
			if (pcNamesList.Count == 0)
				return;

			progressBar.Step = 100 / Math.Max(1, pcNamesList.Count);
            foreach (string pcname in pcNamesList)
			{
				new Thread(() =>
				{
					try
					{
						PC pc = new PC() { Name = pcname ?? "PC71" };
						pc.RetrieveInfo();

						lock (pcl)
							pcl.pcl.Add(pc);

                        cpc.Enqueue(pc);

                        PC? pcx;
                        while (cpc.TryDequeue(out pcx))
						{
                            addToListViewCache(pcx);
                            Invoke(() => { progressBar.Value += 1000000 / Math.Max(1, pcNamesList.Count);});
                        }
                    }
                    catch(Exception)
                    {
						
                    }
				}).Start();
			}
		}

		private void Bw_DoWork(object? sender, DoWorkEventArgs e)
        {
			List<string> prefixList = (List<string>)e.Argument!;
			List<string> pcnameslist = GetPCList(prefixList!);
			e.Result = pcnameslist;
		
        }

        private List<string> GetPCList(List<string> prefixes)
		{
			PrincipalContext? context = getPrincipalContext();
			if(context == null)
				return new List<string>();


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

		private PrincipalContext? getPrincipalContext()
		{
			try
			{
#if DEBUG
				return new PrincipalContext(ContextType.Domain, "csb.local", "akin.demirtug", "Sandalye22");
#else
				return new PrincipalContext(ContextType.Domain, "csb.local");
#endif
			}catch(Exception)
			{

			}
			return null;
		}

        void addToListView(PC pc)
        {
			Cache.Add(pc.GetLVI(Interlocked.Increment(ref pclc).ToString()));
            lv_pcl.VirtualListSize = Cache.Count;
        }

        void addToListViewCache(PC pc)
        {
            Cache.Add(pc.GetLVI(Interlocked.Increment(ref pclc).ToString()));
            lv_pcl.Invoke(() => { 
				lv_pcl.VirtualListSize = Cache.Count;
            });
        }


		private void kaydetToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (fileName == "")
			{
				SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "PC Liste | *.json";
                sfd.DefaultExt = "json";

                if (sfd.ShowDialog() != DialogResult.OK)
					return;
				fileName = sfd.FileName;
			}
			pcl.FlushAsJson(fileName);
		}

        private void lv_pcl_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
			ListViewItem? lvi = new ListViewItem("NA");
			lvi = Cache.ElementAt(e.ItemIndex);
			e.Item = lvi;
        }

        private void lv_pcl_ColumnClick(object sender, ColumnClickEventArgs e)
       {
            bool isnum = false;
            foreach (ListViewItem lvi in Cache)
            {
                if (lvi.SubItems[e.Column].Text.Length > 0)
                {
                    isnum = Double.TryParse(lvi.SubItems[e.Column].Text, out _);
                    break;
                }
            }


            if (isnum)
            {
                Cache = Cache.OrderBy(x => {
                    string sortVal = x.SubItems[e.Column].Text;
                    if (sortVal == "")
                        sortVal = "0";

                    return Convert.ToDouble(sortVal);
                }).ToList();
            }
            else
                Cache = Cache?.OrderBy(x => x.SubItems[e.Column].Text).ToList();


			if (sortColumn == e.Column)
			{
				Cache?.Reverse();
				sortColumn = -1;
			}
			else
			{
                sortColumn = e.Column;
            }


            lv_pcl.Refresh();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
			selectedLvi = lv_pcl.GetItemAt(lv_pcl.PointToClient(Cursor.Position).X, lv_pcl.PointToClient(Cursor.Position).Y);
			buBilgisayarıYenileToolStripMenuItem.Enabled = selectedLvi == null ? false : true;

        }

        private void buBilgisayarıYenileToolStripMenuItem_Click(object sender, EventArgs e)
        {
			if(selectedLvi == null) return;

			string pcname = selectedLvi.SubItems[1].Text;
            new Thread(() =>
            {
                try
                {
                    PC pc = pcl.pcl.FirstOrDefault(px => px.Name == pcname) ?? new PC() { Name = pcname };
                    pc.RetrieveInfo();

                    lock (Cache)
                    {
                        ListViewItem? lvi = Cache.FirstOrDefault(x => x.SubItems[1].Text == pcname) ?? null;
						
                        Cache.RemoveAll(x => x.SubItems[1].Text == pcname);
                        Cache.Add(pc.GetLVI(lvi?.SubItems[0].Text ?? "1000"));
						lvi = null;
                    }
                    lv_pcl.Invoke(() => { lv_pcl.Refresh(); });
                }
                catch (Exception)
                {

                }
            }).Start();
        }

        private void listeEksikleriniTamamlaToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
