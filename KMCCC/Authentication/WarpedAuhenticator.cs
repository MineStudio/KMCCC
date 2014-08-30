using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMCCC.Authentication
{
	/// <summary>
	/// 反常验证器，只是用来包装验证信息让它看上去像是从验证器里出来的一样^_^
	/// PS: 不要吐槽名字
	/// </summary>
	public class WarpedAuhenticator : IAuthenticator
	{
		private AuthenticationInfo info;
		
		/// <summary>
		/// 创建反常验证器
		/// </summary>
		/// <param name="info">要包装的验证信息</param>
		public WarpedAuhenticator(AuthenticationInfo info)
		{
			this.info = info;
		}

		public AuthenticationInfo Do()
		{
			return info;
		}

		public Task<AuthenticationInfo> DoAsync()
		{
			return Task<AuthenticationInfo>.Factory.StartNew(() => info);
		}
	}
}
