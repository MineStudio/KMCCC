namespace KMCCC.Tools
{
	#region

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Microsoft.VisualBasic.Devices;
	using Microsoft.Win32;

	#endregion

	public class SystemTools
	{
		/// <summary>
		///     从注册表中查找可能的JRE位置
		/// </summary>
		/// <returns>JAVA地址列表</returns>
		public static IEnumerable<string> FindJava()
		{
			var reg = Registry.LocalMachine.OpenSubKey("SOFTWARE");
			if (reg == null)
			{
				yield break;
			}
			var registryKey = reg.OpenSubKey("JavaSoft");
			if (registryKey != null)
				reg = registryKey.OpenSubKey("Java Runtime Environment");
			if (reg == null)
				yield break;
			foreach (var str in 
				from ver in reg.GetSubKeyNames()
				select reg.OpenSubKey(ver)
				into command
				where command != null
				select command.GetValue("JavaHome").ToString()
				into str
				where !String.IsNullOrWhiteSpace(str)
				select str)
			{
				yield return str + @"\bin\javaw.exe";
			}
		}

		/// <summary>
		///     取最大内存
		/// </summary>
		/// <returns>最大内存</returns>
		public static ulong GetTotalMemory()
		{
			return new Computer().Info.TotalPhysicalMemory;
		}

		/// <summary>
		///     获取x86 or x64
		/// </summary>
		/// <returns>32 or 64</returns>
		public static string GetArch()
		{
			return Environment.Is64BitOperatingSystem ? "64" : "32";
		}
	}
}