using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.Globalization;
using System.Management;
using System.Text;
using System.DirectoryServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System;

namespace PCEnvanter
{

	public class PcList
	{
		public DateTime TimeStamp { get; set; } = DateTime.Now;
		public List<string> PrefixList { get; set; } = new List<string>();
		public List<PC> pcl { get; set; } = new List<PC>();

		public void FlushAsJson(string path = "", bool sort = true)
		{
			path = path == "" ? Main.dpath + "pc_liste.json" : path;

			JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
			string json = JsonSerializer.Serialize<PcList>(this, options);

			if (!Directory.Exists(Path.GetDirectoryName(path)))
				Directory.CreateDirectory(Path.GetDirectoryName(path) ?? "");
			File.WriteAllText(path, json);
		}

		public static PcList LoadFromFile(string path = "")
		{
			path = path == "" ? Main.dpath + "pc_liste.json" : path;
			if (File.Exists(path))
				return JsonSerializer.Deserialize<PcList>(File.ReadAllText(path)) ?? new PcList();
			return new PcList();
		}
	}
	public class PC
	{
		public string Name { get; set; } = "";

		public Personnel? User { get; set; }

		public string IP { get; set; } = "";

		public string Model { get; set; } = "";

		public string Manufacturer { get; set; } = "";

		public CPU? Cpu { get; set; }

		public Disk? Disk { get; set; }

		public Memory? Memory { get; set; }

		public Monitor? Monitor { get; set; }

		public VideoCard? VideoCard { get; set; } 

		public string Enclosure { get; set; } = "";
		public double Fluency
		{
			get
			{
				double cpuRating = Cpu?.Score.Map(1000, 18000, 0, 3) ?? 1.0;
				double memoryRating = Memory?.Capacity.Map((ulong)Math.Pow(2, 30) * 3, (ulong)Math.Pow(2, 30) * 16, 0, 2) ?? 1;
				double diskRating = Disk?.Score.Map(300, 16000, 0, 5) ?? 0;

				return Math.Max(cpuRating+memoryRating+diskRating, 0.1);
			}
		}

		public bool RetrieveInfo()
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();

			IP = getPCIP();
			if (IP == "0.0.0.0")
				return false;

			var pt = getPCType();
			Model = pt[0];
			Manufacturer = pt[1];
			VideoCard = getVideoCard();
			User = getPCUser();
			Enclosure = getPCEnclosure();
			retrieveCPUInfo();
			Disk = getDisk();
			Memory = getMemory();
			Monitor = getMonitor();
			sw.Stop();
			return true;
		}

		private string[] getPCType()
		{
			string[] r = new string[] { "", "" };

			try
			{
				foreach (ManagementObject m in Query(Name, "SELECT Manufacturer, Model FROM Win32_ComputerSystem"))
				{
					r[0] = m["Model"]?.ToString() ?? "";
					r[1] = m["Manufacturer"]?.ToString() ?? "";
				}
			}
			catch (Exception ex)
			{
				Main.log.Enqueue($"{DateTime.Now} - Hatâ: {Name}::getPCType() - " + ex.ToString());
			}
			return r;

		}
		private VideoCard getVideoCard()
		{
			VideoCard vc = new VideoCard();
			try
			{
				foreach (ManagementObject m in Query(Name, "SELECT * FROM Win32_VideoController"))
				{
					vc.Name = m["Name"]?.ToString() ?? "";
				}
			}
			catch (Exception ex)
			{
                Main.log.Enqueue($"{DateTime.Now} - Hatâ: {Name}::getVideoCard() - " + ex.ToString());
            }
			return vc;

		}

