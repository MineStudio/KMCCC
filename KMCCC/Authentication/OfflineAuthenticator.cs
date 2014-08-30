using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMCCC.Authentication
{
	/// <summary>
	/// 离线验证器
	/// </summary>
	public class OfflineAuthenticator : IAuthenticator
	{
		/// <summary>
		/// 玩家的名字
		/// </summary>
		public readonly String DisplayName;

		/// <summary>
		/// 构造离线验证器
		/// </summary>
		/// <param name="displayName">玩家的名字</param>
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
			return new AuthenticationInfo { AccessToken = Guid.NewGuid(), DisplayName = DisplayName, UUID = Guid.NewGuid(), Properties = "{}", UserType = "Mojang" };
		}

		public Task<AuthenticationInfo> DoAsync()
		{
			return Task.Factory.StartNew((Func<AuthenticationInfo>)this.Do);
		}
	}
}
