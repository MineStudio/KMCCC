using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMCCC.Tools
{
	/// <summary>
	/// 有用的东西
	/// </summary>
	public static class UsefulTools
	{
		public static String DoReplace(this String source, IDictionary<String, String> dic)
		{
			foreach (var pair in dic)
			{
				source = source.Replace("${" + pair.Key + "}", pair.Value);
			}
			return source;
		}

		public static String GoString(this Guid guid)
		{
			return guid.ToString().Replace("-", "");
		}
	}
}
