namespace KMCCC.Launcher
{
	#region

	using System;
	using System.Linq;
	using System.Management;
	using System.Net;
	using System.Reflection;
	using System.Text;
	using System.Threading.Tasks;
	using LitJson;
	using Tools;

	#endregion

	/// <summary>
	///     KMCCC启动报告上传
	/// </summary>
	public static partial class Reporter
	{
		/// <summary>
		///     启动报告的详细程度
		/// </summary>
		public enum ReportLevel
		{
			/// <summary>
			///     完整报告
			/// </summary>
			Full,

			/// <summary>
			///     去除可能的敏感信息
			/// </summary>
			Basic,

			/// <summary>
			///     关闭报告
			/// </summary>
			None
		}

#if DEBUG
		private const string SERVICE_ROOT = @"http://localhost:12580";
#else
		private const string SERVICE_ROOT = @"http://api.kmccc.minestudio.org/v1";
#endif
		private const string LAUNCH_REPORT = SERVICE_ROOT + "/launch";

		private static ReportLevel _reportLevel = ReportLevel.Full;

		private static string _clientName = "(Default) " + KMCCC_TYPE;

		public static void SetReportLevel(ReportLevel level)
		{
			_reportLevel = level;
		}

		/// <summary>
		///     设置客户端名称，默认为 "(Default) {KMCCC_TYPE}"
		/// </summary>
		/// <param name="name">客户端名称</param>
		public static void SetClientName(string name)
		{
			_clientName = name;
		}

		#region GetInfo

		private static string GetVideoCardInfo()
		{
			try
			{
				var sb = new StringBuilder();
				var i = 0;
				foreach (var mo in new ManagementClass("Win32_VideoController").GetInstances().Cast<ManagementObject>())
				{
					sb.Append('#').Append(i++).Append(mo["Name"].ToString().Trim()).Append(' ');
				}
				return sb.ToString();
			}
			catch
			{
				return String.Empty;
			}
		}

		private static string GetProcessorInfo()
		{
			try
			{
				var sb = new StringBuilder();
				var i = 0;
				foreach (var mo in new ManagementClass("WIN32_Processor").GetInstances().Cast<ManagementObject>())
				{
					sb.Append('#').Append(i++).Append(mo["Name"].ToString().Trim()).Append(' ');
				}
				return sb.ToString();
			}
			catch
			{
				return String.Empty;
			}
		}

		#endregion

		#region LaunchReport

		/// <summary>
		///     报告一次启动结果
		/// </summary>
		/// <param name="result">启动结果</param>
		public static LaunchResult Report(this LaunchResult result)
		{
			if (_reportLevel == ReportLevel.None) return result;
			Task.Factory.StartNew(() =>
			{
				try
				{
					var wc = new WebClient();
					wc.Headers.Add("user-agent", _clientName);
					wc.UploadString(LAUNCH_REPORT,
						JsonMapper.ToJson((_reportLevel == ReportLevel.Full) ? new FullLaunchReport(result) : new BasicLaunchReport(result))
#if DEBUG
							.Print()
#endif
						);
				}
				catch
				{
					Console.WriteLine("[KMCCC] report failed");
				}
			});
			return result;
		}

		public class BasicLaunchReport
		{
			#region 目录/位置

			public string JavaPath;

			#endregion

			#region 启动信息

			public string AuthenticationType;
			public string LaunchErrorType;

			#endregion

			#region KMCCC信息

			public string LauncherType;
			public string LauncherVersion;

			#endregion

			#region 系统信息

			public string Arch;
			public uint Memory;
			public string ProcessorInfo;
			public string RuntimeVersion;
			public string SystemVersion;
			public string VideoCardInfo;

			#endregion

			public BasicLaunchReport(LaunchResult result)
			{
				#region KMCCC信息

				LauncherType = KMCCC_TYPE;
				LauncherVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

				#endregion

				#region 系统信息

				RuntimeVersion = Environment.Version.ToString();
				SystemVersion = Environment.OSVersion.VersionString;
				Memory = ((uint) SystemTools.GetTotalMemory() << 20);
				Arch = SystemTools.GetArch();
				VideoCardInfo = GetVideoCardInfo();
				ProcessorInfo = GetProcessorInfo();

				#endregion

				#region 启动信息

				LaunchErrorType = result.ErrorType.ToString();
				JavaPath = result.Handle.Core.JavaPath;
				AuthenticationType = result.Handle.Info.Type;

				#endregion
			}
		}

		public class FullLaunchReport : BasicLaunchReport
		{
			#region 目录/位置

			public string GameDirectory;
			public string LauncherDirectory;

			#endregion

			#region 启动信息

			public string AutoConnectServer;
			public string LaunchedVersionId;
			public string PlayerName;

			#endregion

			public FullLaunchReport(LaunchResult result) : base(result)
			{
				LaunchedVersionId = result.Handle.Arguments.Version.Id;
				AutoConnectServer = (result.Handle.Arguments.Server != null) ? result.Handle.Arguments.Server.ToString() : "";
				LauncherDirectory = Environment.CurrentDirectory;
				GameDirectory = result.Handle.Core.GameRootPath;
				PlayerName = result.Handle.Arguments.Authentication.DisplayName;
			}
		}

		#endregion
	}
}