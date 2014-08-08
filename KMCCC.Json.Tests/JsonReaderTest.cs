using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KMCCC.Json.Tests
{
	[TestClass]
	public class JsonReaderTest
	{
		[TestMethod]
		public void ReaderTest1()
		{
			var jr = new JsonReader(@"{""a"":""b""}");
			Assert.IsTrue(jr.Peek().Token == JsonToken.ObjectStart);
			Assert.IsTrue(jr.Peek().Token == JsonToken.ObjectStart);
			Assert.IsTrue(jr.Pop().Token == JsonToken.ObjectStart);
			Assert.IsTrue(jr.Peek().Token == JsonToken.String);
			Assert.IsTrue(jr.Pop().Token == JsonToken.String);
		}
	}
}
