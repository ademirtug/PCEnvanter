using System.Management;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace PCEnvanter
{
	public partial class PCEnvanter : Form
	{
		ConcurrentBag<string> pcnamelist = new ConcurrentBag<string>();
		ConcurrentBag<PC> pclist = new ConcurrentBag<PC>();
		public static List<CPU> cpuList = new List<CPU>();

#if DEBUG
		string datafile = "../../../data/data.txt";
#else
		string datafile = "./data/data.txt"
#endif

		public PCEnvanter()
		{
			InitializeComponent();
			List<CPU> cpulist = new List<CPU>();

			string[] lines = File.ReadAllLines(datafile);
			foreach (string line in lines)
			{
				string[] sp = line.Split("\t");
				cpulist.Add(new CPU() { Name = sp[0], Score = Convert.ToDouble(sp[1]) });

			}

			PC pc = getPCInfo("AKN-PC");

			List<string> prefixes = new List<string>() { "P71", "A71", "L71" };
			//GetPCList(prefixes);

		}

		private void GetPCList(List<string> prefixes)
		{
			PrincipalContext context = getPrincipalContext();

			pcnamelist = new ConcurrentBag<string>();

			foreach (string str in prefixes)
			{
				ComputerPrincipal queryFilter = new ComputerPrincipal(context);
				queryFilter.Name = str + "*";
				foreach (Principal principal in new PrincipalSearcher((Principal)queryFilter).FindAll())
				{
					if (principal is ComputerPrincipal computerPrincipal)
						pcnamelist.Add(computerPrincipal.Name);
				}
			}


			//foreach (string str in stringList)
			//{
			//	BackgroundWorker backgroundWorker = new BackgroundWorker();
			//	backgroundWorker.DoWork += new DoWorkEventHandler(this.Worker_DoWork);
			//	backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.Worker_RunWorkerCompleted);
			//	backgroundWorker.RunWorkerAsync((object)str);
			//}


			//Parallel.ForEach(pcnamelist, pname =>
			//{
			//	PC pc = getPCInfo(pname);
			//});
			PC pc = getPCInfo("AKN-PC");

		}


		private PC getPCInfo(string pcname)
        {
			PC pc = new PC() { Name = pcname };
			var pt = getPCType(pc.Name);
			pc.Model = pt[0];
			pc.Manufacturer = pt[1];
			pc.VideoCard = getVideoCard(pc.Name);
			pc.User = getPCUser(pc.Name);
			pc.IP = getPCIP(pc.Name);
			pc.Enclosure = getPCEnclosure(pc.Name);
			pc.Cpu = getCPU(pc.Name);
			pc.Disk = getDisk(pc.Name);
			pc.Memory = getMemory(pc.Name);
			pc.Wei = getWei(pc.Name);
			pc.Monitor = getMonitor(pc.Name);
			pclist.Add(pc);
			Debug.WriteLine(pc.Name + " Completed.");
			return pc;
		}

		private string[] getPCType(string pcname)
		{
			string[] r = new string[] {"", ""};

			try
			{
				foreach (ManagementObject m in Query(pcname, "SELECT Manufacturer, Model FROM Win32_ComputerSystem") )
				{
					r[0] = m["Model"]?.ToString() ?? "";
					r[1] = m["Manufacturer"]?.ToString() ?? "";
				}
			}
			catch (Exception ex)
			{    
			}
			return r;

		}
		private VideoCard getVideoCard(string pcname)
		{
			VideoCard vc = new VideoCard();
			try
			{
				foreach (ManagementObject m in Query(pcname, "SELECT * FROM Win32_VideoController"))
				{
					vc.Name = m["Name"]?.ToString() ?? "";
				}
			}
			catch (Exception ex)
			{
			}
			return vc;

		}

		private Personnel? getPCUser(string pcname)
		{
			Personnel p = new Personnel();
			try
			{
				foreach (ManagementObject m in Query(pcname, "SELECT Name, UserName FROM Win32_ComputerSystem") )
				{
					p.Name = m["Name"]?.ToString() ?? "";
					p.UserName = m["UserName"]?.ToString()?.Replace("CSB\\", "") ?? "";

					if (p.UserName.Length == 0)
						return null;

					using (PrincipalContext context = getPrincipalContext())
					{
						using (PrincipalSearcher principalSearcher = new PrincipalSearcher((Principal)new UserPrincipal(context) { SamAccountName = p.UserName }))
						{
							object ode = principalSearcher.FindOne()?.GetUnderlyingObject() ?? null;
							if (ode == null)
								return null;

							DirectoryEntry? de = ode as DirectoryEntry;

							p.Name = de.Properties["GivenName"]?.Value?.ToString() + " " + de.Properties["sn"]?.Value?.ToString();
							p.Title = de.Properties["title"]?.Value?.ToString() ?? "";
							de.Dispose();
						}
					}
				}
			}
			catch (Exception ex)
			{
				return null;
			}
			return p;

		}
		private string getPCIP(string pcname)
		{
			try
			{
				foreach (ManagementObject m in Query(pcname, "SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = 'TRUE'"))
				{
					return m["IPAddress"] == null ? "0.0.0.0" : ((string[])m["IPAddress"])[0];
				}
			}
			catch (Exception ex)
			{
			}
			return "0.0.0.0";

		}
		private string getPCEnclosure(string pcname)
		{
			try
			{
				foreach (ManagementObject m in Query(pcname, "SELECT ChassisTypes FROM Win32_SystemEnclosure"))
				{
					short[] numArray = m["ChassisTypes"] != null ? (short[])m["ChassisTypes"] : new short[1];
					short num = numArray.Length != 0 ? numArray[0] : (short)3;
					switch (num)
					{
						case 3:
						case 4:
						case 6:
						case 7:
							return "Masaüstü PC";
						case 8:
						case 9:
						case 10:
							return "Dizüstü Bilgisayar";
						case 13:
							return "Bütünleşik PC";    
						default:
							return num.ToString();
					}
				}
			}
			catch (Exception ex)
			{
			}
			return "-";

		}
		
		private CPU getCPU(string pcname)
		{
			CPU c = new CPU();
			try
			{
				foreach (ManagementObject m in Query(pcname, "SELECT Name FROM Win32_Processor"))
				{
					//List<string> lst = new List<string>();
					//foreach (var item in m.Properties)
					//{
					//    lst.Add(item.Name + ": " + item.Value?.ToString());
					//}
					c.Name = m["Name"]?.ToString() ?? "";
				}
			}
			catch (Exception ex)
			{
			}
			return c;

		}

		private Monitor getMonitor(string pcname)
		{
			Monitor mo = new Monitor();

			try
			{
				foreach (ManagementObject m in Query(pcname, "SELECT PNPDeviceID FROM CIM_DesktopMonitor"))
				{
					if (m["PNPDeviceID"] == null)
						continue;

					var parts = m["PNPDeviceID"].ToString().Split("\\");
					if (parts.Length > 2)
                    {
						mo.ID = parts[1];
						break;
					}
						
				}
			}
			catch (Exception ex)
			{
			}
			mo.Size = GetMonitorSize(pcname);


			return mo;

		}
		public double GetMonitorSize(string pcname)
		{
			try
			{
				double w = 0;
				double h = 0;

				foreach (ManagementObject m in Query(pcname, "SELECT * FROM WmiMonitorBasicDisplayParams", isMonitor: true))
				{
					w = Convert.ToDouble(m["MaxHorizontalImageSize"].ToString());
					h = Convert.ToDouble(m["MaxVerticalImageSize"].ToString());
				}
				return Math.Sqrt((w * w) + (h * h)) / 2.54;
			}
            catch (Exception)
            {

            }
			return 0;
		}
		private Disk getDisk(string pcname)
		{
			Disk d = new Disk();
			try
			{
				foreach (ManagementObject m in Query(pcname, "SELECT * FROM MSFT_PhysicalDisk", true))
				{
					d.MediaType = m["MediaType"] != null ? m["MediaType"].ToString() : "0";
					d.Capacity += m["Size"] != null ? Convert.ToUInt64(m["Size"].ToString()) : 1UL;
				}
			}
			catch (Exception ex)
			{
			}
			return d;

		}

		private Memory getMemory(string pcname)
		{
			Memory mm = new Memory();

			try
			{
				foreach (ManagementObject m in Query(pcname, "SELECT * FROM Win32_PhysicalMemory"))
				{
					mm.Capacity += Convert.ToUInt64(m["Capacity"]?.ToString() ?? "0");
					mm.Frequency = Convert.ToInt32(m["Speed"]?.ToString() ?? "0");
					mm.MemoryTypeData = m["MemoryType"]?.ToString() ?? "";
				}
			}
			catch (Exception)
			{

			}
			return mm;
		}
		private Wei getWei(string pcname)
		{
			Wei w = new Wei();

			try
			{
				foreach (ManagementObject m in Query(pcname, "SELECT * FROM Win32_WinSAT"))
				{
					w.Cpu = Convert.ToDouble(m["CPUScore"]?.ToString() ?? "0");
					w.Disk = Convert.ToDouble(m["DiskScore"]?.ToString() ?? "0");
					w.Graphics = Convert.ToDouble(m["GraphicsScore"]?.ToString() ?? "0");
					w.Memory = Convert.ToDouble(m["MemoryScore"]?.ToString() ?? "0");
				}
			}
			catch (Exception ex)
			{

			}
			return w;
		}


		private PrincipalContext getPrincipalContext()
		{
#if DEBUG
			return new PrincipalContext(ContextType.Domain, "csb.local", "akin.demirtug", "Sandalye22");
#else
			return new PrincipalContext(ContextType.Domain, "csb.local");
#endif
		}

		private ManagementObjectCollection Query(string pcname, string q, bool isDisk = false, bool isMonitor = false)
		{
			string scope = "";

			if (isDisk)
				scope = "\\\\" + pcname + "\\root\\microsoft\\windows\\storage";
			else if (isMonitor)
				scope = "\\\\" + pcname + "\\root\\wmi";
			else
				scope = "\\\\" + pcname + "\\root\\CIMV2";

			ManagementScope? ms = null;

			if (Environment.MachineName != pcname)
			{
#if DEBUG
				ConnectionOptions co = new ConnectionOptions();
				co.Username = "csb\\akin.demirtug";
                co.Password = "Sandalye22";
				ms = new ManagementScope(scope, co);		
#endif
			}
			
			if(ms == null)
				ms = new ManagementScope(scope);

			ObjectQuery oq = new ObjectQuery(q);
			ManagementObjectSearcher searcher = new ManagementObjectSearcher(ms, oq);
			ManagementObjectCollection queryCollection = searcher.Get();
			return queryCollection;
		}
	}
}