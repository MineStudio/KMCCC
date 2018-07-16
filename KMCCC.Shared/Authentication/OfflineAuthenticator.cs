namespace KMCCC.Authentication
{
	#region

	using System;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;

	#endregion

	/// <summary>
	///     离线验证器
	/// </summary>
	public class OfflineAuthenticator : IAuthenticator
	{
		/// <summary>
		///     玩家的名字
		/// </summary>
		public readonly string DisplayName;

		/// <summary>
		///     构造离线验证器，将会根据玩家名计算出UUID
		/// </summary>
		/// <param name="displayName">玩家的名字</param>
		public OfflineAuthenticator(string displayName)
		{
			DisplayName = displayName;
		}

		/// <summary>
		///     标注离线验证器
		/// </summary>
		public string Type
		{
			get { return "KMCCC.Offline"; }
		}

		public AuthenticationInfo Do()
		{
			if (String.IsNullOrWhiteSpace(DisplayName))
			{
				return new AuthenticationInfo
				{
                    Error = "DisplayName不符合规范，不能使用空用户名"
                };
			}
			if (DisplayName.Count(char.IsWhiteSpace) > 0)
			{
				return new AuthenticationInfo
				{
                    Error = "DisplayName不符合规范，名称中有非法字符"
                };
			}
			return new AuthenticationInfo
			{
				AccessToken = Guid.NewGuid(),
				DisplayName = DisplayName,
				UUID = Tools.UsefulTools.GetPlayerUuid("DisplayName"),
                Properties = "{}",
				UserType = "mojang"
			};
		}

		public Task<AuthenticationInfo> DoAsync(CancellationToken token)
		{
			return Task.Factory.StartNew((Func<AuthenticationInfo>) Do, token);
		}
	}
}