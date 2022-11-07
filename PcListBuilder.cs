using System.Collections.Concurrent;
using System.ComponentModel;
using System.DirectoryServices.AccountManagement;


namespace PCEnvanter
{
	public partial class PcListBuilder : Form
	{
		PcList pcl = new PcList();
		ConcurrentQueue<PC> cpc = new ConcurrentQueue<PC>();

		string fileName = "";
		int pclc = 0;
		public PcListBuilder()
		{
			InitializeComponent();
		}
		public PcListBuilder(string fileName) : this()
		{
			pcl = PcList.LoadFromFile(fileName);

			foreach(PC pc in pcl.pcl)
			{
				addToListView(pc);
			}
		}

		public PcListBuilder(List<string> prefixList) : this()
		{
			pcl.PrefixList = prefixList;
			ThreadPool.SetMinThreads(20, 20);


			//List<string> pcnameslist = GetPCList(prefixList);
            List<string> pcnameslist = new List<string>();
            pcnameslist.Add("P71KIRIKKALE33");

            //Method 1
            //foreach (string pcname in pcnameslist)
            //{
            //    BackgroundWorker backgroundWorker = new BackgroundWorker();
            //    backgroundWorker.DoWork += new DoWorkEventHandler(this.Worker_DoWork);
            //    backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.Worker_RunWorkerCompleted);
            //    backgroundWorker.RunWorkerAsync((object)pcname);
            //}

            //Method 2
            //int c = 0;
            //         foreach (string pcname in pcnameslist)
            //         {
            //	if (++c == 20)
            //		break;

            //	Task.Run(() =>
            //             {
            //		PC pc = new PC() { Name = pcname ?? "PC71" };
            //		pc.RetrieveInfo();
            //                 lock (pcl)
            //                 {
            //                     pcl.pcl.Add(pc);
            //			//Dispatcher.Invoke(new Action(() => { addToListView(pc); }));
            //		 this.Invoke(()=> addToListView(pc));
            //		}
            //             });
            //}

            //Method 3
            //BackgroundWorker rw = new BackgroundWorker();
            //rw.DoWork += new DoWorkEventHandler(this.RefreshWorker_DoWork);
            ////backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.Worker_RunWorkerCompleted);
            //rw.RunWorkerAsync();

            //int c = 0;
            //foreach (string pcname in pcnameslist)
            //{
            //    Task.Run(() =>
            //    {
            //        PC pc = new PC() { Name = pcname ?? "PC71" };
            //        pc.RetrieveInfo();

            //        lock (pcl)
            //        {
            //            pcl.pcl.Add(pc);
            //        }
            //        cpc.Enqueue(pc);

            //    });
            //}


            //Method 4
            BackgroundWorker rw = new BackgroundWorker();
            rw.DoWork += new DoWorkEventHandler(this.RefreshWorker_DoWork);
            //backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.Worker_RunWorkerCompleted);
            rw.RunWorkerAsync();

            int c = 0;
            foreach (string pcname in pcnameslist)
            {
                new Thread(() =>
                         {
                             PC pc = new PC() { Name = pcname ?? "PC71" };
                             pc.RetrieveInfo();

                             lock (pcl)
                             {
                                 pcl.pcl.Add(pc);
                             }
                             cpc.Enqueue(pc);
                         }).Start();
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

		private void RefreshWorker_DoWork(object? sender, DoWorkEventArgs e)
		{
            while (true)
            {
				Thread.Sleep(5000);
				if(cpc.Count > 0)
                {
					PC? pc;
					if (cpc.TryDequeue(out pc))
						this.Invoke(() => addToListView(pc));

				}
            }
		}

		private void Worker_DoWork(object? sender, DoWorkEventArgs e)
		{
			PC pc = new PC() { Name = e.Argument?.ToString() ?? "PC71" };
			pc.RetrieveInfo();
			lock(pcl)
				pcl.pcl.Add(pc);

			e.Result = pc;
		}
		private void Worker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Result == null)
				return;
			
			PC? pc = (PC)e.Result;
			addToListView(pc);

		}

		void addToListView(PC pc)
		{
			ListViewItem lvi = new ListViewItem(Interlocked.Increment(ref pclc).ToString());
			lvi.SubItems.Add(pc.Name);
			lvi.SubItems.Add(pc.IP);
			lvi.SubItems.Add(pc.User?.Name);
			lvi.SubItems.Add(pc.User?.Title);
			lvi.SubItems.Add(pc.Cpu?.Score.ToString());
			lvi.SubItems.Add(pc.Fluency.ToString("F1"));
			lvi.SubItems.Add(pc.Model);
			lvi.SubItems.Add(pc.Enclosure);
			lvi.SubItems.Add(pc.Cpu?.Model);
			lvi.SubItems.Add(pc.Memory?.ToString());
			lvi.SubItems.Add(pc.Disk?.ToString());
			lvi.SubItems.Add(pc.Monitor?.Size.ToString("F1") + " inç" ?? "");
			lvi.SubItems.Add(pc.VideoCard?.Name);

			lock (lv_pcl)
			{
				lv_pcl.Items.Add(lvi);
			}
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
	}
}
