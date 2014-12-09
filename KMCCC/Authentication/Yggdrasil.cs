using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KMCCC.Yggdrasil;

namespace KMCCC.Authentication
{
	/// <summary>
	/// 正版验证器（直接登陆）
	/// </summary>
	public class YggdrasilLogin : IAuthenticator
	{
		/// <summary>
		/// 电子邮件地址
		/// </summary>
		public string Email { get; private set; }

		/// <summary>
		/// 密码
		/// </summary>
		public string Password { get; private set; }

		/// <summary>
		/// 是否启用Twitch
		/// </summary>
		public bool TwitchEnabled { get; private set; }

		/// <summary>
		/// 新建正版验证器
		/// </summary>
		/// <param name="email">电子邮件地址</param>
		/// <param name="password">密码</param>
		/// <param name="twitchEnabled">是否启用Twitch</param>
		public YggdrasilLogin(string email, string password, bool twitchEnabled)
		{
			this.Email = email;
			this.Password = password;
			this.TwitchEnabled = twitchEnabled;
		}

		public AuthenticationInfo Do()
		{
			YggdrasilClient client = new YggdrasilClient();
			if (client.Authenticate(Email, Password, TwitchEnabled))
			{
				return new AuthenticationInfo
				{
					AccessToken = client.AccessToken,
					UserType = client.AccountType,
					DisplayName = client.DisplayName,
					Properties = client.Properties,
					UUID = client.UUID
				};
			}
			else
			{
				return new AuthenticationInfo { Error = "验证错误" };
			}
		}

		public Task<AuthenticationInfo> DoAsync()
		{
			return Task<AuthenticationInfo>.Factory.StartNew(Do); 
		}
	}
}
