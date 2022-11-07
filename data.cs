using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.Globalization;
using System.Management;
using System.Text;
using System.DirectoryServices;
using System.Text.Json;
using System.Collections.Concurrent;

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

			//C:\Users\akn\Desktop\data
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

		public Wei? Wei { get; set; }

		public VideoCard? VideoCard { get; set; } 

		public string Enclosure { get; set; } = "";
		public double Fluency
		{
			get
			{
				if (Wei?.Disk == 0)
					return 0;

				double cpuRating = Cpu?.Score.Map(3000, 12000, 0, 3) ?? 1.0;
				double memoryRating = Memory?.Capacity.Map((ulong)Math.Pow(2, 30) * 5, (ulong)Math.Pow(2, 30) * 10, 0, 2) ?? 1;
				double diskRating = Wei?.Disk / 2 ?? 0;

				return Math.Max(cpuRating+memoryRating+diskRating, 2.0);
			}
		}

		public bool RetrieveInfo()
		{
			IP = getPCIP();
			if (IP == "0.0.0.0")
				return false;

			var pt = getPCType();
			Model = pt[0];
			Manufacturer = pt[1];
			VideoCard = getVideoCard();
			User = getPCUser();
			Enclosure = getPCEnclosure();
			Cpu = getCPU();
			Disk = getDisk();
			Memory = getMemory();
			Wei = getWei();
			Monitor = getMonitor();
			Debug.WriteLine(Name + " Completed.");
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
			}
			return "-";

		}

		private CPU getCPU()
		{
			CPU c = new CPU();
			try
			{
				foreach (ManagementObject m in Query(Name, "SELECT Name FROM Win32_Processor"))
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

		private Monitor getMonitor()
		{
			Monitor mo = new Monitor();

			try
			{
				foreach (ManagementObject m in Query(Name, "SELECT PNPDeviceID FROM CIM_DesktopMonitor"))
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
			catch (Exception)
			{

			}
			return 0;
		}
		private Disk getDisk()
		{
			Disk d = new Disk();
			try
			{
				foreach (ManagementObject m in Query(Name, "SELECT * FROM MSFT_PhysicalDisk", true))
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

		private Memory getMemory()
		{
			Memory mm = new Memory();

			try
			{
				foreach (ManagementObject m in Query(Name, "SELECT * FROM Win32_PhysicalMemory"))
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
		private Wei getWei()
		{
			Wei w = new Wei();

			try
			{
				foreach (ManagementObject m in Query(Name, "SELECT * FROM Win32_WinSAT"))
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

		public double Score
		{
			get
			{
				if (_score > 0)
					return _score;

				foreach (var cpu in Main.cpuList)
				{
					if (cpu.Model == Model)
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
					string[] sp = Name.Split(" ");

					string[] intel = { "i3", "i5", "i7", "i9" };
					foreach (string processor in intel)
					{
						int index = Name.IndexOf(processor);
						if (index > -1)
							return Name.Substring(index, Name.IndexOf(" ", index) - index).Trim();
					}

					for (int i = 0; i < sp.Length; i++)
					{
						if (sp[i].ToUpper().Contains("AMD"))
						{
							return $"{sp[i + 1]} {sp[i + 2]} {sp[i + 3]}";
						}
					}
				}
				catch (Exception)
				{

				}
				return Name;
			}

		}

		public string Name { get; set; } = "";
	}

	public class Memory
	{
		public ulong Capacity { get; set; } = 0;

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
				else if (MemoryTypeData == "21")
					return "DDR2";
				else if (MemoryTypeData == "22")
					return "DDR2";
				else if (MemoryTypeData == "23")
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
			return (Capacity / (1024 * 1024 * 1024)) + " GB "+ MemoryType;
		}
	}

	public class Wei
	{
		public double Cpu { get; set; } = 0;
		public double Graphics { get; set; } = 0;
		public double Disk { get; set; } = 0;
		public double Memory { get; set; } = 0;
		public double Overall 
		{ 
			get 
			{
				return 0;
			} 
		}

	}

	public class Disk
	{ 
		public ulong Capacity { get; set; } = 0;
		public string MediaType { get; set; } = "";
		public override string ToString()
		{
			string t = "";
			if (MediaType == "3" || MediaType == "0")
				t = "HDD";
			else if (MediaType == "4")
				t = "SSD";

			return Capacity > 0 ? Capacity / 1000000000 + " GB " + t : t;
		}
	}

	public class Personnel
	{
		public string Name { get; set; } = "";

		public string UserName { get; set; } = "";

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

		//MAP
		public static double Map(this int value, int fromSource, int toSource, int fromTarget, int toTarget)
		{
			//value = Clamp(value, fromSource, toSource);
			value = Math.Min(value, toSource);
			return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
		}
		public static double Map(this double value, double fromSource, double toSource, double fromTarget, double toTarget)
		{
			//value = Clamp(value, fromSource, toSource);
			value = Math.Min(value, toSource);
			return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
		}

		public static double Map(this ulong value, ulong fromSource, ulong toSource, ulong fromTarget, ulong toTarget)
		{
			//value = Clamp(value, fromSource, toSource);
			value = Math.Min(value, toSource);
			return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
		}


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

	}
}
