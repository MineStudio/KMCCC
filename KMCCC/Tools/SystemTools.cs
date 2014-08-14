using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMCCC.Tools
{
	class SystemTools
	{

		public static String GetArch()
		{
			return Environment.Is64BitOperatingSystem ? "64" : "32";
		}
	}
}
