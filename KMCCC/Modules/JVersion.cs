using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LitJson;

namespace KMCCC.Version
{
	/// <summary>
	/// 用来Json的实体类
	/// </summary>
	public class JVersion
	{
		[JsonPropertyName("id")]
		public String Id { get; set; }

		[JsonPropertyName("time")]
		public DateTime Time { get; set; }

		[JsonPropertyName("releaseTime")]
		public DateTime ReleaseTime { get; set; }

		[JsonPropertyName("type")]
		public String Type { get; set; }

		[JsonPropertyName("minecraftArguments")]
		public String MinecraftArguments { get; set; }

		[JsonPropertyName("minimumLauncherVersion")]
		public Int32 MinimumLauncherVersion { get; set; }

		[JsonPropertyName("libraries")]
		public List<JLibrary> Libraries { get; set; }

		[JsonPropertyName("mainClass")]
		public String MainClass { get; set; }

		[JsonPropertyName("assets")]
		public String Assets { get; set; }
	}

	public class JLibrary
	{
		[JsonPropertyName("name")]
		public String Name { get; set; }

		[JsonPropertyName("natives")]
		public Dictionary<String, String> Natives { get; set; }

		[JsonPropertyName("rules")]
		public List<JRule> Rules { get; set; }

		[JsonPropertyName("extract")]
		public JExtract Extract { get; set; }
	}

	public class JRule
	{
		[JsonPropertyName("action")]
		public String Action { get; set; }

		[JsonPropertyName("os")]
		public JOS OS { get; set; }
	}

	public class JOS
	{
		[JsonPropertyName("name")]
		public String Name { get; set; }
	}

	public class JExtract
	{
		[JsonPropertyName("exclude")]
		public List<String> Exculde { get; set; }
	}
}
