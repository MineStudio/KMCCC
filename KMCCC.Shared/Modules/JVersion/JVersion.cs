namespace KMCCC.Modules.JVersion
{
	#region

	using System;
	using System.Collections.Generic;
	using LitJson;

	#endregion

	/// <summary>
	///     用来Json的实体类
	/// </summary>
	public class JVersion
	{
		[JsonPropertyName("id")]
		public string Id { get; set; }

		[JsonPropertyName("time")]
		public DateTime Time { get; set; }

		[JsonPropertyName("releaseTime")]
		public DateTime ReleaseTime { get; set; }

		[JsonPropertyName("type")]
		public string Type { get; set; }

		[JsonPropertyName("minecraftArguments")]
		public string MinecraftArguments { get; set; }

		[JsonPropertyName("minimumLauncherVersion")]
		public int MinimumLauncherVersion { get; set; }

		[JsonPropertyName("libraries")]
		public List<JLibrary> Libraries { get; set; }

		[JsonPropertyName("mainClass")]
		public string MainClass { get; set; }

		[JsonPropertyName("assets")]
		public string Assets { get; set; }

		[JsonPropertyName("inheritsFrom")]
		public string InheritsVersion { get; set; }

		[JsonPropertyName("jar")]
		public string JarId { get; set; }
	}

	public class JLibrary
	{
		[JsonPropertyName("name")]
		public string Name { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("natives")]
		public Dictionary<string, string> Natives { get; set; }

		[JsonPropertyName("rules")]
		public List<JRule> Rules { get; set; }

		[JsonPropertyName("extract")]
		public JExtract Extract { get; set; }
	}

	public class JRule
	{
		[JsonPropertyName("action")]
		public string Action { get; set; }

		[JsonPropertyName("os")]
		public JOperatingSystem OS { get; set; }
	}

	public class JOperatingSystem
	{
		[JsonPropertyName("name")]
		public string Name { get; set; }
	}

	public class JExtract
	{
		[JsonPropertyName("exclude")]
		public List<string> Exculde { get; set; }
	}
}