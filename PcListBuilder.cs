using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.Globalization;
using System.IO;
using System.Reflection.PortableExecutable;

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

		int totalPc = 0;

		public PcListBuilder()
		{
			InitializeComponent();
		}

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
			listeEksikleriniTamamlaToolStripMenuItem.Enabled = false;

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

			totalPc = pcNamesList.Count;
			progressBar.Step = 100 / Math.Max(1, pcNamesList.Count);
            System.Threading.Timer tt = new System.Threading.Timer((sender) => { 
				
			});

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

						if (pcl.pcl.Count == pcNamesList.Count)
						{
							listeEksikleriniTamamlaToolStripMenuItem.Enabled = true;
						}
						cpc.Enqueue(pc);

						PC? pcx;
						while (cpc.TryDequeue(out pcx))
						{
							addToListViewCache(pcx);
							Invoke(() => { progressBar.Value += 1000000 / Math.Max(1, pcNamesList.Count); });
						}
					}
					catch
					{
					}
				}).Start();
			}
		}

		private void Bw_DoWork(object? sender, DoWorkEventArgs e)
		{
			sbMessage.Text = "Hazırlanıyor...";
			progressBar.Value = 0;
			progressBar.Enabled = true;

			List<string> prefixList = (List<string>)e.Argument!;
			List<string> pcnameslist = GetPCList(prefixList!);
			totalPc = pcnameslist.Count;
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
			}
			catch (Exception)
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
			sbMessage.Text = "Kaydediliyor...";
			progressBar.Value = 0;
			progressBar.Enabled = false;

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
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += Bw_DoWork;
            bw.RunWorkerCompleted += RefreshBw_RunWorkerCompleted;
            bw.RunWorkerAsync(pcl.PrefixList);
        }

        private void RefreshBw_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result == null)
                return;
            List<string> pcNamesList = (List<string>)e.Result;
            if (pcNamesList.Count == 0)
                return;

            Cache.Clear();
            lv_pcl.VirtualListSize = 0;
            lv_pcl.Items.Clear();
            listeEksikleriniTamamlaToolStripMenuItem.Enabled = false;
            sbMessage.Text = "Hazırlanıyor...";
            progressBar.Enabled = true;
            progressBar.Value = 0;

            List<string> pcn = new List<string>();
            int pcc = pcl.pcl.Count;

            foreach (string pcx in pcNamesList)
            {
				PC? p = pcl.pcl.FirstOrDefault(x => x.Name == pcx);
				if(p == null)
                    pcn.Add(pcx);

				if(p.IP == "0.0.0.0")
					pcn.Add(pcx);
            }

			foreach (string pcname in pcn)
				pcl.pcl.RemoveAll(px => px.Name == pcname);

			foreach (PC pc in pcl.pcl)
				cpc.Enqueue(pc);

			progressBar.Value = (1000000 / pcc) * cpc.Count;

			foreach (string pcname in pcn)
			{
				new Thread(() =>
				{
					try
					{
						PC pc = new PC() { Name = pcname ?? "PC71" };
						pc.RetrieveInfo();

						lock (pcl)
							pcl.pcl.Add(pc);

						if (pcl.pcl.Count == pcc)
						{
							listeEksikleriniTamamlaToolStripMenuItem.Enabled = true;
						}

						cpc.Enqueue(pc);

						PC? pcx;
						while (cpc.TryDequeue(out pcx))
						{
							addToListViewCache(pcx);
							Invoke(() => { long v = 1000000 / pcc; if (progressBar.Value + v <= 1000000) progressBar.Value += 1000000 / pcc; });
						}
					}
					catch (Exception)
					{

					}
				}).Start();
			}

		}

        private void excelOlarakDışaAktarToolStripMenuItem_Click(object sender, EventArgs e)
        {
			if (pcl.pcl.Count == 0)
				return;

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel Dosyası | *.xlsx";
            sfd.DefaultExt = "xlsx";

            if (sfd.ShowDialog() != DialogResult.OK)
                return;
			try
			{
                IWorkbook wb = new XSSFWorkbook();
                ISheet sheet = wb.CreateSheet("PC Liste");


                //HEADERS
                IRow headerRow = sheet.CreateRow(0);
                XSSFFont defaultFont = (XSSFFont)wb.CreateFont();
                defaultFont.FontHeightInPoints = (short)11;
                defaultFont.Color = IndexedColors.Black.Index;
                defaultFont.IsBold = true;

                XSSFCellStyle yourCellStyle = (XSSFCellStyle)wb.CreateCellStyle();
                yourCellStyle.SetFont(defaultFont);
                headerRow.RowStyle = yourCellStyle;

                for (int i = 0; i < lv_pcl.Columns.Count; i++)
				{
                    ICell headerCell = headerRow.CreateCell(i);

                    headerCell.SetCellValue(lv_pcl.Columns[i].Text.ToUpper(CultureInfo.GetCultureInfo("tr-TR")));
                    sheet.AutoSizeColumn(i);
                }

				//DATsA
				for (int i = 0; i < Cache.Count; i++)
				{
					ListViewItem lvi = Cache.ElementAt(i);
					IRow row = sheet.CreateRow(i+1);
					for (int x = 0; x < lvi.SubItems.Count; x++)
					{
                        ICell cell = row.CreateCell(x);
						cell.SetCellValue(lvi.SubItems[x].Text);
                        sheet.AutoSizeColumn(x);
                    }
                }
				wb.Write(new FileStream(sfd.FileName, FileMode.Create));
				wb.Close();
				MessageBox.Show("Kaydedildi.", "İşlem Başarılı");
            }
			catch (Exception)
			{

			}


        }

    }
}
