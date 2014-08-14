using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMCCC.Authentication
{
	public interface IAuthenticator
	{
		AuthenticationInfo Do();

		Task<AuthenticationInfo> DoAsync();
	}
}
