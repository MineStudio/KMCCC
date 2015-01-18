namespace KMCCC.Launcher
{
	#region

	using System;

	#endregion

	/// <summary>
	///     启动模式
	/// </summary>
	public abstract class LaunchMode
	{
		public static readonly BmclLaunchMode BmclMode = new BmclLaunchMode();

		public static readonly MCLauncherMode MCLauncher = new MCLauncherMode();

		/// <summary>
		///     启动模式
		/// </summary>
		/// <returns>模式是否应用成功</returns>
		public abstract bool Operate(LauncherCore core, MinecraftLaunchArguments args);
	}

	/// <summary>
	///     模仿BMCL的启动模式
	/// </summary>
	public class BmclLaunchMode : LaunchMode
	{
		public override bool Operate(LauncherCore core, MinecraftLaunchArguments args)
		{
			core.CopyVersionDirectory("mods", args.Version.Id);
			core.CopyVersionDirectory("coremods", args.Version.Id);
			core.CopyVersionDirectories(core.GetVersionRootPath(args.Version));
			return true;
		}
	}

	/// <summary>
	///     模仿MCLauncher的启动模式
	/// </summary>
	public class MCLauncherMode : LaunchMode
	{
		public override bool Operate(LauncherCore core, MinecraftLaunchArguments args)
		{
			args.Tokens["game_directory"] = String.Format(@".\versions\{0}\", args.Version.Id);
			return true;
		}
	}

	/// <summary>
	///     简单的映射启动模式
	/// </summary>
	public class SimpleWarpedMode : LaunchMode
	{
		private readonly Func<LauncherCore, MinecraftLaunchArguments, bool> _operatorMethod;

		public SimpleWarpedMode(Func<LauncherCore, MinecraftLaunchArguments, bool> operatorMethod)
		{
			_operatorMethod = operatorMethod;
		}

		public override bool Operate(LauncherCore core, MinecraftLaunchArguments args)
		{
			return _operatorMethod.Invoke(core, args);
		}
	}
}