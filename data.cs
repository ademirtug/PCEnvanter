using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PCEnvanter
{
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

		//public bool RetrieveInfo()
		//{
		//    var pt = getPCType(Name);
		//    Model = pt[0];
		//    Manufacturer = pt[1];
		//    User = getPCUser();
		//    IP = getPCIP();
		//    Enclosure = getPCEnclosure();
		//    Cpu = getCPU();
		//    Disk = getDisk();
		//    Memory = getMemory();
		//    Wei = getWei();
		//    var xe = getMonitor();
		//    double size = GetMonitorSize();
		//} 
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

				foreach (var cpu in PCEnvanter.cpuList)
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

		public int Cores { get; set; } = 0;

		public int Frequency { get; set; } = 0;
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

	}
}
