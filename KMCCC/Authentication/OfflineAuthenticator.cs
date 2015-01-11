namespace KMCCC.Authentication
{
	#region

	using System;
	using System.Linq;
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
		///     构造离线验证器
		/// </summary>
		/// <param name="displayName">玩家的名字</param>
		public OfflineAuthenticator(string displayName)
		{
			DisplayName = displayName;
		}

		public AuthenticationInfo Do()
		{
			if (String.IsNullOrWhiteSpace(DisplayName))
			{
				return new AuthenticationInfo {Error = "DisplayName不符合规范"};
			}
			if (DisplayName.Count(char.IsWhiteSpace) > 0)
			{
				return new AuthenticationInfo {Error = "DisplayName不符合规范"};
			}
			return new AuthenticationInfo
			{
				AccessToken = Guid.NewGuid(),
				DisplayName = DisplayName,
				UUID = Guid.NewGuid(),
				Properties = "{}",
				UserType = "Mojang"
			};
		}

		public Task<AuthenticationInfo> DoAsync()
		{
			return Task.Factory.StartNew((Func<AuthenticationInfo>) Do);
		}
	}
}