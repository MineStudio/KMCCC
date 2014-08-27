using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO.Compression;
using System.IO;
using System.IO.Packaging;
using System.Reflection;
using System.Collections;
using System.Threading.Tasks;

namespace KMCCC.Tools
{
	public static class ZipTools
	{
		static ZipTools()
		{
			try
			{
				Package F__kingLink = null;
				var windowsBase = AppDomain.CurrentDomain.GetAssemblies().Single(item => item.FullName.StartsWith("WindowsBase,"));
				ZipArchive = windowsBase.GetType("MS.Internal.IO.Zip.ZipArchive");
				ZipArchive_OpenOnFile = ZipArchive.GetMethod("OpenOnFile", BindingFlags.NonPublic | BindingFlags.Static);
				ZipArchive_GetFiles = ZipArchive.GetMethod("GetFiles", BindingFlags.NonPublic | BindingFlags.Instance);

				ZipFileInfo = windowsBase.GetType("MS.Internal.IO.Zip.ZipFileInfo");
				ZipFileInfo_GetStream = ZipFileInfo.GetMethod("GetStream", BindingFlags.NonPublic | BindingFlags.Instance);
				ZipFileInfo_Name = ZipFileInfo.GetProperty("Name", BindingFlags.NonPublic | BindingFlags.Instance);
				Enabled = true;
			}
			catch { Enabled = false; }
		}

		public static readonly Boolean Enabled;

		public static readonly Type ZipArchive;

		public static readonly MethodInfo ZipArchive_OpenOnFile;

		public static readonly MethodInfo ZipArchive_GetFiles;

		public static readonly Type ZipFileInfo;

		public static readonly MethodInfo ZipFileInfo_GetStream;

		public static readonly PropertyInfo ZipFileInfo_Name;

		public static bool Unzip(String zipFile, String outputDirectory, UnzipOptions options)
		{
			if (options == null) { return false; }
			try
			{
				var root = new DirectoryInfo(outputDirectory);
				root.Create();
				var zip = ZipArchive_OpenOnFile.Invoke(null, new object[] { zipFile, FileMode.Open, FileAccess.Read, FileShare.Read, false });
				IEnumerable files = (IEnumerable)ZipArchive_GetFiles.Invoke(zip, new object[] { });
				IEnumerable<String> exclude = (options.Exclude == null ? new List<String>() : options.Exclude);
				if (exclude.Count() > 1000) { exclude = exclude.AsParallel(); }
				foreach (var item in files)
				{
					String name = (String)ZipFileInfo_Name.GetValue(item, null);
					if (exclude.Any(ex => name.StartsWith(ex)))
					{
						continue;
					}
					using (Stream stream = (Stream)ZipFileInfo_GetStream.Invoke(item, new object[] { FileMode.Open, FileAccess.Read }))
					{
						String filePath = root.FullName + "/" + name;
						new FileInfo(filePath).Directory.Create();
						using (var fs = File.OpenWrite(filePath))
						{
							stream.CopyTo(fs);
						}
					}
				}
				return true;
			}
			catch
			{
				return false;
			}
		}

		private static string CreateFilenameFromUri(Uri uri)
		{
			char[] invalidChars = Path.GetInvalidFileNameChars();
			StringBuilder sb = new StringBuilder(uri.OriginalString.Length);
			foreach (char c in uri.OriginalString)
			{
				sb.Append(Array.IndexOf(invalidChars, c) < 0 ? c : '_');
			}
			return sb.ToString();

		}

	}

	public class UnzipOptions
	{
		public List<String> Exclude { get; set; }
	}

}