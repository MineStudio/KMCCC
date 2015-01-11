namespace KMCCC.Modules.JVersion
{
	#region

	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using Launcher;
	using LitJson;
	using Tools;
	using Version = Launcher.Version;

	#endregion

	/// <summary>
	///     默认的版本定位器
	/// </summary>
	public class JVersionLocator : IVersionLocator
	{
		public string GameRootPath { get; set; }

		public LauncherCore Core
		{
			set { GameRootPath = value.GameRootPath; }
		}

		public Version Locate(string id)
		{
			var versionPath = GameRootPath + @"\versions\" + id;
			if (!Directory.Exists(versionPath))
			{
				return null;
			}
			versionPath = versionPath + '\\' + id + ".json";
			return File.Exists(versionPath)
				? GetVersionInternal(versionPath)
				: null;
		}

		public IEnumerable<Version> GetAllVersions()
		{
			try
			{
				return
					new DirectoryInfo(GameRootPath + @"\versions\").EnumerateDirectories()
						.Select(directory => Locate(directory.Name))
						.Where(ver => ver != null);
			}
			catch
			{
				return new Version[0];
			}
		}

		/// <summary>
		///     获取Version信息，当出现错误时会返回null
		/// </summary>
		/// <param name="jsonPath">json文件的位置</param>
		/// <returns>Version的信息</returns>
		internal Version GetVersionInternal(string jsonPath)
		{
			try
			{
				var ver = new Version();
				var json = File.ReadAllText(jsonPath);
				var jver = JsonMapper.ToObject<JVersion>(json);
				if (String.IsNullOrWhiteSpace(jver.Id))
				{
					return null;
				}
				if (String.IsNullOrWhiteSpace(jver.MinecraftArguments))
				{
					return null;
				}
				if (String.IsNullOrWhiteSpace(jver.MainClass))
				{
					return null;
				}
				if (String.IsNullOrWhiteSpace(jver.Assets))
				{
					jver.Assets = "legacy";
				}
				if (jver.Libraries == null)
				{
					return null;
				}
				ver.Id = jver.Id;
				ver.MinecraftArguments = jver.MinecraftArguments;
				ver.Assets = jver.Assets;
				ver.MainClass = jver.MainClass;
				ver.Libraries = new List<Library>();
				ver.Natives = new List<Native>();
				foreach (var lib in jver.Libraries)
				{
					if (String.IsNullOrWhiteSpace(lib.Name))
					{
						continue;
					}
					var names = lib.Name.Split(':');
					if (names.Length != 3)
					{
						continue;
					}
					if (lib.Natives == null)
					{
						if (!IfAllowed(lib.Rules))
						{
							continue;
						}
						ver.Libraries.Add(new Library
						{
							NS = names[0],
							Name = names[1],
							Version = names[2]
						});
					}
					else
					{
						if (!IfAllowed(lib.Rules))
						{
							continue;
						}
						var native = new Native
						{
							NS = names[0],
							Name = names[1],
							Version = names[2],
							NativeSuffix = lib.Natives["windows"].Replace("${arch}", SystemTools.GetArch())
						};
						ver.Natives.Add(native);
						if (lib.Extract != null)
						{
							native.Options = new UnzipOptions {Exclude = lib.Extract.Exculde};
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


		/// <summary>
		///     判断一系列规则后是否启用
		/// </summary>
		/// <param name="rules">规则们</param>
		/// <returns>是否启用</returns>
		public Boolean IfAllowed(List<JRule> rules)
		{
			if (rules == null)
			{
				return true;
			}
			if (rules.Count == 0)
			{
				return true;
			}
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