		private Personnel? getPCUser()
		{
			Personnel p = new Personnel();
			try
			{
				foreach (ManagementObject m in Query(Name, "SELECT Name, UserName FROM Win32_ComputerSystem"))
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
                Main.log.Enqueue($"{DateTime.Now} - Hatâ: {Name}::getPCUser() - " + ex.ToString());

                return null;
			}
			return p;

		}
		private PrincipalContext getPrincipalContext()
		{
#if DEBUG
			return new PrincipalContext(ContextType.Domain, "csb.local", "akin.demirtug", "Sandalye22");
#else
			return new PrincipalContext(ContextType.Domain, "csb.local");
#endif
		}

		private string getPCIP()
		{
			try
			{
				foreach (ManagementObject m in Query(Name, "SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = 'TRUE'"))
				{
					return m["IPAddress"] == null ? "0.0.0.0" : ((string[])m["IPAddress"])[0];
				}
			}
			catch (Exception ex)
			{
               // Main.log.Enqueue($"{DateTime.Now} - Hatâ: {Name}::getPCIP() - " + ex.ToString());
            }
			return "0.0.0.0";

		}
		private string getPCEnclosure()
		{
			try
			{
				foreach (ManagementObject m in Query(Name, "SELECT ChassisTypes FROM Win32_SystemEnclosure"))
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
                Main.log.Enqueue($"{DateTime.Now} - Hatâ: {Name}::getPCEnclosure() - " + ex.ToString());
            }
			return "-";

		}

		private void retrieveCPUInfo()
		{
			try
			{
				Cpu = new CPU();
				foreach (ManagementObject m in Query(Name, "SELECT Name FROM Win32_Processor"))
				{

					Cpu.Name = m["Name"]?.ToString() ?? "";
				}
			}
			catch (Exception ex)
			{
                Main.log.Enqueue($"{DateTime.Now} - Hatâ: {Name}::retrieveCPUInfo() - " + ex.ToString());
                Cpu = null;
			}
		}

		private Monitor getMonitor()
		{
			Monitor mo = new Monitor();

			try
			{
				foreach (ManagementObject m in Query(Name, "SELECT PNPDeviceID FROM CIM_DesktopMonitor"))
				{
					if (m["PNPDeviceID"] == null)
						continue;

					var parts = m["PNPDeviceID"]?.ToString()?.Split("\\");
					if (parts?.Length > 2)
					{
						mo.ID = parts[1];
						break;
					}

				}
			}
			catch (Exception ex)
			{
                Main.log.Enqueue($"{DateTime.Now} - Hatâ: {Name}::getMonitor() - " + ex.ToString());
            }
			mo.Size = GetMonitorSize();


			return mo;

		}
		public double GetMonitorSize()
		{
			try
			{
				double w = 0;
				double h = 0;

				foreach (ManagementObject m in Query(Name, "SELECT * FROM WmiMonitorBasicDisplayParams", isMonitor: true))
				{
					w = Convert.ToDouble(m["MaxHorizontalImageSize"].ToString());
					h = Convert.ToDouble(m["MaxVerticalImageSize"].ToString());
				}
				return Math.Sqrt((w * w) + (h * h)) / 2.54;
			}
			catch (Exception ex)
			{
                Main.log.Enqueue($"{DateTime.Now} - Hatâ: {Name}::GetMonitorSize() - " + ex.ToString());
            }
			return 0;
		}
		private Disk getDisk()
		{
			Disk d = new Disk();
			try
			{
                List<string> list = new List<string>();
                foreach (ManagementObject m in Query(Name, "SELECT * FROM MSFT_PhysicalDisk", true))
				{

                    d.MediaType = m["MediaType"]?.ToUTF8() ?? "0";
					d.BusType = m["BusType"]?.ToString() ?? "0";
                    
					//usb türlerini geç
                    if (d.BusType == "7" || d.BusType == "12" || d.BusType == "13")
						continue;


                    foreach (var y in m.Properties)
					{
						list.Add(y.Name + "=" + y.Value);
					}
                    d.Model = m["Model"]?.ToUTF8() ?? "";
					d.FriendlyName = m["FriendlyName"]?.ToUTF8() ?? "";
					d.Capacity += Convert.ToUInt64(m["Size"]?.ToString() ?? "0");
					break;


				}
			}
			catch (Exception ex )
			{
                Main.log.Enqueue($"{DateTime.Now} - Hatâ: {Name}::getDisk() - " + ex.ToString());
            }
			return d;

		}

