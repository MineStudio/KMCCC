namespace KMCCC.Launcher
{
	#region

	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using Modules.JVersion;
	using Tools;

	#endregion

	/// <summary>
	///     启动器核心
	/// </summary>
	public partial class LauncherCore
	{
		internal int CurrentCode;

		internal Random Random = new Random();
		private IVersionLocator _versionLocator;

		#region GetVersion

		/// <summary>
		///     返回包含全部版本数组
		/// </summary>
		/// <returns>版本数组</returns>
		public IEnumerable<Version> GetVersions()
		{
			return (VersionLocator == null)
				? new Version[0]
				: _versionLocator.GetAllVersions();
		}

		/// <summary>
		///     返回指定id的版本
		/// </summary>
		/// <param name="id">要指定的ID</param>
		/// <returns>指定的版本</returns>
		public Version GetVersion(string id)
		{
			return (VersionLocator == null)
				? null
				: _versionLocator.Locate(id);
		}

		#endregion

		private LauncherCore()
		{
		}

		/// <summary>
		///     游戏根目录
		/// </summary>
		public string GameRootPath { get; private set; }

		/// <summary>
		///     JAVA目录
		/// </summary>
		public string JavaPath { get; set; }

		/// <summary>
		///     版本定位器
		/// </summary>
		public IVersionLocator VersionLocator
		{
			get { return _versionLocator; }
			set { (_versionLocator = value).Core = this; }
		}

		/// <summary>
		///     从CreationOption创建启动器核心
		/// </summary>
		/// <param name="option">启动器创建选项</param>
		/// <returns>创建的启动器核心</returns>
		public static LauncherCore Create(LauncherCoreCreationOption option)
		{
			var launcherCore = new LauncherCore
			{
				GameRootPath = option.GameRootPath,
				JavaPath = option.JavaPath,
				VersionLocator = option.VersionLocator
			};
			return launcherCore;
		}

		public static LauncherCore Create(string gameRootPath = null)
		{
			return Create(new LauncherCoreCreationOption(gameRootPath ?? @".minecraft"));
		}

		/// <summary>
		///     启动函数
		///     过程：
		///     1. 运行验证器(authenticator)，出错返回null
		///     2. 继续构造启动参数
		///     3. 遍历Operators对启动参数进行修改
		///     4. 启动
		/// </summary>
		/// <param name="options">启动选项</param>
		/// <param name="argumentsOperators">启动参数的修改器</param>
		/// <returns>启动结果</returns>
		public LaunchResult Launch(LaunchOptions options, params Action<MinecraftLaunchArguments>[] argumentsOperators)
		{
			return this.Report(LaunchInternal(options, argumentsOperators), options);
		}

		/// <summary>
		///     游戏退出事件
		/// </summary>
		public event Action<LaunchHandle, int> GameExit;

		/// <summary>
		///     游戏Log事件
		/// </summary>
		public event Action<LaunchHandle, string> GameLog;
	}

	/// <summary>
	///     启动器核心选项
	///     以后可能包含更多内容
	/// </summary>
	public class LauncherCoreCreationOption
	{
		/// <summary>
		///     核心选项
		/// </summary>
		/// <param name="gameRootPath">游戏根目录，默认为 ./.minecraft </param>
		/// <param name="javaPath">JAVA地址，默认为自动搜寻所的第一个</param>
		/// <param name="versionLocator">Version定位器，默认为 JVersionLoacator</param>
		public LauncherCoreCreationOption(string gameRootPath = null, string javaPath = null, IVersionLocator versionLocator = null)
		{
			GameRootPath = new DirectoryInfo(gameRootPath ?? ".minecraft").FullName;
			JavaPath = javaPath ?? SystemTools.FindJava().FirstOrDefault();
			VersionLocator = versionLocator ?? new JVersionLocator();
			if (!Directory.Exists(GameRootPath))
			{
				Directory.CreateDirectory(GameRootPath);
			}
		}

		/// <summary>
		///     游戏根目录
		/// </summary>
		public string GameRootPath { get; internal set; }

		/// <summary>
		///     JAVA地址
		/// </summary>
		public string JavaPath { get; internal set; }

		/// <summary>
		///     Version定位器
		/// </summary>
		public IVersionLocator VersionLocator { get; internal set; }

		[Obsolete]
		public static LauncherCoreCreationOption Create(string gameRootPath = null, string javaPath = null, IVersionLocator versionLocator = null)
		{
			return new LauncherCoreCreationOption(gameRootPath, javaPath, versionLocator);
		}
	}

	/// <summary>
	///     启动后返回的启动结果
	/// </summary>
	public class LaunchResult
	{
		/// <summary>
		///     获取是否启动成功
		/// </summary>
		public bool Success { get; set; }

		/// <summary>
		///     获取发生的错误类型
		/// </summary>
		public ErrorType ErrorType { get; set; }

		/// <summary>
		///     获取错误信息
		/// </summary>
		public string ErrorMessage { get; set; }

		/// <summary>
		///     启动时发生异常
		/// </summary>
		public Exception Exception { get; set; }

		/// <summary>
		///     获取启动句柄
		/// </summary>
		public LaunchHandle Handle { get; set; }
	}

	public enum ErrorType
	{
		/// <summary>
		///     没有错误
		/// </summary>
		None,

		/// <summary>
		///     没有找到JAVA
		/// </summary>
		NoJAVA,

		/// <summary>
		///     验证失败
		/// </summary>
		AuthenticationFailed,

		/// <summary>
		///     操作器出现故障
		/// </summary>
		OperatorException,

		/// <summary>
		///     未知
		/// </summary>
		Unknown,

		/// <summary>
		///     解压错误
		/// </summary>
		UncompressingFailed
	}
}