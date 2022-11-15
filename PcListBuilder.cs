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

			//PC pc = new PC() { Name = "P71DURSUNTEKBAL" };
			//pc.RetrieveInfo();

			//var score = pc.Fluency;

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
						{
							pcl.pcl.Add(pc);
							cpc.Enqueue(pc);

							PC? pcx;
                            while (cpc.TryDequeue(out pcx))
								addToListViewCache(pcx);
						}
					}catch(Exception)
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
            ListViewItem lvi = new ListViewItem(Interlocked.Increment(ref pclc).ToString());
            lvi.SubItems.Add(pc.Name);
            lvi.SubItems.Add(pc.IP == "0.0.0.0" ? "" : pc.IP);
            lvi.SubItems.Add(pc.User?.Name);
            lvi.SubItems.Add(pc.User?.Title);
            lvi.SubItems.Add(pc.Cpu?.Score.ToString());
            lvi.SubItems.Add(pc.Disk?.Score.ToString());
            lvi.SubItems.Add(pc.IP == "0.0.0.0" ? "" : pc.Fluency.ToString("F1"));
            lvi.SubItems.Add(pc.Model);
            lvi.SubItems.Add(pc.Enclosure);
            lvi.SubItems.Add(pc.Cpu?.Model);
            lvi.SubItems.Add(pc.Memory?.ToString());
            lvi.SubItems.Add(pc.Disk?.ToString());
            lvi.SubItems.Add(pc.Monitor?.Size.ToString("F1").Length > 0 ? pc.Monitor?.Size.ToString("F1") + " inç" : "");
            lvi.SubItems.Add(pc.VideoCard?.Name);

            Cache.Add(lvi);
            lv_pcl.VirtualListSize = Cache.Count;
        }

        void addToListViewCache(PC pc)
        {
            ListViewItem lvi = new ListViewItem(Interlocked.Increment(ref pclc).ToString());
			lvi.SubItems.Add(pc.Name);
			lvi.SubItems.Add(pc.IP == "0.0.0.0" ? "": pc.IP);
			lvi.SubItems.Add(pc.User?.Name);
			lvi.SubItems.Add(pc.User?.Title);
			lvi.SubItems.Add(pc.Cpu?.Score.ToString());
            lvi.SubItems.Add(pc.Disk?.Score.ToString());
            lvi.SubItems.Add(pc.IP == "0.0.0.0" ? "" : pc.Fluency.ToString("F1"));
            lvi.SubItems.Add(pc.Model);
			lvi.SubItems.Add(pc.Enclosure);
			lvi.SubItems.Add(pc.Cpu?.Model);
			lvi.SubItems.Add(pc.Memory?.ToString());
			lvi.SubItems.Add(pc.Disk?.ToString());
			lvi.SubItems.Add(pc.Monitor?.Size.ToString("F1").Length > 0 ? pc.Monitor?.Size.ToString("F1") + " inç" : "");
			lvi.SubItems.Add(pc.VideoCard?.Name);

			Cache.Add(lvi);
			
			//lvil = lvil.OrderBy(x => Convert.ToInt32(x.SubItems[0].Text)).ToList();
			lv_pcl.Invoke(() => { lv_pcl.VirtualListSize = Cache.Count;});
        }


		private void kaydetToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (fileName == "")
			{
				SaveFileDialog sfd = new SaveFileDialog();
				if (sfd.ShowDialog() != DialogResult.OK)
					return;
				fileName = sfd.FileName;
			}
			pcl.FlushAsJson(fileName);
		}

        private void lv_pcl_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
			//Debug.WriteLine("lv_pcl_RetrieveVirtualItem");
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
    }
}
