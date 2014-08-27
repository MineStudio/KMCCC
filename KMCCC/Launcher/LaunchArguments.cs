using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace KMCCC.Launcher
{
	public class MinecraftLaunchArguments
	{
		public MinecraftLaunchArguments()
		{
			Tokens = new Dictionary<String, String>();
			AdvencedArguments = new List<String>();
		}

		public String MainClass { get; set; }

		public List<String> Libraries { get; set; }

		public Int32 MaxMemory { get; set; }

		public Int32 MinMemory { get; set; }

		public Boolean CGCEnabled { get; set; }

		public String NativePath { get; set; }

		public String MinecraftArguments { get; set; }

		public Dictionary<String, String> Tokens { get; set; }

		public List<String> AdvencedArguments { get; set; }

		public String ToArguments()
		{
			return String.Empty;
		}
	}

}
