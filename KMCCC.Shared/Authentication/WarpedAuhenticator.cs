namespace KMCCC.Authentication
{
	#region

	using System.Threading.Tasks;

	#endregion

	/// <summary>
	///     反常验证器，只是用来包装验证信息让它看上去像是从验证器里出来的一样^_^
	///     PS: 不要吐槽名字
	/// </summary>
	public class WarpedAuhenticator : IAuthenticator
	{
		private readonly AuthenticationInfo _info;

		/// <summary>
		///     创建反常验证器
		/// </summary>
		/// <param name="info">要包装的验证信息</param>
		public WarpedAuhenticator(AuthenticationInfo info)
		{
			_info = info;
		}

		public AuthenticationInfo Do()
		{
			return _info;
		}

		public Task<AuthenticationInfo> DoAsync()
		{
			return Task<AuthenticationInfo>.Factory.StartNew(() => _info);
		}
	}
}