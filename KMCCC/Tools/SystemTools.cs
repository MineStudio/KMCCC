using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Microsoft.Win32;
using Microsoft.VisualBasic.Devices;

namespace KMCCC.Tools
{
	class SystemTools
	{
		public static String FindJava()
		{
			try
			{
				RegistryKey reg = Registry.LocalMachine;
				var openSubKey = reg.OpenSubKey("SOFTWARE");
				if (openSubKey != null)
				{
					var registryKey = openSubKey.OpenSubKey("JavaSoft");
					if (registryKey != null)
						reg = registryKey.OpenSubKey("Java Runtime Environment");
				}
				if (reg != null)
					foreach (string ver in reg.GetSubKeyNames())
					{
						try
						{
							RegistryKey command = reg.OpenSubKey(ver);
							if (command != null)
							{
								string str = command.GetValue("JavaHome").ToString();
								if (str != "")
									return str + @"\bin\javaw.exe";
							}
						}
						catch { return null; }
					}
				return null;
			}
			catch { return null; }
		}

		public static UInt64 GetTotalMemory()
		{
			return new Computer().Info.TotalPhysicalMemory;
		}

		public static String GetArch()
		{
			return Environment.Is64BitOperatingSystem ? "64" : "32";
		}
	}
}
