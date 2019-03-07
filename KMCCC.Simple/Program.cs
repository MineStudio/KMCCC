namespace KMCCC.Simple
{
	#region

	using System;
	using System.IO;
	using System.Threading;
	using Authentication;
	using Launcher;
    using KMCCC.Modules.Minecraft;
    using KMCCC.Pro.Modules.MojangAPI;

    #endregion

    internal class Program
	{
		private static FileStream _fs;

		private static TextWriter _tw;

		private static readonly AutoResetEvent Are = new AutoResetEvent(false);

		private static void Main()
		{
            TestTimer Timer = new TestTimer();
            
            try
            {
                var a = MojangAPI.GetStatistics();
                Console.WriteLine(a.ToString()+ "\n" + Timer.ToString());
                var api = MojangAPI.GetServiceStatus();
                foreach (var list in api)
                {
                    Console.WriteLine($"{list.Key} : {list.Value}");
                }

                Console.WriteLine("UUID:" + MojangAPI.NameToUUID("Notch") +"\n" + Timer.ToString());
            }
            catch(Exception ex)
            {

            }

            /*
            try
            {
                var ping = new ServerPing("mc.hypixel.net", 25565);
                var server = ping.Ping();
                Console.WriteLine(Timer.ToString());
                Console.WriteLine(server.description.text);
                Console.WriteLine("{0} / {1}", server.players.online, server.players.max);
                Console.WriteLine(server.version.name);
                Console.WriteLine(server.modinfo);
                
            }
            catch(Exception ex)
            {
                Console.WriteLine("服务器信息获取失败:"+ex.Message+"\n"+ Timer.ToString());
            }
            */

            Console.WriteLine("初始化"+Timer.ToString());
            using (_fs = new FileStream("mc.log", FileMode.Create))
			{
				using (_tw = new StreamWriter(_fs))
				{
					//这里图方便没有检验LauncherCoreCreationOption.Create()返回的是不是null
					var core = LauncherCore.Create();
					core.GameExit += core_GameExit;
					core.GameLog += core_GameLog;
                    Console.WriteLine("创建核心"+Timer.ToString());
                    var launch = new LaunchOptions
                    {
                        Version = core.GetVersion("1.13.2"),
                        //Authenticator = new YggdrasilRefresh(new Guid(),false),
                        Authenticator = new OfflineAuthenticator("test"),
                        //Authenticator = new YggdrasilValidate(Guid.Parse("****"), Guid.Parse("****"), Guid.Parse("****"), "***"),
                        //Authenticator = new YggdrasilLogin("***@**", "***", true,Guid.Parse("****")),
                        //Authenticator = new YggdrasilAuto("***@**", "***", null, null, null, null),
                        //Server = new ServerInfo {Address = "mc.hypixel.net"},
                        Mode = null,
                        MaxMemory = 2048,
                        MinMemory = 1024,
                        Size = new WindowSize { Height = 768, Width = 1280 }
                    };
                    Console.WriteLine("设置参数"+Timer.ToString());
                    var result = core.Launch(launch, (Action<MinecraftLaunchArguments>) (x => { }));
                    Console.WriteLine("开启游戏"+Timer.ToString());
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
                    Console.WriteLine($"AccessToken:{result.Handle.Info.AccessToken} " + "\n" + Timer.ToString());
                    GC.Collect(0);
					Are.WaitOne();
					Console.WriteLine("游戏已关闭");
                    result = null;
                    GC.Collect(0);
                    Console.ReadKey();
				}
			}
		}

		private static void core_GameLog(LaunchHandle handle, string line)
		{
			Console.WriteLine(line);
			_tw.WriteLine(line);

			//handle.SetTitle("啦啦啦");
		}

		private static void core_GameExit(LaunchHandle handle, int code)
		{
			Are.Set();
		}
	}

    public class TestTimer
    {

        private int count = Environment.TickCount;
        private int now = Environment.TickCount;

        public int Used
        {
            get
            {
                int used = Environment.TickCount - this.now;
                this.now = Environment.TickCount;
                return used;
            }
        }

        public int Total
        {
            get { return Environment.TickCount - this.count; }
        }

        public override string ToString() => $" [消耗时长: {this.Used}毫秒, 共消耗: {this.Total}毫秒]";

    }
}