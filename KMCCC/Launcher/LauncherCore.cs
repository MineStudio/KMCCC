using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using KMCCC.Tools;

namespace KMCCC.Launcher
{
	/// <summary>
	/// 启动器核心
	/// </summary>
	public partial class LauncherCore
	{
		/// <summary>
		/// 从CreationOption创建启动器核心
		/// </summary>
		/// <param name="option">启动器创建选项</param>
		/// <returns>创建的启动器核心</returns>
		public static LauncherCore Create(LauncherCoreCreationOption option)
		{
			LauncherCore launcherCore = new LauncherCore();
			launcherCore.GameRootPath = option.GameRootPath;
			launcherCore.JavaPath = option.JavaPath;
			return launcherCore;
		}

		public static LauncherCore Create(string gameRootPath = null)
		{
			return Create(LauncherCoreCreationOption.Create(gameRootPath));
		}

		private LauncherCore()
		{

		}

		/// <summary>
		/// 游戏根目录
		/// </summary>
		public string GameRootPath { get; private set; }

		/// <summary>
		/// JAVA目录
		/// </summary>
		public string JavaPath { get; set; }

		#region GetVersion

		/// <summary>
		/// 返回包含全部版本数组
		/// </summary>
		/// <returns>版本数组</returns>
		public Version[] GetVersions()
		{
			lock (locker)
			{
				try
				{
					var directories = new DirectoryInfo(GameRootPath + @"\versions\").EnumerateDirectories();
					LinkedList<Version> result = new LinkedList<Version>();
					foreach (var directory in directories)
					{
						var ver = GetVersion(directory.Name);
						if (ver != null)
						{
							result.AddFirst(ver);
						}
					}
					return result.ToArray();
				}
				catch
				{
					return new Version[0];
				}
			}
		}

		/// <summary>
		/// 返回指定id的版本
		/// </summary>
		/// <param name="id">要指定的ID</param>
		/// <returns>指定的版本</returns>
		public Version GetVersion(string id)
		{
			lock (locker)
			{
				var versionPath = GameRootPath + @"\versions\" + id;
				if (!Directory.Exists(versionPath)) { return null; }
				versionPath = versionPath + '\\' + id + ".json";
				if (!File.Exists(versionPath)) { return null; }
				return getVersionInternal(versionPath);
			}
		}

		#endregion

		/// <summary>
		/// 启动函数
		/// 过程：
		/// 1. 运行验证器(authenticator)，出错返回null
		/// 2. 继续构造启动参数
		/// 3. 遍历Operators对启动参数进行修改
		/// 4. 启动
		/// </summary>
		/// <param name="options">启动选项</param>
		/// <param name="argumentsOperators">启动参数的修改器</param>
		/// <returns>启动结果</returns>
		public LaunchResult Launch(LaunchOptions options, params Action<MinecraftLaunchArguments>[] argumentsOperators)
		{
			lock (locker)
			{
				if (!File.Exists(JavaPath)) { return new LaunchResult { Success = false, ErrorType = ErrorType.NoJAVA, ErrorMessage = "指定的JAVA位置不存在" }; }
				currentCode = random.Next();
				MinecraftLaunchArguments args = new MinecraftLaunchArguments();
				var result = generateArguments(options, ref args);
				if (result != null) { return result; }
				if (argumentsOperators != null)
				{
					foreach (var opt in argumentsOperators)
					{
						try
						{
							if (opt != null)
							{
								opt(args);
							}
						}
						catch(Exception exp) { return new LaunchResult { Success = false, ErrorType = ErrorType.OperatorException, ErrorMessage = "指定的操作器引发了异常", Exception = exp}; };
					}
				}
				return this.launch(args);
			}
		}

		/// <summary>
		/// 游戏退出事件
		/// </summary>
		public event Action<LaunchHandle, int> GameExit;

		/// <summary>
		/// 游戏Log事件
		/// </summary>
		public event Action<LaunchHandle, string> GameLog;

		internal int currentCode;

		internal Random random = new Random();
	}

	/// <summary>
	///	启动器核心创建选项
	///	以后可能包含更多内容
	/// </summary>
	public class LauncherCoreCreationOption
	{
		/// <summary>
		/// 创建“创建选项”
		/// </summary>
		/// <param name="gameRootPath">游戏根目录</param>
		/// <param name="javaPath">JAVA目录</param>
		/// <returns></returns>
		public static LauncherCoreCreationOption Create(string gameRootPath = null, string javaPath = null)
		{
			gameRootPath = gameRootPath ?? ".minecraft";
			javaPath = javaPath ?? SystemTools.FindJava().FirstOrDefault();
			if (!Directory.Exists(gameRootPath)) { Directory.CreateDirectory(gameRootPath); }
			LauncherCoreCreationOption option = new LauncherCoreCreationOption();
			option.GameRootPath = new DirectoryInfo(gameRootPath).FullName;
			option.JavaPath = javaPath;
			return option;
		}

		/// <summary>
		/// 游戏根目录
		/// </summary>
		public string GameRootPath { get; internal set; }

		/// <summary>
		/// JAVA地址
		/// </summary>
		public string JavaPath { get; internal set; }
	}

	/// <summary>
	/// 启动后返回的启动结果
	/// </summary>
	public class LaunchResult
	{
		/// <summary>
		/// 获取是否启动成功
		/// </summary>
		public bool Success { get; set; }

		/// <summary>
		/// 获取发生的错误类型
		/// </summary>
		public ErrorType ErrorType { get; set; }

		/// <summary>
		/// 获取错误信息
		/// </summary>
		public string ErrorMessage { get; set; }

		/// <summary>
		/// 启动时发生异常
		/// </summary>
		public Exception Exception { get; set; }

		/// <summary>
		/// 获取启动句柄
		/// </summary>
		public LaunchHandle Handle { get; set; }
	}

	public enum ErrorType
	{
		/// <summary>
		/// 没有错误
		/// </summary>
		None,
		/// <summary>
		/// 没有找到JAVA
		/// </summary>
		NoJAVA,
		/// <summary>
		/// 验证失败
		/// </summary>
		AuthenticationFailed,
		/// <summary>
		/// 操作器出现故障
		/// </summary>
		OperatorException,
		/// <summary>
		/// 未知
		/// </summary>
		Unknown
	}
}
