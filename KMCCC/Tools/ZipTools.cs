namespace KMCCC.Tools
{
	#region

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.IO.Packaging;
	using System.Linq;
	using System.Reflection;

	#endregion

	/// <summary>
	///     操蛋的通过反射调用Zip解压
	///     Notice: 文件名只支持ASCII
	/// </summary>
	public static class ZipTools
	{
		public static readonly Boolean Enabled;

		public static readonly Type ZipArchive;

		public static readonly MethodInfo ZipArchive_OpenOnFile;

		public static readonly MethodInfo ZipArchive_GetFiles;

		public static readonly MethodInfo ZipArchive_Close;

		public static readonly Type ZipFileInfo;

		public static readonly MethodInfo ZipFileInfo_GetStream;

		public static readonly PropertyInfo ZipFileInfo_Name;

		public static readonly PropertyInfo ZipFileInfo_FolderFlag;

		static ZipTools()
		{
			try
			{
				var windowsBase = typeof (Package).Assembly;
				ZipArchive = windowsBase.GetType("MS.Internal.IO.Zip.ZipArchive");
				ZipArchive_OpenOnFile = ZipArchive.GetMethod("OpenOnFile", BindingFlags.NonPublic | BindingFlags.Static);
				ZipArchive_GetFiles = ZipArchive.GetMethod("GetFiles", BindingFlags.NonPublic | BindingFlags.Instance);
				ZipArchive_Close = ZipArchive.GetMethod("Close", BindingFlags.NonPublic | BindingFlags.Instance);

				ZipFileInfo = windowsBase.GetType("MS.Internal.IO.Zip.ZipFileInfo");
				ZipFileInfo_GetStream = ZipFileInfo.GetMethod("GetStream", BindingFlags.NonPublic | BindingFlags.Instance);
				ZipFileInfo_Name = ZipFileInfo.GetProperty("Name", BindingFlags.NonPublic | BindingFlags.Instance);
				ZipFileInfo_FolderFlag = ZipFileInfo.GetProperty("FolderFlag", BindingFlags.NonPublic | BindingFlags.Instance);
				Enabled = true;
			}
			catch
			{
				Enabled = false;
			}
		}

		public static bool Unzip(string zipFile, string outputDirectory, UnzipOptions options)
		{
			if (options == null)
			{
				return false;
			}
			try
			{
				var root = new DirectoryInfo(outputDirectory);
				root.Create();
				var rootPath = root.FullName + "/";
				var zip = ZipArchive_OpenOnFile.Invoke(null, new object[] {zipFile, FileMode.Open, FileAccess.Read, FileShare.Read, false});
				var files = (IEnumerable) ZipArchive_GetFiles.Invoke(zip, new object[] {});
				IEnumerable<string> exclude = (options.Exclude ?? new List<string>());
				if (exclude.Count() > 1000)
				{
					exclude = exclude.AsParallel();
				}
				foreach (var item in files)
				{
					var name = (string) ZipFileInfo_Name.GetValue(item, null);
					if (exclude.Any(name.StartsWith))
					{
						continue;
					}
					if ((bool) ZipFileInfo_FolderFlag.GetValue(item, null))
					{
						Directory.CreateDirectory(rootPath + name);
						continue;
					}
					using (var stream = (Stream) ZipFileInfo_GetStream.Invoke(item, new object[] {FileMode.Open, FileAccess.Read}))
					{
						var filePath = rootPath + name;
						var directoryInfo = new FileInfo(filePath).Directory;
						if (directoryInfo != null) directoryInfo.Create();
						using (var fs = new FileStream(filePath, FileMode.Create))
						{
							stream.CopyTo(fs);
						}
					}
				}
				ZipArchive_Close.Invoke(zip, new object[] {});
				return true;
			}
			catch
			{
				return false;
			}
		}
	}

	public class UnzipOptions
	{
		public List<string> Exclude { get; set; }
	}
}