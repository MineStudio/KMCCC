using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KMCCC.Json.Tests
{
	[TestClass]
	public class JObjectBuilderTest
	{
		[TestMethod]
		public void JObjectBuildTest()
		{
			String json = File.ReadAllText(@"D:\Ganeric\【萌爱世界】完美整合包（所有配置皆可）\【萌爱世界】完美整合包（所有配置皆可）\.minecraft\versions\【1.7.2】标准版光影整合包\【1.7.2】标准版光影整合包.json");
			JObject obj = json.BuildJObject();
		}
	}
}
