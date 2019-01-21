﻿namespace KMCCC.Modules.JVersion
{
	#region

	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using Launcher;
	using LitJson;
	using Tools;

	#endregion

	/// <summary>
	///     默认的版本定位器
	/// </summary>
	public class JVersionLocator : IVersionLocator
	{
		private readonly HashSet<string> _locatingVersion;

		private readonly Dictionary<string, Version> _versions;

		public JVersionLocator()
		{
			_versions = new Dictionary<string, Version>();
			_locatingVersion = new HashSet<string>();
		}

		public string GameRootPath { get; set; }

		private LauncherCore _core;

		public LauncherCore Core
		{
			set
			{
				GameRootPath = value.GameRootPath;
				_core = value;
			}
		}

		public Version Locate(string id)
		{
			lock (_locatingVersion)
			{
				return GetVersionInternal(id);
			}
		}

		public IEnumerable<Version> GetAllVersions()
		{
			try
			{
				lock (_locatingVersion)
				{
					return new DirectoryInfo(GameRootPath + (SystemTools.GetIsWindows() ? @"\versions" : @"/versions")).EnumerateDirectories()
						.Select(dir => GetVersionInternal(dir.Name)).Where(item => item != null);
				}
			}
			catch
			{
				return new Version[0];
			}
		}

        private string GetOSSpecifiedNative() {
            switch(System.Environment.OSVersion.Platform)
            {
                case System.PlatformID.Unix:
                    return "linux";
                case System.PlatformID.MacOSX:
                    return "macos";
                default:
                    return "windows";
            }
        }

        /// <summary>
        ///     获取Version信息，当出现错误时会返回null
        /// </summary>
        /// <param name="id">版本id</param>
        /// <returns>Version的信息</returns>
        internal Version GetVersionInternal(string id)
		{
            try
            {
                if (_locatingVersion.Contains(id))
                {
                    return null;
                }
                _locatingVersion.Add(id);

                if (_versions.TryGetValue(id, out Version version))
                {
                    return version;
                }

                var jver = LoadVersion(_core.GetVersionJsonPath(id));
                if (jver == null)
                {
                    return null;
                }

                version = new Version();
                if (string.IsNullOrWhiteSpace(jver.Id))
                {
                    return null;
                }
                if (jver.arguments == null && string.IsNullOrWhiteSpace(jver.MinecraftArguments))
                {
                    return null;
                }
                if (string.IsNullOrWhiteSpace(jver.MainClass))
                {
                    return null;
                }
                if (string.IsNullOrWhiteSpace(jver.Assets))
                {
                    jver.Assets = "legacy";
                }
                if (jver.Libraries == null)
                {
                    return null;
                }
                version.Id = jver.Id;
                if (jver.AssetsIndex != null)
                {
                    version.AssetsIndex = new GameFileInfo()
                    {
                        ID = jver.AssetsIndex.ID,
                        Path = jver.AssetsIndex.Path,
                        SHA1 = jver.AssetsIndex.SHA1,
                        Size = jver.AssetsIndex.Size,
                        TotalSize = jver.AssetsIndex.TotalSize,
                        Url = jver.AssetsIndex.Url
                    };
                }
                if (jver.Downloads != null)
                {
                    version.Downloads = new Download()
                    {
                        Client = jver.Downloads.Client != null ? new GameFileInfo()
                        {
                            SHA1 = jver.Downloads.Client.SHA1,
                            Size = jver.Downloads.Client.Size,
                            Url = jver.Downloads.Client.Url
                        } : null,
                        Server = jver.Downloads.Server != null ? new GameFileInfo()
                        {
                            SHA1 = jver.Downloads.Server.SHA1,
                            Size = jver.Downloads.Server.Size,
                            Url = jver.Downloads.Server.Url
                        } : null
                    };
                }
                version.MinecraftArguments = jver.MinecraftArguments ?? UsefulTools.PrintfArray(jver.arguments.game);
                version.Assets = jver.Assets;
				version.MainClass = jver.MainClass;
				version.JarId = jver.JarId;
				version.Libraries = new List<Library>();
				version.Natives = new List<Native>();
				foreach (var lib in jver.Libraries)
				{
					if (string.IsNullOrWhiteSpace(lib.Name))
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
						version.Libraries.Add(new Library
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
							NativeSuffix = lib.Natives[GetOSSpecifiedNative()].Replace("${arch}", SystemTools.GetArch())
						};
						version.Natives.Add(native);
						if (lib.Extract != null)
						{
							native.Options = new UnzipOptions {Exclude = lib.Extract.Exculde};
						}
					}
				}
                
                if (jver.InheritsVersion != null)
				{
					var target = GetVersionInternal(jver.InheritsVersion);
					if (target == null)
					{
						return null;
					}
					else
					{
                        if (version.Assets == "legacy")
                            version.Assets = null;
                        version.AssetsIndex = version.AssetsIndex ?? target.AssetsIndex;
                        version.Downloads = version.Downloads ?? target.Downloads;
                        version.Assets = version.Assets ?? target.Assets;
						version.JarId = version.JarId ?? target.JarId;
						version.MainClass = version.MainClass ?? target.MainClass;
						version.MinecraftArguments = version.MinecraftArguments ?? target.MinecraftArguments;
						version.Natives.AddRange(target.Natives);
						version.Libraries.AddRange(target.Libraries);
                        version.AssetsIndex = version.AssetsIndex ?? target.AssetsIndex;
                    }
				}


                version.JarId = version.JarId ?? version.Id;
				_versions.Add(version.Id, version);
				return version;
			}
            catch (System.Exception)
            {
                return null;
			}
			finally
			{
				_locatingVersion.Remove(id);
			}
		}

		public JVersion LoadVersion(string jsonPath)
		{
			try
			{
				return JsonMapper.ToObject<JVersion>(File.ReadAllText(jsonPath));
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
		public bool IfAllowed(List<JRule> rules)
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