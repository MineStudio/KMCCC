using System;
using System.Threading;
using System.Threading.Tasks;
using KMCCC.Modules.Yggdrasil;
using System.Collections.Generic;

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
        public YggdrasilLogin(string email, string password, bool twitchEnabled, Guid clientToken, string token = null, string authServer = null)
        {
            Email = email;
            Password = password;
            TwitchEnabled = twitchEnabled;
            ClientToken = clientToken;
            AuthServer = authServer;
            Token = token;
        }

        /// <summary>
        ///     新建正版验证器(随机的新ClientToken，如果要使用Vaildate，不推荐)
        /// </summary>
        /// <param name="email">电子邮件地址</param>
        /// <param name="password">密码</param>
        /// <param name="twitchEnabled">是否启用Twitch</param>
        /// <param name="authServer">验证服务器</param>
        public YggdrasilLogin(string email, string password, bool twitchEnabled, string token = null, string authServer = null)
        {
            Email = email;
            Password = password;
            TwitchEnabled = twitchEnabled;
            AuthServer = authServer;
            Token = token;
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
        ///     第三方服务器
        /// </summary>
        public string AuthServer { get; set; }

        /// <summary>
        ///     第三方验证服务器的一些验证Token（伪正版）
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        ///     返回Yggdrasil验证器类型
        /// </summary>
        public string Type => "KMCCC.Yggdrasil";

        public AuthenticationInfo Do()
        {
            var client = new YggdrasilClient(AuthServer, ClientToken);
            var LoginError = client.Authenticate(Email, Password, Token, TwitchEnabled);

            if (LoginError == null)
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
                Error = LoginError.Message
            };
        }

        public Task<AuthenticationInfo> DoAsync(CancellationToken token)
        {
            var client = new YggdrasilClient(AuthServer, ClientToken);
            return client.AuthenticateAsync(Email, Password, Token, TwitchEnabled, token).ContinueWith(task =>
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
                    Error = task.Exception.Message
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
            var LoginError = client.Refresh(AccessToken, TwitchEnabled);

            if (LoginError == null)
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
                Error = LoginError.Message
            };
        }

        public Task<AuthenticationInfo> DoAsync(CancellationToken token)
        {
            return Task<AuthenticationInfo>.Factory.StartNew(Do, token);
        }
    }

    #endregion

    #region Validate
    /// <summary>
    ///     正版验证器（验证AccessToken是否可用，不可用将尝试Refresh）
    /// </summary>
    public class YggdrasilValidate : IAuthenticator
    {
        /// <summary>
        ///     新建AccessToken验证器
        /// </summary>
        /// <param name="accessToken">合法的AccessToken</param>
        /// <param name="clientToken">获得AccessToken时设置的ClientToken</param>
        /// <param name="DisplayName">游戏名，如果设置错误将导致无法进入正版服务器</param>
        /// <param name="uuid">UUID，如果设置错误将导致无法进入正版服务器</param>
        /// <param name="authServer"></param>
        public YggdrasilValidate(Guid accessToken, Guid clientToken, Guid uuid, string displayName, string authServer = null)
        {
            AccessToken = accessToken;
            ClientToken = clientToken;
            DisplayName = displayName;
            UUID = uuid;
            AuthServer = authServer;
        }

        public string Type => "KMCCC.Yggdrasil";

        public Guid AccessToken { get; }

        /// <summary>
        /// </summary>
        public Guid ClientToken { get; }

        /// <summary>
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// </summary>
        public Guid UUID { get; }

        /// <summary>
        ///     手动设置验证服务器地址
        /// </summary>
        public string AuthServer { get; set; }

        public AuthenticationInfo Do()
        {
            var client = new YggdrasilClient(AuthServer, ClientToken);
            var LoginError = client.AuthToken(AccessToken, UUID, DisplayName);

            if (LoginError == null)
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
                Error = LoginError.Message
            };
        }

        public Task<AuthenticationInfo> DoAsync(CancellationToken token)
        {
            return Task<AuthenticationInfo>.Factory.StartNew(Do, token);
        }
    }
    #endregion

    #region AutoSelete (Unique skills!!!!!!)

    /// <summary>
    ///     超级无敌完美全方位正版验证器！验证顺序Validate > Refresh > Login，将无法使用Twitch登录
    ///     为提高验证速度，Token将为string类型，无需转换
    ///     请提供固定的ClientToken，推荐为每个客户端生成一个固定的ClientToken并保存在配置文件中
    /// </summary>
    public class YggdrasilAuto : IAuthenticator
    {
        public YggdrasilAuto(string email, string password, string accessToken, string clientToken, string uuid, string displayName, string authServer = null)
        {
            Email = email;
            Password = password;
            AccessToken = accessToken;
            ClientToken = clientToken;
            UUID = uuid;
            DisplayName = displayName;
            AuthServer = authServer;
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
        /// </summary>
        public string AccessToken { get; }

        /// <summary>
        /// </summary>
        public string ClientToken { get; }

        /// <summary>
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// </summary>
        public string UUID { get; }

        /// <summary>
        ///     第三方服务器
        /// </summary>
        public string AuthServer { get; set; }

        /// <summary>
        ///     返回Yggdrasil验证器类型
        /// </summary>
        public string Type => "KMCCC.Yggdrasil";

        private IEnumerable<IAuthenticator> TryQueue()
        {
            if (Guid.TryParse(AccessToken, out Guid at) && Guid.TryParse(ClientToken, out Guid ct))
            {
                if (Guid.TryParse(UUID, out Guid id))
                    yield return new YggdrasilValidate(at, ct, id, DisplayName, AuthServer);
                yield return new YggdrasilRefresh(at, false, ct, AuthServer);
            }
            yield return new YggdrasilLogin(Email, Password, false, null, AuthServer);
        }

        public AuthenticationInfo Do()
        {
            string ErrorMessage = "";
            foreach (IAuthenticator auth in TryQueue())
            {
                var AuthInfo = auth.Do();
                if (AuthInfo.Error == null)
                {
                    return new AuthenticationInfo()
                    {
                        AccessToken = AuthInfo.AccessToken,
                        DisplayName = AuthInfo.DisplayName,
                        UUID = AuthInfo.UUID,
                        Properties = AuthInfo.Properties,
                    };
                }
                else
                {
                    ErrorMessage = AuthInfo.Error;
                }
            }
            return new AuthenticationInfo()
            {
                Error = ErrorMessage
            };
        }

        public Task<AuthenticationInfo> DoAsync(CancellationToken token)
        {
            return Task<AuthenticationInfo>.Factory.StartNew(Do, token);
        }
    }
    #endregion
}