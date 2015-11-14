using System;
using System.Threading;
using System.Threading.Tasks;
using KMCCC.Modules.Yggdrasil;

namespace KMCCC.Authentication
{

	#region Login

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
		/// <param name="authServer">验证服务器</param>
		public YggdrasilLogin(string email, string password, bool twitchEnabled, Guid clientToken, string authServer = null)
		{
			Email = email;
			Password = password;
			TwitchEnabled = twitchEnabled;
			ClientToken = clientToken;
			AuthServer = authServer;
		}

		/// <summary>
		///     新建正版验证器(随机的新ClientToken)
		/// </summary>
		/// <param name="email">电子邮件地址</param>
		/// <param name="password">密码</param>
		/// <param name="twitchEnabled">是否启用Twitch</param>
		/// <param name="authServer">验证服务器</param>
		public YggdrasilLogin(string email, string password, bool twitchEnabled, string authServer = null)
			: this(email, password, twitchEnabled, Guid.NewGuid(), authServer)
		{
		}

		/// <summary>
		///     电子邮件地址
		/// </summary>
		public string Email { get; }

		/// <summary>
		///     密码
		/// </summary>
		public string Password { get; }

		/// <summary>
		///     是否启用Twitch
		/// </summary>
		public bool TwitchEnabled { get; }

		/// <summary>
		/// </summary>
		public Guid ClientToken { get; }

		/// <summary>
		/// </summary>
		public string AuthServer { get; set; }

		/// <summary>
		///     返回Yggdrasil验证器类型
		/// </summary>
		public string Type => "KMCCC.Yggdrasil";

		public AuthenticationInfo Do()
		{
			var client = new YggdrasilClient(AuthServer, ClientToken);
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
			var client = new YggdrasilClient(AuthServer, ClientToken);
			return client.AuthenticateAsync(Email, Password, TwitchEnabled, token).ContinueWith(task =>
			{
				if ((task.Exception == null) && (task.Result))
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

	#endregion

	#region Refresh

	/// <summary>
	///     正版验证器（直接登陆）
	/// </summary>
	public class YggdrasilRefresh : IAuthenticator
	{
		/// <summary>
		///     新建正版验证器
		/// </summary>
		/// <param name="accessToken">合法的Token</param>
		/// <param name="twitchEnabled">是否启用Twitch</param>
		/// <param name="clientToken">clientToken</param>
		public YggdrasilRefresh(Guid accessToken, bool twitchEnabled, Guid clientToken, string authServer = null)
		{
			AccessToken = accessToken;
			TwitchEnabled = twitchEnabled;
			ClientToken = clientToken;
			AuthServer = authServer;
		}

		/// <summary>
		///     新建正版验证器(随机的新ClientToken)
		/// </summary>
		/// <param name="accessToken">合法的Token</param>
		/// <param name="twitchEnabled">是否启用Twitch</param>
		public YggdrasilRefresh(Guid accessToken, bool twitchEnabled, string authServer = null)
			: this(accessToken, twitchEnabled, Guid.NewGuid(), authServer)
		{
		}

		public Guid AccessToken { get; }

		/// <summary>
		///     是否启用Twitch
		/// </summary>
		public bool TwitchEnabled { get; }

		/// <summary>
		/// </summary>
		public Guid ClientToken { get; }

		/// <summary>
		/// </summary>
		public string AuthServer { get; set; }

		/// <summary>
		///     返回Yggdrasil验证器类型
		/// </summary>
		public string Type => "KMCCC.Yggdrasil";

		public AuthenticationInfo Do()
		{
			var client = new YggdrasilClient(AuthServer, ClientToken);
			if (client.Refresh(AccessToken, TwitchEnabled))
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
			return Task<AuthenticationInfo>.Factory.StartNew(Do, token);
		}
	}

	#endregion
}