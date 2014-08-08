using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMCCC.Json
{
	public static class JObjectBuilder
	{
		public static JObject BuildJObject(this String json)
		{
			if (String.IsNullOrWhiteSpace(json))
			{
				throw new ArgumentNullException("json");
			}
			JsonReader reader = new JsonReader(json);
			if (reader.Peek().Token == JsonToken.ObjectStart)
			{
				return BuildJObjectInternal(reader);
			}
			else
			{
				return null;
			}
		}

		public static JArray BuildJArray(this String json)
		{
			if (String.IsNullOrWhiteSpace(json))
			{
				throw new ArgumentNullException("json");
			}
			JsonReader reader = new JsonReader(json);
			if (reader.Peek().Token == JsonToken.ArrayStart)
			{
				return BuildJArrayInternal(reader);
			}
			else
			{
				return null;
			}
		}

		private static JObject BuildJObjectInternal(JsonReader reader)
		{
			#region Check ObjectStart
			if (reader.Peek().Token != JsonToken.ObjectStart)
			{
				return null;
			}
			reader.Pop();
			#endregion
			#region Check if Empty
			if (reader.Peek().Token == JsonToken.ObjectEnd)
			{
				reader.Pop();
				return new JObject();
			}
			#endregion
			JObject obj = new JObject();
			while (reader.Peek().Token == JsonToken.String)
			{
				String key = reader.Pop().Data;
				if (reader.Peek().Token != JsonToken.Colon)
				{
					return null;
				}
				reader.Pop();
				switch (reader.Peek().Token)
				{
					case JsonToken.String:
						obj.Add(key, new JString { Name = key, Parent = obj, v = reader.Pop().Data });
						break;
					case JsonToken.Boolean:
						obj.Add(key, new JBoolean { Name = key, Parent = obj, v = reader.Pop().Data == "true" });
						break;
					case JsonToken.Number:
						obj.Add(key, new JNumber { Name = key, Parent = obj, v = double.Parse(reader.Pop().Data) });
						break;
					case JsonToken.Null:
						obj.Add(key, new JNull { Name = key, Parent = obj });
						break;
					case JsonToken.ArrayStart:
						var arrx = BuildJArrayInternal(reader);
						arrx.Name = key;
						arrx.Parent = obj;
						obj.Add(key, arrx);
						break;
					case JsonToken.ObjectStart:
						var objx = BuildJObjectInternal(reader);
						objx.Name = key;
						objx.Parent = obj;
						obj.Add(key, objx);
						break;
					default:
						return obj;
				}
				if (reader.Peek().Token != JsonToken.Comma) { break; }
				reader.Pop();
			}
			if (reader.Peek().Token != JsonToken.ObjectEnd)
			{
				return null;
			}
			reader.Pop();
			return obj;
		}

		private static JArray BuildJArrayInternal(JsonReader reader)
		{
			#region Check ArrayStart
			if (reader.Peek().Token != JsonToken.ArrayStart)
			{
				return null;
			}
			reader.Pop();
			#endregion
			#region Check if Empty
			if (reader.Peek().Token == JsonToken.ArrayEnd)
			{
				reader.Pop();
				return new JArray();
			}
			#endregion
			JArray arr = new JArray();
			while (reader.Peek().Token != JsonToken.ArrayEnd)
			{
				switch (reader.Peek().Token)
				{
					case JsonToken.String:
						arr.Add(new JString { Parent = arr, v = reader.Pop().Data });
						break;
					case JsonToken.Boolean:
						arr.Add(new JBoolean { Parent = arr, v = reader.Pop().Data == "true" });
						break;
					case JsonToken.Number:
						arr.Add(new JNumber { Parent = arr, v = double.Parse(reader.Pop().Data) });
						break;
					case JsonToken.Null:
						arr.Add(new JNull { Parent = arr });
						break;
					case JsonToken.ArrayStart:
						var arrx = BuildJArrayInternal(reader);
						arrx.Parent = arr;
						arr.Add(arrx);
						break;
					case JsonToken.ObjectStart:
						var objx = BuildJObjectInternal(reader);
						objx.Parent = arr;
						arr.Add(objx);
						break;
					default:
						return arr;
				}
				if (reader.Peek().Token != JsonToken.Comma) { break; }
				reader.Pop();
			}
			if (reader.Peek().Token != JsonToken.ArrayEnd)
			{
				return null;
			}
			reader.Pop();
			return arr;
		}
	}

}
