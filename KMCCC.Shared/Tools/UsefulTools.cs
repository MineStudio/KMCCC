namespace KMCCC.Tools
{
	#region

	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;

	#endregion

	/// <summary>
	///     有用的东西
	/// </summary>
	public static class UsefulTools
	{
		public static string DoReplace(this string source, IDictionary<string, string> dic)
		{
			return dic.Aggregate(source, (current, pair) => current.Replace("${" + pair.Key + "}", pair.Value));
		}

		public static string GoString(this Guid guid)
		{
			return guid.ToString().Replace("-", "");
		}

		public static void Dircopy(string source, string target)
		{
			var sourceDir = new DirectoryInfo(source);
			if (!Directory.Exists(target))
			{
				Directory.CreateDirectory(target);
			}
			foreach (var file in sourceDir.GetFiles())
			{
				File.Copy(file.FullName, target + "\\" + file.Name, true);
			}
			foreach (var subdir in sourceDir.GetDirectories())
			{
				Dircopy(subdir.FullName, target + "\\" + subdir.Name);
			}
		}

#if DEBUG
		public static string Print(this string str)
		{
			Console.WriteLine(str);
			return str;
		}
#endif
	}
}