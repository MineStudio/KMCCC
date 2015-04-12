namespace KMCCC.Authentication
{
	#region

	using System;
	using System.Threading;
	using System.Threading.Tasks;
	using Modules.Yggdrasil;

	#endregion

	/// <summary>
	///     正版验证器（直接登陆）
	/// </summary>
	public class YggdrasilLogin : IAuthenticator
	{
		/// <summary>
		///     新建正版验证器
		/// </summary>
		/// <param name="email">电子邮件地址</param>
		/// <param name="password">密码</param>
		/// <param name="twitchEnabled">是否启用Twitch</param>
		/// <param name="clientToken">clientToken</param>
		public YggdrasilLogin(string email, string password, bool twitchEnabled, Guid clientToken)
		{
			Email = email;
			Password = password;
			TwitchEnabled = twitchEnabled;
			ClientToken = clientToken;
		}

		/// <summary>
		///     新建正版验证器(随机的新ClientToken)
		/// </summary>
		/// <param name="email">电子邮件地址</param>
		/// <param name="password">密码</param>
		/// <param name="twitchEnabled">是否启用Twitch</param>
		public YggdrasilLogin(string email, string password, bool twitchEnabled) : this(email, password, twitchEnabled, Guid.NewGuid())
		{
		}

		/// <summary>
		///     电子邮件地址
		/// </summary>
		public string Email { get; private set; }

		/// <summary>
		///     密码
		/// </summary>
		public string Password { get; private set; }

		/// <summary>
		///     是否启用Twitch
		/// </summary>
		public bool TwitchEnabled { get; private set; }

		/// <summary>
		/// </summary>
		public Guid ClientToken { get; private set; }

		/// <summary>
		///     返回Yggdrasil验证器类型
		/// </summary>
		public string Type
		{
			get { return "KMCCC.Yggdrasil"; }
		}

		public AuthenticationInfo Do()
		{
			var client = new YggdrasilClient(ClientToken);
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
			return new AuthenticationInfo
			{
				Error = "验证错误"
			};
		}

		public Task<AuthenticationInfo> DoAsync(CancellationToken token)
		{
			var client = new YggdrasilClient(ClientToken);
			return client.AuthenticateAsync(Email, Password, TwitchEnabled, token).ContinueWith(task =>
			{
				if ((task.Exception==null)&&(task.Result))
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
				return new AuthenticationInfo
				{
					Error = "验证错误"
				};
			}, token);
		}
	}
}