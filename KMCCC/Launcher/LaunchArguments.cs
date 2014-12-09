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
			Tokens = new Dictionary<string, string>();
			AdvencedArguments = new List<string>();
		}

		/// <summary>
		/// 主类
		/// </summary>
		public string MainClass { get; set; }

		/// <summary>
		/// 库文件列表（推荐绝对路径）
		/// </summary>
		public List<string> Libraries { get; set; }

		/// <summary>
		/// 最大内存
		/// </summary>
		public int MaxMemory { get; set; }

		/// <summary>
		/// 最小内存，推荐不设置
		/// </summary>
		public int MinMemory { get; set; }

		/// <summary>
		/// 默认true，不要作死去设置成false
		/// </summary>
		public bool CGCEnabled { get; set; }

		/// <summary>
		/// 本地dll文件地址
		/// </summary>
		public string NativePath { get; set; }

		/// <summary>
		/// MC主参数（将由Token进行替换操作）
		/// </summary>
		public string MinecraftArguments { get; set; }

		/// <summary>
		/// 对MC主参数要进行的替换操作表
		/// </summary>
		public Dictionary<string, string> Tokens { get; set; }

		/// <summary>
		/// 启动后直接连接到服务器
		/// </summary>
		public ServerInfo Server { get; set; }

		/// <summary>
		/// 启动后的窗口大小设置
		/// </summary>
		public WindowSize Size { get; set; }

		/// <summary>
		/// 附加参数
		/// </summary>
		public List<string> AdvencedArguments { get; set; }

		/// <summary>
		/// 转化为String参数
		/// </summary>
		/// <returns>转化后的参数</returns>
		public string ToArguments()
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
			if (this.Server != null)
			{
				if (!String.IsNullOrWhiteSpace(this.Server.Address))
				{
					sb.Append(" --server " + this.Server.Address);
					if (this.Server.Port==0)
					{
						sb.Append(" --port 25565");
					}
					else
					{
						sb.Append(" --port " + this.Server.Port.ToString());
					}
				}
			}
			if (this.Size != null)
			{
				if (this.Size.FullScreen == true)
				{
					sb.Append(" --fullscreen");
				}
				if (this.Size.Height != null)
				{
					ushort height = (ushort)this.Size.Height;
					if (height > 0) { sb.Append(String.Format(" --height {0}", height)); }
				}
				if (this.Size.Width != null)
				{
					ushort width = (ushort)this.Size.Width;
					if (width > 0) { sb.Append(String.Format(" --width {0}", width)); }
				}
			}
			return sb.ToString();
		}

		internal AuthenticationInfo authentication;
		internal Version version;
	}

	public class ServerInfo
	{
		public string Address { get; set; }

		public ushort Port { get; set; }
	}

	public class WindowSize
	{
		public bool? FullScreen { get; set; }

		public ushort? Height { get; set; }

		public ushort? Width { get; set; }
	}

}
