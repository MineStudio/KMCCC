using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Reflection;
using System.Text;
using KMCCC.Tools;

namespace KMCCC.Modules.Zip
{
	internal static class Reflection
	{
		public static readonly bool Enabled;

		public static readonly Type ZipArchive;

		public static readonly MethodInfo ZipArchive_OpenOnStream;

		public static readonly MethodInfo ZipArchive_GetFile;

		public static readonly MethodInfo ZipArchive_GetFiles;

		public static readonly FieldInfo ZipArchive_ZipIOBlockManager;

		public static readonly Type ZipFileInfo;

		public static readonly MethodInfo ZipFileInfo_GetStream;

		public static readonly PropertyInfo ZipFileInfo_Name;

		public static readonly PropertyInfo ZipFileInfo_FolderFlag;

		public static readonly Type ZipIOBlockManager;

		public static readonly FieldInfo ZipIOBlockManager_Encoding;

		static Reflection()
		{
			try
			{
				var windowsBase = typeof (Package).Assembly;
				ZipArchive = windowsBase.GetType("MS.Internal.IO.Zip.ZipArchive");
				ZipArchive_OpenOnStream = ZipArchive.GetMethod("OpenOnStream", BindingFlags.NonPublic | BindingFlags.Static);
				ZipArchive_GetFile = ZipArchive.GetMethod("GetFile", BindingFlags.NonPublic | BindingFlags.Instance);
				ZipArchive_GetFiles = ZipArchive.GetMethod("GetFiles", BindingFlags.NonPublic | BindingFlags.Instance);
				ZipArchive_ZipIOBlockManager = ZipArchive.GetField("_blockManager", BindingFlags.NonPublic | BindingFlags.Instance);

				ZipFileInfo = windowsBase.GetType("MS.Internal.IO.Zip.ZipFileInfo");
				ZipFileInfo_GetStream = ZipFileInfo.GetMethod("GetStream", BindingFlags.NonPublic | BindingFlags.Instance);
				ZipFileInfo_Name = ZipFileInfo.GetProperty("Name", BindingFlags.NonPublic | BindingFlags.Instance);
				ZipFileInfo_FolderFlag = ZipFileInfo.GetProperty("FolderFlag", BindingFlags.NonPublic | BindingFlags.Instance);

				ZipIOBlockManager = windowsBase.GetType("MS.Internal.IO.Zip.ZipIOBlockManager");
				ZipIOBlockManager_Encoding = ZipIOBlockManager.GetField("_encoding", BindingFlags.NonPublic | BindingFlags.Instance);

				Enabled = true;
			}
			catch
			{
				Enabled = false;
			}
		}
	}

	public class ZipArchive : IDisposable
	{
		private readonly IDisposable _realArchive;

		private ZipArchive(IDisposable obj)
		{
			_realArchive = obj;
		}

		public ZipFileInfo this[string name] => GetFile(name);

		public void Dispose()
		{
			_realArchive?.Dispose();
		}

		public ZipFileInfo GetFile(string name)
		{
			return new ZipFileInfo(Reflection.ZipArchive_GetFile.Invoke(_realArchive, new[] {name}));
		}

		public IEnumerable<ZipFileInfo> GetFiles()
		{
			var enumerable = (IEnumerable) Reflection.ZipArchive_GetFiles.Invoke(_realArchive, new object[0]);
			var enumerator = enumerable.GetEnumerator();
			while (enumerator.MoveNext())
			{
				yield return new ZipFileInfo(enumerator.Current);
			}
		}

		public static ZipArchive OpenZipFile(Stream zipFile, Encoding encoding)
		{
			var zip = Reflection.ZipArchive_OpenOnStream.Invoke(null,
				new object[] {zipFile, FileMode.Open, FileAccess.Read, false});

			var ioManager = Reflection.ZipArchive_ZipIOBlockManager.GetValue(zip);
			Reflection.ZipIOBlockManager_Encoding.SetValue(ioManager, new ZipTools.WarpedEncoding(encoding));

			return new ZipArchive((IDisposable) zip);
		}
	}

	public class ZipFileInfo
	{
		private readonly object _realFileInfo;

		internal ZipFileInfo(object obj)
		{
			_realFileInfo = obj;
		}

		public bool FolderFlag => (bool) Reflection.ZipFileInfo_FolderFlag.GetValue(_realFileInfo, new object[0]);

		public string Name => (string) Reflection.ZipFileInfo_Name.GetValue(_realFileInfo, new object[0]);

		public Stream GetStream()
		{
			return (Stream) Reflection.ZipFileInfo_GetStream.Invoke(_realFileInfo, new object[] {FileMode.Open, FileAccess.Read});
		}
	}
}