		private Memory getMemory()
		{
			Memory mm = new Memory();

			try
			{
				foreach (ManagementObject m in Query(Name, "SELECT * FROM Win32_PhysicalMemory"))
				{
					mm.Capacity += Convert.ToInt64 (m["Capacity"]?.ToString() ?? "0");
					mm.Frequency = Convert.ToInt32(m["Speed"]?.ToString() ?? "0");
					mm.MemoryTypeData = m["MemoryType"]?.ToString() ?? "";
				}
			}
			catch (Exception ex)
			{
                Main.log.Enqueue($"{DateTime.Now} - Hatâ: {Name}::getMemory() - " + ex.ToString());

            }
			return mm;
		}

		private ManagementObjectCollection Query(string Name, string q, bool isDisk = false, bool isMonitor = false)
		{
			string scope = "";

			if (isDisk)
				scope = "\\\\" + Name + "\\root\\microsoft\\windows\\storage";
			else if (isMonitor)
				scope = "\\\\" + Name + "\\root\\wmi";
			else
				scope = "\\\\" + Name + "\\root\\CIMV2";

			ManagementScope? ms = null;

			if (Environment.MachineName != Name)
			{
#if DEBUG
				ConnectionOptions co = new ConnectionOptions();
				co.Username = "csb\\akin.demirtug";
				co.Password = "Sandalye22";
				ms = new ManagementScope(scope, co);
#endif
			}

			if (ms == null)
				ms = new ManagementScope(scope);

			ObjectQuery oq = new ObjectQuery(q);
			ManagementObjectSearcher searcher = new ManagementObjectSearcher(ms, oq);
			ManagementObjectCollection queryCollection = searcher.Get();
			return queryCollection;
		}

