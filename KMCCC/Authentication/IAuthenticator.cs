using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMCCC.Authentication
{
	/// <summary>
	/// 验证器接口
	/// </summary>
	public interface IAuthenticator
	{
		/// <summary>
		/// 同步方式调用
		/// </summary>
		/// <returns>验证信息</returns>
		AuthenticationInfo Do();

		/// <summary>
		/// 异步方式调用
		/// </summary>
		/// <returns>验证信息</returns>
		Task<AuthenticationInfo> DoAsync();
	}
}
