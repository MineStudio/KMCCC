namespace KMCCC.Authentication
{
	#region

	using System;
	using System.Collections.Generic;

	#endregion

	/// <summary>
	///     表示验证信息
	/// </summary>
	public class AuthenticationInfo
	{
		/// <summary>
		///     玩家的名字
		/// </summary>
		public string DisplayName { get; set; }

		/// <summary>
		///     UUID不解释
		/// </summary>
		public Guid UUID { get; set; }

		/// <summary>
		///     Session不解释
		/// </summary>
		public Guid AccessToken { get; set; }

		/// <summary>
		///     各种属性（比如Twitch的Session）
		/// </summary>
		public string Properties { get; set; }

		/// <summary>
		///     错误信息，无错误则为null
		/// </summary>
		public string Error { get; set; }

		/// <summary>
		///     用户类型：Legacy or Mojang
		/// </summary>
		public string UserType { get; set; }

		/// <summary>
		///     其他验证信息，一边用不着
		/// </summary>
		public Dictionary<string, string> AdvancedInfo { get; set; }
	}
}