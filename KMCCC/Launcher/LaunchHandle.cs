using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KMCCC.Authentication;

namespace KMCCC.Launcher
{
	public class LaunchOptions
	{
		public Int32 MaxMemory { get; set; }

		public Int32 MinMemory { get; set; }

		public Version Version { get; set; }

		public IAuthenticator Authenticator { get; set; }
	}
}
