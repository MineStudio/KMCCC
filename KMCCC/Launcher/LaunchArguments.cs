using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace KMCCC.Launcher
{
	using KMCCC.Authentication;
	using Tools;

	/// <summary>
	/// 启动参数
	/// </summary>
	public class MinecraftLaunchArguments
	{
		/// <summary>
		/// 新建启动参数
		/// </summary>
		public MinecraftLaunchArguments()
		{
			CGCEnabled = true;
			Tokens = new Dictionary<String, String>();
			AdvencedArguments = new List<String>();
		}

		/// <summary>
		/// 主类
		/// </summary>
		public String MainClass { get; set; }

		/// <summary>
		/// 库文件列表（推荐绝对路径）
		/// </summary>
		public List<String> Libraries { get; set; }

		/// <summary>
		/// 最大内存
		/// </summary>
		public Int32 MaxMemory { get; set; }

		/// <summary>
		/// 最小内存，推荐不设置
		/// </summary>
		public Int32 MinMemory { get; set; }

		/// <summary>
		/// 默认true，不要作死去设置成false
		/// </summary>
		public Boolean CGCEnabled { get; set; }

		/// <summary>
		/// 本地dll文件地址
		/// </summary>
		public String NativePath { get; set; }

		/// <summary>
		/// MC主参数（将由Token进行替换操作）
		/// </summary>
		public String MinecraftArguments { get; set; }

		/// <summary>
		/// 对MC主参数要进行的替换操作表
		/// </summary>
		public Dictionary<String, String> Tokens { get; set; }

		/// <summary>
		/// 附加参数
		/// </summary>
		public List<String> AdvencedArguments { get; set; }

		/// <summary>
		/// 转化为String参数
		/// </summary>
		/// <returns>转化后的参数</returns>
		public String ToArguments()
		{
			StringBuilder sb = new StringBuilder();
			if (CGCEnabled) { sb.Append("-Xincgc"); }
			if (MinMemory > 0) { sb.Append(" -Xms").Append(MinMemory).Append("M "); }
			if (MaxMemory > 0) { sb.Append(" -Xmx").Append(MaxMemory).Append("M"); }
			else
			{
				sb.Append("-Xmx2048M ");
			}
			foreach (var adv in AdvencedArguments)
			{
				sb.Append(' ').Append(adv);
			}
			sb.Append(" -Djava.library.path=\"").Append(NativePath);
			sb.Append("\" -cp \"");
			foreach (var lib in Libraries)
			{
				sb.Append(lib).Append(';');
			}
			sb.Append("\" ").Append(MainClass).Append(' ').Append(MinecraftArguments.DoReplace(Tokens));
			return sb.ToString();
		}

		internal AuthenticationInfo authentication;
		internal Version version;
	}

}
