namespace KMCCC.Launcher
{
	#region

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Runtime.InteropServices;
	using System.Threading.Tasks;
	using Tools;

	#endregion

	partial class LauncherCore
	{
		internal object Locker = new object();


		private LaunchResult GenerateArguments(LaunchOptions options, ref MinecraftLaunchArguments args)
		{
			try
			{
				var authentication = options.Authenticator.Do();
				if (!String.IsNullOrWhiteSpace(authentication.Error))
					return new LaunchResult
					{
						Success = false,
						ErrorType = ErrorType.AuthenticationFailed,
						ErrorMessage = "验证错误: " + authentication.Error
					};
				args.CGCEnabled = true;
				args.MainClass = options.Version.MainClass;
				args.MaxMemory = options.MaxMemory;
				args.MinMemory = options.MinMemory;
				args.NativePath = GameRootPath + @"\$natvies-" + Guid.NewGuid();
				foreach (var native in options.Version.Natives)
				{
					try
					{
						ZipTools.Unzip(this.GetNativePath(native), args.NativePath, native.Options);
					}
					catch (Exception exp)
					{
						return new LaunchResult
						{
							Success = false,
							ErrorType = ErrorType.UncompressingFailed,
							ErrorMessage = string.Format("解压错误: {0}:{1}:{2}", native.NS, native.Name, native.Version),
							Exception = exp
						};
					}
				}
				args.Server = options.Server;
				args.Size = options.Size;
				args.Libraries = options.Version.Libraries.Select(this.GetLibPath).ToList();
				args.Libraries.Add(this.GetVersionJarPath(options.Version));
				args.MinecraftArguments = options.Version.MinecraftArguments;
				args.Tokens.Add("auth_access_token", authentication.AccessToken.GoString());
				args.Tokens.Add("auth_session", authentication.AccessToken.GoString());
				args.Tokens.Add("auth_player_name", authentication.DisplayName);
				args.Tokens.Add("version_name", options.Version.Id);
				args.Tokens.Add("game_directory", ".");
				args.Tokens.Add("game_assets", "assets");
				args.Tokens.Add("assets_root", "assets");
				args.Tokens.Add("assets_index_name", options.Version.Assets);
				args.Tokens.Add("auth_uuid", authentication.UUID.GoString());
				args.Tokens.Add("user_properties", authentication.Properties);
				args.Tokens.Add("user_type", authentication.UserType);
				args.AdvencedArguments = new List<string> {"-Dfml.ignoreInvalidMinecraftCertificates=true -Dfml.ignorePatchDiscrepancies=true"};
				args.Authentication = authentication;
				args.Version = options.Version;
				switch (options.Mode)
				{
					case LaunchMode.BMCL:
						Bmclmode(args);
						break;
					case LaunchMode.McLauncher:
						McLaunchermode(args);
						break;
				}
				return null;
			}
			catch (Exception exp)
			{
				return new LaunchResult {Success = false, ErrorType = ErrorType.Unknown, ErrorMessage = "在生成参数时发生了意外的错误", Exception = exp};
			}
		}

		private void Bmclmode(MinecraftLaunchArguments args)
		{
			OperateDirectory("mods", args.Version.Id);
			OperateDirectory("coremods", args.Version.Id);
			OperateDirectories(this.GetVersionRootPath(args.Version));
		}

		private static void McLaunchermode(MinecraftLaunchArguments args)
		{
			args.Tokens["game_directory"] = String.Format(@".\versions\{0}\", args.Version.Id);
		}

		private void OperateDirectory(string name, string ver)
		{
			OperateDirectoryInternal(String.Format(@"{0}\versions\{2}\{1}", GameRootPath, name, ver),
				String.Format(@"{0}\{1}", GameRootPath, name));
		}

		private void OperateDirectoryInternal(string source, string target)
		{
			var code = CurrentCode;
			if (!Directory.Exists(source)) return;
			if (Directory.Exists(target))
			{
				Directory.Delete(target, true);
			}
			UsefulTools.Dircopy(source, target);
			Action<LaunchHandle, int> handler = null;
			handler = (handle, c) =>
			{
				if (handle.Code == code)
				{
					Directory.Delete(source, true);
					UsefulTools.Dircopy(target, source);
					Directory.Delete(target, true);
				}
				GameExit -= handler;
			};
			GameExit += handler;
		}

		private void OperateDirectories(string ver)
		{
			var root = String.Format(@"{0}\versions\{1}\moddir", GameRootPath, ver);
			if (!Directory.Exists(root))
			{
				return;
			}
			foreach (var dir in new DirectoryInfo(root).EnumerateDirectories())
			{
				OperateDirectoryInternal(dir.FullName, String.Format(@"{0}\{1}", GameRootPath, dir.Name));
			}
		}

		internal void Log(LaunchHandle handle, string line)
		{
			if (GameLog != null)
			{
				GameLog(handle, line);
			}
		}

		internal void Exit(LaunchHandle handle, int code)
		{
			if (GameExit != null)
			{
				GameExit(handle, code);
			}
		}

		private LaunchResult LaunchInternal(MinecraftLaunchArguments args)
		{
			try
			{
				var handle = new LaunchHandle(args.Authentication) {Code = CurrentCode, Core = this};
				var psi = new ProcessStartInfo(JavaPath)
				{
					Arguments = args.ToArguments(),
					UseShellExecute = false,
					WorkingDirectory = GameRootPath,
					RedirectStandardError = true,
					RedirectStandardOutput = true
				};
				handle.Process = Process.Start(psi);
				handle.Work();
				Task.Factory.StartNew(handle.Process.WaitForExit).ContinueWith(t =>
				{
					Directory.Delete(args.NativePath, true);
					Exit(handle, handle.Process.ExitCode);
				});
				return new LaunchResult {Success = true, Handle = handle};
			}
			catch (Exception exp)
			{
				return new LaunchResult {Success = false, ErrorType = ErrorType.Unknown, ErrorMessage = "启动时出现了异常", Exception = exp};
			}
		}
	}

	public static class LaunchHandleExtensions
	{
		public static void SetTitle(this LaunchHandle handle, string title)
		{
			try
			{
				SetWindowText(handle.Process.MainWindowHandle, title);
			}
			catch
			{
			}
		}

		[DllImport("User32.dll")]
		public static extern int SetWindowText(IntPtr winHandle, string title);
	}
}