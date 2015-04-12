namespace KMCCC.Authentication
{
	#region

	using System.Threading;
	using System.Threading.Tasks;

	#endregion

	/// <summary>
	///     验证器接口
	/// </summary>
	public interface IAuthenticator
	{
		/// <summary>
		///     获取验证器的类型
		/// </summary>
		string Type { get; }

		/// <summary>
		///     同步方式调用
		/// </summary>
		/// <returns>验证信息</returns>
		AuthenticationInfo Do();

		/// <summary>
		///     异步方式调用
		/// </summary>
		/// <returns>验证信息</returns>
		Task<AuthenticationInfo> DoAsync(CancellationToken token);
	}
}