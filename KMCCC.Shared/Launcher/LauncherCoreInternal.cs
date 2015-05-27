namespace KMCCC.Launcher
{
	#region

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
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
				if (!string.IsNullOrWhiteSpace(authentication.Error))
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
				args.NativePath = GameRootPath + @"\$natives";
				foreach (var native in options.Version.Natives)
				{
					var exp = ZipTools.UnzipFile(this.GetNativePath(native), args.NativePath, native.Options);
					if (exp == null)
					{
						continue;
					}
					return new LaunchResult
					{
						Success = false,
						ErrorType = ErrorType.UncompressingFailed,
						ErrorMessage = string.Format("解压错误: {0}:{1}:{2}", native.NS, native.Name, native.Version),
						Exception = exp
					};
				}
				args.Server = options.Server;
				args.Size = options.Size;
				args.Libraries = options.Version.Libraries.Select(this.GetLibPath).ToList();
				args.Libraries.Add(this.GetVersionJarPath(options.Version.JarId));
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
				if (options.Mode != null)
				{
					options.Mode.Operate(this, args);
				}
				return null;
			}
			catch (Exception exp)
			{
				return new LaunchResult {Success = false, ErrorType = ErrorType.Unknown, ErrorMessage = "在生成参数时发生了意外的错误", Exception = exp};
			}
		}

		internal LaunchResult LaunchInternal(LaunchOptions options, params Action<MinecraftLaunchArguments>[] argumentsOperators)
		{
			lock (Locker)
			{
				if (!File.Exists(JavaPath))
				{
					return new LaunchResult {Success = false, ErrorType = ErrorType.NoJAVA, ErrorMessage = "指定的JAVA位置不存在"};
				}
				CurrentCode = Random.Next();
				var args = new MinecraftLaunchArguments();
				var result = GenerateArguments(options, ref args);
				if (result != null)
				{
					return result;
				}
				if (argumentsOperators == null) return LaunchGame(args);
				foreach (var opt in argumentsOperators)
				{
					try
					{
						if (opt != null)
						{
							opt(args);
						}
					}
					catch (Exception exp)
					{
						return new LaunchResult {Success = false, ErrorType = ErrorType.OperatorException, ErrorMessage = "指定的操作器引发了异常", Exception = exp};
					}
				}
				return LaunchGame(args);
			}
		}

		private LaunchResult LaunchGame(MinecraftLaunchArguments args)
		{
			try
			{
				var handle = new LaunchHandle(args.Authentication)
				{
					Code = CurrentCode,
					Core = this,
					Arguments = args,
					Process = Process.Start(new ProcessStartInfo(JavaPath)
					{
						Arguments = args.ToArguments(),
						UseShellExecute = false,
						WorkingDirectory = GameRootPath,
						RedirectStandardError = true,
						RedirectStandardOutput = true
					})
				};
				handle.Work();
				Task.Factory.StartNew(handle.Process.WaitForExit).ContinueWith(t => Exit(handle, handle.Process.ExitCode));
				return new LaunchResult {Success = true, Handle = handle};
			}
			catch (Exception exp)
			{
				return new LaunchResult {Success = false, ErrorType = ErrorType.Unknown, ErrorMessage = "启动时出现了异常", Exception = exp};
			}
		}

		#region 复制文件夹

		public void CopyVersionDirectory(string directoryName, string versionId)
		{
			CopyDirectory(string.Format(@"{0}\versions\{2}\{1}", GameRootPath, directoryName, versionId),
				string.Format(@"{0}\{1}", GameRootPath, directoryName));
		}

		public void CopyDirectory(string source, string target)
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

		public void CopyVersionDirectories(string ver)
		{
			var root = string.Format(@"{0}\versions\{1}\moddir", GameRootPath, ver);
			if (!Directory.Exists(root))
			{
				return;
			}
			foreach (var dir in new DirectoryInfo(root).EnumerateDirectories())
			{
				CopyDirectory(dir.FullName, string.Format(@"{0}\{1}", GameRootPath, dir.Name));
			}
		}

		#endregion

		#region 事件

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

		#endregion
	}
}