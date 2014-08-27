using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KMCCC.Tools;
using LitJson;

namespace KMCCC.Launcher
{
	public class Version
	{
		public String Id { get; set; }

		public String MinecraftArguments { get; set; }

		public String Assets { get; set; }

		public String MainClass { get; set; }

		public List<Library> Libraries { get; set; }

		public List<Native> Natives { get; set; }
	}

	public class Library
	{
		public String NS { get; set; }

		public String Name { get; set; }

		public String Version { get; set; }
	}

	public class Native
	{
		public String NS { get; set; }

		public String Name { get; set; }

		public String Version { get; set; }

		public String NativeSuffix { get; set; }

		public UnzipOptions Options { get; set; }
	}

	public static class LauncherCoreItemResolverExtensions
	{
		public static String GetJarPath(this LauncherCore core, Version version)
		{
			return String.Format("{0}/versions/{1}/{1}.jar", core.GameRootPath, version.Id);
		}

		public static String GetLibPath(this LauncherCore core, Library lib)
		{
			return String.Format("{0}/libraries/{1}/{2}/{3}/{2}-{3}.jar", core.GameRootPath, lib.NS, lib.Name, lib.Version);
		}

		public static String GetNativePath(this LauncherCore core, Version version)
		{
			return String.Format("{0}/natives/{1}/", core.GameRootPath, version.Id);
		}
	}
}
