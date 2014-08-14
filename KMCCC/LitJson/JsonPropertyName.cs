using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LitJson
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public class JsonPropertyName : Attribute
	{
		public String Name { get; private set; }

		public JsonPropertyName(String name)
		{
			this.Name = name;
		}
	}
}
