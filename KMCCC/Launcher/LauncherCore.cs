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

		private LauncherCore()
		{

		}

		/// <summary>
		/// 游戏根目录
		/// </summary>
		public String GameRootPath { get; private set; }

		/// <summary>
		/// JAVA目录
		/// </summary>
		public String JavaPath { get; set; }

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
		public Version GetVersion(String id)
		{
			lock (locker)
			{
				var versionPath = GameRootPath + @"\versions\" + id;
				if (!Directory.Exists(versionPath)) { return null; }
				versionPath = versionPath + '\\' + id + ".json";
				if (!File.Exists(versionPath)) { return null; }
				return GetVersionInternal(versionPath);
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
		/// <returns>启动句柄</returns>
		public LaunchHandle Launch(LaunchOptions options, params Action<MinecraftLaunchArguments>[] argumentsOperators)
		{
			lock (locker)
			{
				if (!File.Exists(JavaPath)) { return null; }
				currentCode = random.Next();
				var args = GenerateArguments(options);
				if (args == null) { return null; }
				if (argumentsOperators != null)
				{
					foreach (var opt in argumentsOperators)
					{
						if (opt != null)
						{
							opt(args);
						}
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
		public event Action<LaunchHandle, String> GameLog;

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
		/// <param name="GameRootPath">游戏根目录</param>
		/// <param name="JavaPath">JAVA目录</param>
		/// <returns></returns>
		public static LauncherCoreCreationOption Create(String GameRootPath = null, String JavaPath = null)
		{
			GameRootPath = GameRootPath ?? ".minecraft";
			JavaPath = JavaPath ?? SystemTools.FindJava();
			if (!Directory.Exists(GameRootPath)) { Directory.CreateDirectory(GameRootPath); }
			LauncherCoreCreationOption option = new LauncherCoreCreationOption();
			option.GameRootPath = new DirectoryInfo(GameRootPath).FullName;
			option.JavaPath = JavaPath;
			return option;
		}

		/// <summary>
		/// 游戏根目录
		/// </summary>
		public String GameRootPath { get; internal set; }

		/// <summary>
		/// JAVA地址
		/// </summary>
		public String JavaPath { get; internal set; }
	}
}
