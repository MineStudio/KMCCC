namespace KMCCC.Authentication
{
	#region

	using System.Threading.Tasks;

	#endregion

	/// <summary>
	///     验证器接口
	/// </summary>
	public interface IAuthenticator
	{
		/// <summary>
		///     同步方式调用
		/// </summary>
		/// <returns>验证信息</returns>
		AuthenticationInfo Do();

		/// <summary>
		///     异步方式调用
		/// </summary>
		/// <returns>验证信息</returns>
		Task<AuthenticationInfo> DoAsync();
	}
}