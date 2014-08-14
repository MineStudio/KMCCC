using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMCCC
{
	public static class SafeConvertExtensions
	{
		public static DateTime AsDateTime(this String str)
		{
			DateTime time;
			DateTime.TryParse(str, out time);
			return time;
		}
	}

}
