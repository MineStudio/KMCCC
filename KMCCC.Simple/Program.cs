using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using KMCCC.Authentication;
using KMCCC.Launcher;
using KMCCC.Tools;

namespace KMCCC.Simple
{
	class Program
	{
		private static FileStream fs;

		private static TextWriter tw;

		private static AutoResetEvent are = new AutoResetEvent(false);

		private static LaunchHandle handle;

		static void Main(string[] args)
		{
			using (fs = new FileStream("mc.log", FileMode.Create))
			{
				using (tw = new StreamWriter(fs))
				{
					//这里图方便没有检验LauncherCoreCreationOption.Create()返回的是不是null
					LauncherCore core = LauncherCore.Create(LauncherCoreCreationOption.Create());
					core.GameExit += core_GameExit;
					core.GameLog += core_GameLog;
					handle = core.Launch(new LaunchOptions
					{
						Version = core.GetVersion("****"),
						Authenticator = new OfflineAuthenticator("KBlackcn"),
						//Authenticator = new YggdrasilLogin("****@****", "****", true),
						Server = new ServerInfo { Address = "mc.hypixel.net" },
						Mode = LaunchMode.BMCL,
						MaxMemory = 2048,
						MinMemory = 1024
					});
					are.WaitOne();
				}
			}
		}

		static void core_GameLog(LaunchHandle handle, string line)
		{
			tw.WriteLine(line);
		}

		static void core_GameExit(LaunchHandle handle, int code)
		{
			are.Set();
		}


	}
}
