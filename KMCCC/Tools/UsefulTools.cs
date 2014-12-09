using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace KMCCC.Tools
{
	/// <summary>
	/// 有用的东西
	/// </summary>
	public static class UsefulTools
	{
		public static string DoReplace(this string source, IDictionary<string, string> dic)
		{
			foreach (var pair in dic)
			{
				source = source.Replace("${" + pair.Key + "}", pair.Value);
			}
			return source;
		}

		public static string GoString(this Guid guid)
		{
			return guid.ToString().Replace("-", "");
		}

		public static void Dircopy(string source, string target)
		{
			DirectoryInfo sourceDir = new DirectoryInfo(source);
			if (!Directory.Exists(target))
			{
				Directory.CreateDirectory(target);
			}
			foreach (FileInfo file in sourceDir.GetFiles())
			{
				File.Copy(file.FullName, target + "\\" + file.Name, true);
			}
			foreach (DirectoryInfo subdir in sourceDir.GetDirectories())
			{
				Dircopy(subdir.FullName, target + "\\" + subdir.Name);
			}
		}

	}
}
