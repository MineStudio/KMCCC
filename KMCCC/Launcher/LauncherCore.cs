using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using KMCCC.Tools;

namespace KMCCC.Launcher
{
	public partial class LauncherCore
	{
		public static LauncherCore Create(LauncherCoreCreationOption option)
		{
			LauncherCore launcherCore = new LauncherCore();
			launcherCore.GameRootPath = option.GameRootPath;
			launcherCore.JavaPath = option.JavaPath;
			return launcherCore;
		}

		private LauncherCore()
		{

		}

		public String GameRootPath { get; private set; }

		public String JavaPath { get; private set; }

		#region GetVersion

		public Version[] GetVersions()
		{
			try
			{
				var directories = new DirectoryInfo(GameRootPath + "/versions/").EnumerateDirectories();
				LinkedList<Version> result = new LinkedList<Version>();
				foreach (var directory in directories)
				{
					var ver = GetVersion(directory.Name);
					if (ver != null)
					{
						result.AddFirst(ver);
					}
				}
				return result.ToArray();
			}
			catch
			{
				return new Version[0];
			}
		}

		public Version GetVersion(String id)
		{
			var versionPath = GameRootPath + "/versions/" + id;
			if (!Directory.Exists(versionPath)) { return null; }
			versionPath = versionPath + "/" + id + ".json";
			if (!File.Exists(versionPath)) { return null; }
			return GetVersionInternal(versionPath);
		}

		#endregion
	}

	public class LauncherCoreCreationOption
	{
		public static LauncherCoreCreationOption Create(String GameRootPath = null, String JavaPath = null)
		{
			GameRootPath = GameRootPath ?? ".minecraft";
			JavaPath = JavaPath ?? SystemTools.FindJava();
			if (!Directory.Exists(GameRootPath)) { return null; }
			if (!File.Exists(JavaPath)) { return null; }
			LauncherCoreCreationOption option = new LauncherCoreCreationOption();
			option.GameRootPath = new DirectoryInfo(GameRootPath).FullName;
			option.JavaPath = JavaPath;
			return option;
		}

		public String GameRootPath { get; set; }

		public String JavaPath { get; set; }
	}
}
