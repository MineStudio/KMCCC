using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using KMCCC.Tools;
using KMCCC.Version;
using LitJson;

namespace KMCCC.Launcher
{
	partial class LauncherCore
	{
		#region GetVesion

		internal object locker = new object();

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
				if (String.IsNullOrWhiteSpace(jver.Assets)) { jver.Assets = "legacy"; }
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
						ver.Libraries.Add(new Library
						{
							NS = names[0],
							Name = names[1],
							Version = names[2]
						});
					}
					else
					{
						if (!IfAllowed(lib.Rules)) { continue; }
						var native = new Native
						{
							NS = names[0],
							Name = names[1],
							Version = names[2],
							NativeSuffix = lib.Natives["windows"].Replace("{arch}", SystemTools.GetArch())
						};
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

		#endregion

		private MinecraftLaunchArguments GenerateArguments(LaunchOptions options)
		{
			var authentication = options.Authenticator.Do();
			if (String.IsNullOrWhiteSpace(authentication.Error))
			{
				MinecraftLaunchArguments args = new MinecraftLaunchArguments();
				args.CGCEnabled = true;
				args.MainClass = options.Version.MainClass;
				args.MaxMemory = options.MaxMemory;
				args.MinMemory = options.MinMemory;
				args.NativePath = GameRootPath + @"\$natvies-" + Guid.NewGuid().ToString();
				foreach (var native in options.Version.Natives)
				{
					try
					{
						ZipTools.Unzip(this.GetNativePath(native), args.NativePath, native.Options);
					}
					catch { }
				}
				args.Libraries = options.Version.Libraries.Select(lib => this.GetLibPath(lib)).ToList();
				args.Libraries.Add(this.GetVersionJarPath(options.Version));
				args.MinecraftArguments = options.Version.MinecraftArguments;
				args.Tokens.Add("auth_access_token", authentication.AccessToken.GoString());
				args.Tokens.Add("auth_player_name", authentication.DisplayName);
				args.Tokens.Add("version_name", options.Version.Id);
				args.Tokens.Add("game_directory", ".");
				args.Tokens.Add("game_assets", "assets");
				args.Tokens.Add("assets_root", "assets");
				args.Tokens.Add("assets_index_name", options.Version.Assets);
				args.Tokens.Add("auth_uuid", authentication.UUID.GoString());
				args.Tokens.Add("user_properties", authentication.Properties);
				args.Tokens.Add("user_type", authentication.UserType);
				args.AdvencedArguments = new List<string> { "-Dfml.ignoreInvalidMinecraftCertificates=true -Dfml.ignorePatchDiscrepancies=true" };
				args.authentication = authentication;
				args.version = options.Version;
				switch (options.Mode)
				{
					case LaunchMode.BMCL:
						bmclmode(args);
						break;
					case LaunchMode.MCLauncher:
						mcLaunchermode(args);
						break;
				}
				return args;
			}
			else
			{
				return null;
			}
		}

		private LaunchHandle launch(MinecraftLaunchArguments args)
		{
			try
			{
				LaunchHandle handle = new LaunchHandle(args.authentication);
				handle.code = this.currentCode;
				handle.core = this;
				ProcessStartInfo psi = new ProcessStartInfo(this.JavaPath);
				psi.Arguments = args.ToArguments();
				psi.UseShellExecute = false;
				psi.WorkingDirectory = GameRootPath;
				psi.RedirectStandardError = true;
				psi.RedirectStandardOutput = true;
				handle.process = Process.Start(psi);
				handle.work();
				Task task = Task.Factory.StartNew(() =>
				{
					handle.process.WaitForExit();
				}).ContinueWith(t =>
					{
						Directory.Delete(args.NativePath, true);
						this.exit(handle, handle.process.ExitCode);
					});
				return handle;
			}
			catch (Exception)
			{
				return null;
			}
		}

		private void bmclmode(MinecraftLaunchArguments args)
		{
			operateDirectory("mods", args.version.Id);
			operateDirectory("coremods", args.version.Id);
			operateDirectories(this.GetVersionRootPath(args.version));
		}

		private void mcLaunchermode(MinecraftLaunchArguments args)
		{
			args.Tokens["game_directory"] = this.GetVersionRootPath(args.version);
		}

		private void operateDirectory(String name, String ver)
		{
			operateDirectoryInternal(String.Format(@"{0}\versions\{2}\{1}", GameRootPath, name, ver),
									 String.Format(@"{0}\{1}", GameRootPath, name));
		}

		private void operateDirectoryInternal(String source, String target)
		{
			int code = currentCode;
			if (Directory.Exists(source))
			{
				if (Directory.Exists(target))
				{
					Directory.Delete(target, true);
				}
				UsefulTools.Dircopy(source, target); Action<LaunchHandle, int> handler = null;
				handler = (handle, c) =>
				{
					if (handle.code == code)
					{
						Directory.Delete(source, true);
						UsefulTools.Dircopy(target, source);
						Directory.Delete(target, true);
					}
					GameExit -= handler;
				};
				GameExit += handler;
			}
		}

		private void operateDirectories(String ver)
		{
			var root = String.Format(@"{0}\versions\{1}\moddir", GameRootPath, ver);
			if (!Directory.Exists(root)) { return; }
			foreach (var dir in new DirectoryInfo(root).EnumerateDirectories())
			{
				operateDirectoryInternal(dir.FullName, String.Format(@"{0}\{1}", GameRootPath, dir.Name));
			}
		}

		internal void log(LaunchHandle handle, String line)
		{
			if (GameLog != null)
			{
				GameLog(handle, line);
			}
		}

		internal void exit(LaunchHandle handle, int code)
		{
			if (GameExit != null)
			{
				GameExit(handle, code);
			}
		}

		/// <summary>
		/// 判断一系列规则后是否启用
		/// </summary>
		/// <param name="rules">规则们</param>
		/// <returns>是否启用</returns>
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

	public static class LaunchHandleExtensions
	{
		public static void SetTitle(this LaunchHandle handle, String title)
		{
			try
			{
				SetWindowText(handle.process.MainWindowHandle, title);
			}
			catch { }
		}

		[DllImport("User32.dll")]
		public static extern int SetWindowText(IntPtr WinHandle, String Title);
	}
}