		public ListViewItem GetLVI(string pclc)
		{
            ListViewItem lvi = new ListViewItem(pclc);
            lvi.SubItems.Add(Name);
            lvi.SubItems.Add(IP == "0.0.0.0" ? "" : IP);
            lvi.SubItems.Add(User?.Name);
            lvi.SubItems.Add(User?.Title);
            lvi.SubItems.Add(Cpu?.Score.ToString());
            lvi.SubItems.Add(Disk?.Score.ToString());
            lvi.SubItems.Add(IP == "0.0.0.0" ? "" : Fluency.ToString("F1"));
            lvi.SubItems.Add(Model);
            lvi.SubItems.Add(Enclosure);
            lvi.SubItems.Add(Cpu?.Model);
            lvi.SubItems.Add(Memory?.ToString());
            lvi.SubItems.Add(Disk?.ToString());
            lvi.SubItems.Add(Monitor?.Size.ToString("F1").Length > 0 ? Monitor?.Size.ToString("F1") + " inç" : "");
            lvi.SubItems.Add(VideoCard?.Name);

			return lvi;
        }
	}

	public class Brand
	{
		public string Manufacturer { get; set; } = "";

		public string Model { get; set; } = "";

		public string Enclosure { get; set; } = "";
	}

	public class CPU
	{
		double _score = 0;

		public CPU(){}
		public CPU(string raw){}

        [JsonIgnore]
        public double Score
		{
			get
			{
				if (_score > 0)
					return _score;

				foreach (var cpu in Main.cpuList)
				{
					if (cpu.Model.Contains(Model))
					{
						_score = cpu.Score;
						return cpu.Score;
					}
				}
				return 0;

			}
			set
			{
				_score = value;
			}
		}

		public string Manufacturer 
		{
			get 
			{
				if (Name == "")
					return "";

				if (Name.ToUpper().Contains("INTEL"))
					return "Intel";
				else if (Name.ToUpper().Contains("AMD"))
					return "AMD";

				return "";
			}

		}

		public string Model
		{
			get
			{
				if (Name == "")
					return "";

				try
				{
					string safeName = Name + " ";
					string[] sp = safeName.Split(" ");
					string[] intel = { "i3", "i5", "i7", "i9" };
					foreach (string processor in intel)
					{
						int index = safeName.IndexOf(processor);
						if (index > -1)
							return clearName(safeName.Substring(index, safeName.IndexOf(" ", index) - index));
					}
						
					for (int i = 0; i < sp.Length; i++)
						if (sp[i].ToUpper().Contains("AMD"))
                            return clearName($"{sp[i + 1]} {sp[i + 2]}  {(i + 3 < sp.Length ? sp[i + 3] : "")}");

				}
				catch (Exception ex)
				{
                    Main.log.Enqueue($"{DateTime.Now} - Hatâ: {Name}::Model - " + ex.ToString());

                }
				return clearName(Name);
			}

		}

		public string Name { get; set; } = "";

		string clearName(string n)
        {
			if(n.IndexOf("@") > -1)
			{
				n = n.Substring(0, n.IndexOf("@"));
			}
            if (n.IndexOf("CPU") > -1)
            {
                n = n.Substring(0, n.IndexOf("CPU"));
            }

            n = n.Replace("(tm)", "").Replace("Core(TM)", "").Replace("Intel(R)", "").Replace("Processor", "");
			n = n.Replace("  ", " ").Replace("   ", " ").Trim();

			return n;
        }
	}

	public class Memory
	{
		public double Capacity { get; set; } = 0;

		public int Frequency { get; set; } = 0;

		public string MemoryTypeData { get; set; } = "";

		public string MemoryType
		{
			get
			{
				if (MemoryTypeData == "11")
					return "Flash";
				else if (MemoryTypeData == "17")
					return "SDRAM";
				else if (MemoryTypeData == "20")
					return "DDR";
				else if (MemoryTypeData == "21" || MemoryTypeData == "22" || MemoryTypeData == "23")
					return "DDR2";
				else if (MemoryTypeData == "24")
					return "DDR3";
				else if (MemoryTypeData == "26")
					return "DDR4";
				else if (MemoryTypeData == "0")
					return Convert.ToInt32(Frequency) < 2133 || Convert.ToInt32(Frequency) > 5000 ? (Convert.ToInt32(Frequency) < 1066 || Convert.ToInt32(Frequency) >= 2133 ? (Convert.ToInt32(Frequency) >= 1066 ? "???" : "DDR2") : "DDR3") : "DDR4";
				return "";
			}
		}


		public override string ToString()
		{
			return ((Capacity / (1024 * 1024 * 1024))).ToString("F1") + " GB "+ MemoryType;
		}
	}

	public class Disk
	{
		double _score = 0;
		string _model = "";

		public string Model
		{
			get { return _model; }
			set { _model = value.Replace("-", " ").Trim().ToUpper(); }
		}
		public string FriendlyName { get; set; } = "";
		public ulong Capacity { get; set; } = 0;

		public DiskCapacityTier CapacityTier
		{
			get
			{
				if(Capacity > 200 * Math.Pow(2, 30) && Capacity < 290 * Math.Pow(2, 30))
				{
					return DiskCapacityTier.d256;
				}
				else if (Capacity > 290 * Math.Pow(2, 30) && Capacity < 350 * Math.Pow(2, 30))
                {
                    return DiskCapacityTier.d320;
                }
				else if (Capacity > 450 * Math.Pow(2, 30) && Capacity < 550 * Math.Pow(2, 30))
                {
                    return DiskCapacityTier.d500;
                }
				else if (Capacity > 800 * Math.Pow(2, 30) && Capacity < 1100 * Math.Pow(2, 30))
                {
                    return DiskCapacityTier.d1000;
                }
				else if (Capacity > 1100 * Math.Pow(2, 30) && Capacity < 2100 * Math.Pow(2, 30))
                {
                    return DiskCapacityTier.d2000;
                }

                return DiskCapacityTier.d256;
			}
		}
		public string scap { get; set; } = "";
		public string MediaType { get; set; } = "";
		public string BusType { get; set; } = "";

		[JsonIgnore]
		public double Score
		{
			get
            {
				if (_score > 0)
                    return _score;



				foreach (Disk d in Main.diskList)
				{
					if(d.Model.Contains(Model) && d.CapacityTier == CapacityTier)
					{
						_score = d.Score;
						return _score;
					}
				}


				string mm = Model.LastIndexOf(" ") > -1 ? Model.Remove(Model.LastIndexOf(" "), Model.Length - Model.LastIndexOf(" ")) : Model;

                foreach (Disk d in Main.diskList)
                {
                    if (d.Model.Contains(mm) && d.CapacityTier == CapacityTier)
                    {
                        _score = d.Score;
                        return _score;
                    }
                }
				_score = MediaType == "3" || MediaType == "0" ? 600 : 8500;
                return _score;
            }
            set
            {
                _score = value;
            }
        }

        public override string ToString()
		{
			if (Capacity == 0)
				return "";

			string t = "";
			if (MediaType == "3" || MediaType == "0")
				t = "HDD";
			else if (MediaType == "4")
				t = "SSD";

			return Capacity > 0 ? Capacity / 1000000000 + " GB " + t : t;
		}

		public enum DiskCapacityTier { d256, d320, d500, d1000, d2000 };

	}

	public class Personnel
	{
		public string Name { get; set; } = "";

		public string UserName { get; set; } = "";

		[JsonIgnore]
		public string UniName
		{
			get
			{
				return Name.ToUpper().RemoveDiacritics();
			}
		}

		public string Title { get; set; } = "";

	}

	public class Monitor
	{
		public string ID { get; set; } = "";

		public double Size { get; set; } = 0;
	}

	public class VideoCard
	{
		public string Name { get; set; } = "";
	}

	public static class MyExtensions
	{
		public static string RemoveDiacritics(this string text)
		{
			var normalizedString = text.Normalize(NormalizationForm.FormD);
			var stringBuilder = new StringBuilder();

			foreach (var c in normalizedString.EnumerateRunes())
			{
				var unicodeCategory = Rune.GetUnicodeCategory(c);
				if (unicodeCategory != UnicodeCategory.NonSpacingMark)
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
		}

		public static double Map(this double value, double fromSource, double toSource, double fromTarget, double toTarget)
		{
			//value = Clamp(value, fromSource, toSource);
			value = Math.Min(value, toSource);
			return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
		}

        //public static double Map(this long value, long fromSource, long toSource, long fromTarget, long toTarget)
        //{
        //    //value = Clamp(value, fromSource, toSource);
        //    value = Math.Min(value, toSource);
        //    return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
        //}


        //CLAMP
        public static int Clamp(int value, int min, int max)
		{
			return (value < min) ? min : (value > max) ? max : value;
		}
		public static double Clamp(double value, double min, double max)
		{
			return (value < min) ? min : (value > max) ? max : value;
		}
		public static ulong Clamp(ulong value, ulong min, ulong max)
		{
			return (value < min) ? min : (value > max) ? max : value;
		}
		public static long Clamp(long value, long min, long max)
		{
			return (value < min) ? min : (value > max) ? max : value;
		}

        public static string ToUTF8(this object value)
        {
            byte[] bytes = Encoding.Default.GetBytes(value.ToString());
            string ux = Encoding.UTF8.GetString(bytes);

			return ux;
            //return (value < min) ? min : (value > max) ? max : value;
        }

	}
}
