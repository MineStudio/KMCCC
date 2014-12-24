using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Microsoft.Win32;
using Microsoft.VisualBasic.Devices;

namespace KMCCC.Tools
{
	public class SystemTools
	{
		/// <summary>
		/// 从注册表中查找可能的JRE位置
		/// </summary>
		/// <returns>JAVA地址列表</returns>
		public static IEnumerable<string>  FindJava()
		{ 
			try
			{
				SortedSet<string> list = new SortedSet<string>();
				var reg = Registry.LocalMachine.OpenSubKey("SOFTWARE");
				if (reg != null)
				{
					var registryKey = reg.OpenSubKey("JavaSoft");
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
									list.Add(str + @"\bin\javaw.exe");
							}
						}
						catch { }
					}
				return list;
			}
			catch { return new string[0]; }
		}

		/// <summary>
		/// 取最大内存
		/// </summary>
		/// <returns>最大内存</returns>
		public static ulong GetTotalMemory()
		{
			return new Computer().Info.TotalPhysicalMemory;
		}

		/// <summary>
		/// 获取x86 or x64
		/// </summary>
		/// <returns>32 or 64</returns>
		public static string GetArch()
		{
			return Environment.Is64BitOperatingSystem ? "64" : "32";
		}
	}
}
