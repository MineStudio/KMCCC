using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using KMCCC.Tools;
using KMCCC.Version;
using LitJson;

namespace KMCCC.Launcher
{
	partial class LauncherCore
	{
		internal Version GetVersionInternal(String jsonPath)
		{
			try
			{
				Version ver = new Version();
				var json = File.ReadAllText(jsonPath);
				var jver = JsonMapper.ToObject<JVersion>(json);
				if (String.IsNullOrWhiteSpace(jver.Id)) { return null; }
				if (String.IsNullOrWhiteSpace(jver.MinecraftArguments)) { return null; }
				if (String.IsNullOrWhiteSpace(jver.MainClass)) { return null; }
				if (String.IsNullOrWhiteSpace(jver.Assets)) { return null; }
				if (jver.Libraries == null) { return null; }
				ver.Id = jver.Id;
				ver.MinecraftArguments = jver.MinecraftArguments;
				ver.Assets = jver.Assets;
				ver.MainClass = jver.MainClass;
				ver.Libraries = new List<Library>();
				ver.Natives = new List<Native>();
				foreach (var lib in jver.Libraries)
				{
					if (String.IsNullOrWhiteSpace(lib.Name)) { continue; }
					var names = lib.Name.Split(':');
					if (names.Length != 3) { continue; }
					if (lib.Natives == null)
					{
						if (!IfAllowed(lib.Rules)) { continue; }
						ver.Libraries.Add(new Library { NS = names[0], Name = names[1], Version = names[2] });
					}
					else
					{
						if (!IfAllowed(lib.Rules)) { continue; }
						var native = new Native { NS = names[0], Name = names[1], Version = names[2] };
						ver.Natives.Add(native);
						if (lib.Extract != null)
						{
							native.Options = new UnzipOptions();
							native.Options.Exclude = lib.Extract.Exculde;
						}
					}
				}
				return ver;
			}
			catch
			{
				return null;
			}
		}

		public Boolean IfAllowed(List<JRule> rules)
		{
			if (rules == null) { return true; }
			if (rules.Count == 0) { return true; }
			var allowed = false;
			foreach (var rule in rules)
			{
				if (rule.OS == null)
				{
					allowed = rule.Action == "allow";
					continue;
				}
				if (rule.OS.Name == "windows")
				{
					allowed = rule.Action == "allow";
				}
			}
			return allowed;
		}
	}
}
