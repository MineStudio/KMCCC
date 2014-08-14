using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMCCC.Authentication
{
	public class AuthenticationInfo
	{
		public String DisplayName { get; set; }

		public Guid UUID { get; set; }

		public Guid AccessToken { get; set; }

		public String Properties { get; set; }

		public String Error { get; set; }
	}
}
