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
	using System.Text;

	#endregion

	/// <summary>
	///     操蛋的通过反射调用Zip解压
	/// </summary>
	public static class ZipTools
	{
		public static readonly Boolean Enabled;

		public static readonly Type ZipArchive;

		public static readonly MethodInfo ZipArchive_OpenOnFile;

		public static readonly MethodInfo ZipArchive_GetFiles;

		public static readonly FieldInfo ZipArchive_ZipIOBlockManager;

		public static readonly Type ZipFileInfo;

		public static readonly MethodInfo ZipFileInfo_GetStream;

		public static readonly PropertyInfo ZipFileInfo_Name;

		public static readonly PropertyInfo ZipFileInfo_FolderFlag;

		public static readonly Type ZipIOBlockManager;

		public static readonly FieldInfo ZipIOBlockManager_Encoding;

		static ZipTools()
		{
			try
			{
				var windowsBase = typeof (Package).Assembly;
				ZipArchive = windowsBase.GetType("MS.Internal.IO.Zip.ZipArchive");
				ZipArchive_OpenOnFile = ZipArchive.GetMethod("OpenOnFile", BindingFlags.NonPublic | BindingFlags.Static);
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

		public static bool Unzip(string zipFile, string outputDirectory, UnzipOptions options)
		{
			return UnzipFile(zipFile, outputDirectory, options) == null;
		}

		public static Exception UnzipFile(string zipFile, string outputDirectory, UnzipOptions options)
		{
			options = options ?? new UnzipOptions();
			try
			{
				var root = new DirectoryInfo(outputDirectory);
				root.Create();
				var rootPath = root.FullName + "/";
				using (var zip = (IDisposable) ZipArchive_OpenOnFile.Invoke(null, new object[] {zipFile, FileMode.Open, FileAccess.Read, FileShare.Read, false}))
				{
					var ioManager = ZipArchive_ZipIOBlockManager.GetValue(zip);
					ZipIOBlockManager_Encoding.SetValue(ioManager, new WarpedEncoding(options.Encoding ?? Encoding.Default));

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
						if ((bool) ZipFileInfo_FolderFlag.GetValue(item, null) || name.Last() == '/')
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
				}
				return null;
			}
			catch (Exception exp)
			{
				return exp;
			}
		}

		#region 编码大法

		public class WarpedEncoding : ASCIIEncoding
		{
			private readonly Encoding _innerEncoding;

			public WarpedEncoding(Encoding encoding)
			{
				_innerEncoding = encoding ?? Default;
			}

			public override bool Equals(object value)
			{
				return _innerEncoding.Equals(value);
			}

			public override int GetByteCount(char[] chars, int index, int count)
			{
				return _innerEncoding.GetByteCount(chars, index, count);
			}

			public override int GetByteCount(string chars)
			{
				return _innerEncoding.GetByteCount(chars);
			}

			public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
			{
				return _innerEncoding.GetBytes(chars, charIndex, charCount, bytes, byteIndex);
			}

			public override int GetBytes(string s, int charIndex, int charCount, byte[] bytes, int byteIndex)
			{
				return _innerEncoding.GetBytes(s, charIndex, charCount, bytes, byteIndex);
			}

			public override int GetCharCount(byte[] bytes, int index, int count)
			{
				return _innerEncoding.GetCharCount(bytes, index, count);
			}

			public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
			{
				return _innerEncoding.GetChars(bytes, byteIndex, byteCount, chars, charIndex);
			}

			public override Decoder GetDecoder()
			{
				return _innerEncoding.GetDecoder();
			}

			public override Encoder GetEncoder()
			{
				return _innerEncoding.GetEncoder();
			}

			public override int GetHashCode()
			{
				return _innerEncoding.GetHashCode();
			}

			public override int GetMaxByteCount(int charCount)
			{
				return _innerEncoding.GetMaxByteCount(charCount);
			}

			public override int GetMaxCharCount(int byteCount)
			{
				return _innerEncoding.GetMaxCharCount(byteCount);
			}

			public override byte[] GetPreamble()
			{
				return _innerEncoding.GetPreamble();
			}

			public override string GetString(byte[] bytes, int index, int count)
			{
				return _innerEncoding.GetString(bytes, index, count);
			}
		}

		#endregion
	}


	/// <summary>
	///     解压选项
	/// </summary>
	public class UnzipOptions
	{
		/// <summary>
		///     排除的文件（夹）
		/// </summary>
		public List<string> Exclude { get; set; }

		/// <summary>
		///     文件（夹）名使用的编码
		/// </summary>
		public Encoding Encoding { get; set; }
	}
}