namespace KMCCC.Launcher
{
	#region

	using System;
	using System.Collections.Generic;
	using Tools;

	#endregion

	/// <summary>
	///     版本定位器接口
	/// </summary>
	public interface IVersionLocator
	{
		/// <summary>
		///     设置定位器基于的核心
		/// </summary>
		LauncherCore Core { set; }

		/// <summary>
		///     获取对应Id的Version，若不存在应返回null
		/// </summary>
		/// <param name="versionId">对应的Id</param>
		/// <returns>对应的Version</returns>
		Version Locate(string versionId);

		/// <summary>
		///     获取所有可找到Version
		/// </summary>
		/// <returns>所有Version</returns>
		IEnumerable<Version> GetAllVersions();
	}

	/// <summary>
	///     表示版本
	/// </summary>
	public sealed class Version
	{
		/// <summary>
		///     ID
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		///     主启动参数
		/// </summary>
		public string MinecraftArguments { get; set; }

		/// <summary>
		///     资源名
		/// </summary>
		public string Assets { get; set; }

		/// <summary>
		///     主类
		/// </summary>
		public string MainClass { get; set; }

		/// <summary>
		///     库列表
		/// </summary>
		public List<Library> Libraries { get; set; }

		/// <summary>
		///     本地实现表
		/// </summary>
		public List<Native> Natives { get; set; }

		/// <summary>
		///     Jar文件（Id）
		/// </summary>
		public string JarId { get; set; }
	}

	/// <summary>
	///     表示库
	/// </summary>
	public class Library
	{
		/// <summary>
		///     NS
		/// </summary>
		public string NS { get; set; }

		/// <summary>
		///     Name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		///     Version
		/// </summary>
		public string Version { get; set; }
	}

	/// <summary>
	///     表示本机实现
	/// </summary>
	public class Native
	{
		/// <summary>
		///     NS
		/// </summary>
		public string NS { get; set; }

		/// <summary>
		///     Name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		///     Version
		/// </summary>
		public string Version { get; set; }

		/// <summary>
		///     本机实现后缀
		/// </summary>
		public string NativeSuffix { get; set; }

		/// <summary>
		///     解压参数
		/// </summary>
		public UnzipOptions Options { get; set; }
	}

	/// <summary>
	///     找Item，自己看我不加注释了
	/// </summary>
	public static class LauncherCoreItemResolverExtensions
	{

		public static string GetVersionRootPath(this LauncherCore core, Version version)
		{
			return GetVersionRootPath(core, version.Id);
		}

		public static string GetVersionRootPath(this LauncherCore core, string versionId)
		{
			return String.Format(@"{0}\versions\{1}\", core.GameRootPath, versionId);
		}

		public static string GetVersionJarPath(this LauncherCore core, Version version)
		{
			return GetVersionJarPath(core, version.Id);
		}

		public static string GetVersionJarPath(this LauncherCore core, string versionId)
		{
			return String.Format(@"{0}\versions\{1}\{1}.jar", core.GameRootPath, versionId);
		}

		public static string GetVersionJsonPath(this LauncherCore core, Version version)
		{
			return GetVersionJsonPath(core, version.Id);
		}

		public static string GetVersionJsonPath(this LauncherCore core, string versionId)
		{
			return String.Format(@"{0}\versions\{1}\{1}.json", core.GameRootPath, versionId);
		}

		public static string GetLibPath(this LauncherCore core, Library lib)
		{
			return String.Format(@"{0}\libraries\{1}\{2}\{3}\{2}-{3}.jar", core.GameRootPath, lib.NS.Replace(".", "\\"), lib.Name, lib.Version);
		}

		public static string GetNativePath(this LauncherCore core, Native native)
		{
			return String.Format(@"{0}\libraries\{1}\{2}\{3}\{2}-{3}-{4}.jar", core.GameRootPath, native.NS.Replace(".", "\\"), native.Name, native.Version,
				native.NativeSuffix);
		}
	}
}