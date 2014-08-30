using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KMCCC.Authentication;

namespace KMCCC.Launcher
{
	/// <summary>
	/// 启动选项
	/// </summary>
	public class LaunchOptions
	{
		/// <summary>
		/// 最大内存
		/// </summary>
		public Int32 MaxMemory { get; set; }

		/// <summary>
		/// 最小内存
		/// </summary>
		public Int32 MinMemory { get; set; }

		/// <summary>
		/// 启动的版本
		/// </summary>
		public Version Version { get; set; }

		/// <summary>
		/// 使用的验证器
		/// </summary>
		public IAuthenticator Authenticator { get; set; }
	}

	/// <summary>
	/// 启动句柄，基本上也就比较用
	/// </summary>
	public class LaunchHandle
	{
		/// <summary>
		/// 只读的验证信息
		/// </summary>
		public readonly AuthenticationInfo info;

		internal LauncherCore core;

		internal Process process;

		internal LaunchHandle(AuthenticationInfo info)
		{
			this.info = info;
		}

		internal StreamReader _gameoutput;
		internal StreamReader _gameerror;

		internal Thread _thOutput;
		internal Thread _thError;

		internal void Logger()
		{
			_gameoutput = process.StandardOutput;
			_gameerror = process.StandardError;
			_thOutput = new Thread(new ThreadStart(delegate
			{
				while (true)
				{
					try
					{
						if (!_gameoutput.EndOfStream)
						{
							string line = _gameoutput.ReadLine();
							core.log(this, line);
						}
					}
					catch (Exception ex)
					{
						string line = ex.StackTrace;
						core.log(this, line);
					}
				}
				// ReSharper disable once FunctionNeverReturns
			}));
			_thError = new Thread(new ThreadStart(delegate
			{
				while (true)
				{
					try
					{
						if (!_gameerror.EndOfStream)
						{
							string line = _gameerror.ReadLine();
							core.log(this, line);
						}
					}
					catch (Exception ex)
					{
						string line = ex.StackTrace;
						core.log(this, line);
					}
				}
				// ReSharper disable once FunctionNeverReturns
			}));
			_thOutput.IsBackground = true;
			_thError.IsBackground = true;
			_thOutput.Start();
			_thError.Start();

		}

		internal void work()
		{
			Logger();
		}
	}

	/// <summary>
	/// 启动异常（未启用）
	/// </summary>
	public class LaunchException : Exception
	{
		/// <summary>
		/// 异常类型
		/// </summary>
		public LaunchExceptionType Type { get; private set; }

		/// <summary>
		/// 启动异常
		/// </summary>
		/// <param name="type">异常类型</param>
		/// <param name="message">异常信息</param>
		public LaunchException(LaunchExceptionType type, String message) : base(message) { this.Type = type; }
		/// <summary>
		/// 启动异常
		/// </summary>
		/// <param name="type">异常类型</param>
		/// <param name="message">异常信息</param>
		/// <param name="innerException">内部异常</param>
		public LaunchException(LaunchExceptionType type, String message, Exception innerException) : base(message, innerException) { this.Type = type; }
	}

	/// <summary>
	/// 异常类型
	/// </summary>
	public enum LaunchExceptionType
	{
		/// <summary>
		/// 验证器错误
		/// </summary>
		Authenticator, 
		/// <summary>
		/// 启动参数操作器错误
		/// </summary>
		ArguementsOperator, 
		/// <summary>
		/// 启动时错误
		/// </summary>
		LaunchTime, 
		/// <summary>
		/// 未知
		/// </summary>
		Unknow
	}
}
