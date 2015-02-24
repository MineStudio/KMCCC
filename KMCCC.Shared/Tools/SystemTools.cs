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
		///     从注册表中查找可能的javaw.exe位置
		/// </summary>
		/// <returns>JAVA地址列表</returns>
		public static IEnumerable<string> FindJava()
		{
			try
			{
				var rootReg = Registry.LocalMachine.OpenSubKey("SOFTWARE");
				return rootReg == null
					? new string[0]
					: FindJavaInternal(rootReg).Union(FindJavaInternal(rootReg.OpenSubKey("Wow6432Node")));
			}
			catch
			{
				return new string[0];
			}
		}

		public static IEnumerable<string> FindJavaInternal(RegistryKey registry)
		{
			try
			{
				var registryKey = registry.OpenSubKey("JavaSoft");
				if ((registryKey == null) || ((registry = registryKey.OpenSubKey("Java Runtime Environment")) == null)) return new string[0];
				return (from ver in registry.GetSubKeyNames()
					select registry.OpenSubKey(ver)
					into command
					where command != null
					select command.GetValue("JavaHome")
					into javaHomes
					where javaHomes != null
					select javaHomes.ToString()
					into str
					where !String.IsNullOrWhiteSpace(str)
					select str + @"\bin\javaw.exe");
			}
			catch
			{
				return new string[0];
			}
		}

		/// <summary>
		///     取物理内存
		/// </summary>
		/// <returns>物理内存</returns>
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