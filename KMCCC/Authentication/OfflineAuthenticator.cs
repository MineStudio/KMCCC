using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMCCC.Authentication
{
	public class OfflineAuthenticator : IAuthenticator
	{
		public readonly String DisplayName;

		public OfflineAuthenticator(String displayName)
		{
			this.DisplayName = displayName;
		}

		public AuthenticationInfo Do()
		{
			if (String.IsNullOrWhiteSpace(DisplayName))
			{
				return new AuthenticationInfo { Error = "DisplayName不符合规范" };
			}
			if (DisplayName.Count(c => char.IsWhiteSpace(c)) > 0)
			{
				return new AuthenticationInfo { Error = "DisplayName不符合规范" };
			}
			return new AuthenticationInfo { AccessToken = Guid.NewGuid(), DisplayName = DisplayName, UUID = Guid.NewGuid(), Properties = "{}" };
		}

		public Task<AuthenticationInfo> DoAsync()
		{
			return Task.Factory.StartNew((Func<AuthenticationInfo>)this.Do);
		}
	}
}
