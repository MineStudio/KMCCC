namespace KMCCC.Simple
{
	#region

	using System;
	using System.IO;
	using System.Threading;
	using Authentication;
	using Launcher;

	#endregion

	internal class Program
	{
		private static FileStream _fs;

		private static TextWriter _tw;

		private static readonly AutoResetEvent Are = new AutoResetEvent(false);

		private static void Main()
		{
			using (_fs = new FileStream("mc.log", FileMode.Create))
			{
				using (_tw = new StreamWriter(_fs))
				{
					//这里图方便没有检验LauncherCoreCreationOption.Create()返回的是不是null
					var core = LauncherCore.Create();
					core.GameExit += core_GameExit;
					core.GameLog += core_GameLog;
					var result = core.Launch(new LaunchOptions
					{
						Version = core.GetVersion("****"),
						Authenticator = new OfflineAuthenticator("KBlackcn"),
						//Authenticator = new YggdrasilLogin("****@****", "****", true),
						//Server = new ServerInfo {Address = "mc.hypixel.net"},
						Mode = null,
						MaxMemory = 2048,
						MinMemory = 1024,
						Size = new WindowSize {Height = 768, Width = 1280}
					}, (Action<MinecraftLaunchArguments>) (x => { }));
					if (!result.Success)
					{
						Console.WriteLine("启动失败：[{0}] {1}", result.ErrorType, result.ErrorMessage);
						if (result.Exception != null)
						{
							Console.WriteLine(result.Exception.Message);
							Console.WriteLine(result.Exception.Source);
							Console.WriteLine(result.Exception.StackTrace);
						}
						Console.ReadKey();
						return;
					}
					Are.WaitOne();
					Console.WriteLine("游戏已关闭");
					Console.ReadKey();
				}
			}
		}

		private static void core_GameLog(LaunchHandle handle, string line)
		{
			Console.WriteLine(line);
			_tw.WriteLine(line);

			handle.SetTitle("啦啦啦");
		}

		private static void core_GameExit(LaunchHandle handle, int code)
		{
			Are.Set();
		}
	}
}