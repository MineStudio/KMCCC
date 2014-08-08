using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace KMCCC.Json
{

	#region JsonReader

	/// <summary>
	/// 从一个字符串中分出一个又一个Json块
	/// </summary>
	public class JsonReader
	{
		private StringReader reader;

		public JsonReader(String json)
		{
			reader = new StringReader(json);
		}

		public JsonReaderItem Peek()
		{
			if (templated != null)
			{
				return templated;
			}
			else
			{
				templated = getNext();
				return templated;
			}
		}

		private JsonReaderItem templated;

		public JsonReaderItem Pop()
		{
			if (templated != null)
			{
				var result = templated;
				templated = getNext();
				return result;
			}
			else
			{
				var result = getNext();
				templated = getNext();
				return result;
			}
		}

		private JsonReaderItem getNext()
		{
			while (reader.Peek() != -1)
			{
				if (char.IsWhiteSpace((char)reader.Peek()))
				{
					reader.Read();
				}
				else
				{
					break;
				}
			}
			if (reader.Peek() == -1)
			{
				return new JsonReaderItem { Token = JsonToken.End, Data = String.Empty };
			}
			int c = reader.Read();
			StringBuilder sb;
			switch (c)
			{
				case '{': return new JsonReaderItem { Token = JsonToken.ObjectStart, Data = "{" };
				case '}': return new JsonReaderItem { Token = JsonToken.ObjectEnd, Data = "}" };
				case '[': return new JsonReaderItem { Token = JsonToken.ArrayStart, Data = "[" };
				case ']': return new JsonReaderItem { Token = JsonToken.ArrayEnd, Data = "]" };
				case ':': return new JsonReaderItem { Token = JsonToken.Colon, Data = ":" };
				case ',': return new JsonReaderItem { Token = JsonToken.Comma, Data = "," };
				case '"':
					#region String
					sb = new StringBuilder();
					while (reader.Peek() != '"')
					{
						if (reader.Peek() == -1) { break; }
						if (reader.Peek() == '\\')
						{
							reader.Read();
							switch (reader.Peek())
							{
								case '"':
								case '\\':
								case '/':
									sb.Append((char)reader.Read());
									break;
								case 'b':
									reader.Read();
									sb.Remove(sb.Length - 1, 1);
									break;
								case 'f':
									reader.Read();
									break;
								case 'n':
									reader.Read();
									sb.Append('\n');
									break;
								case 'r':
									reader.Read();
									sb.Append('\r');
									break;
								case 't':
									reader.Read();
									sb.Append('\t');
									break;
								case 'u':
									reader.Read();
									#region UTF
									int x = 0;
									for (int i = 0; i < 3; i++)
									{
										switch (reader.Peek())
										{
											case '0':
											case '1':
											case '2':
											case '3':
											case '4':
											case '5':
											case '6':
											case '7':
											case '8':
											case '9':
												x = (x << 4) + reader.Read() - 48;
												break;
											case 'a':
											case 'b':
											case 'c':
											case 'd':
											case 'e':
											case 'f':
												x = (x << 4) + reader.Read() - 87;
												break;
											case 'A':
											case 'B':
											case 'C':
											case 'D':
											case 'E':
											case 'F':
												x = (x << 4) + reader.Read() - 55;
												break;
											default:
												x = x << 4;
												break;
										}
									}
									sb.Append((char)x);
									#endregion
									break;
								default:
									break;
							}
						}
						else { sb.Append((char)reader.Read()); }
					}
					reader.Read();
					return new JsonReaderItem { Token = JsonToken.String, Data = sb.ToString() };
					#endregion
				case -1:
					return new JsonReaderItem { Token = JsonToken.End, Data = String.Empty };
				default:
					sb = new StringBuilder();
					sb.Append((char)c);
					while (isBasic((char)reader.Peek()))
					{
						sb.Append((char)reader.Read());
					}
					var st = sb.ToString();
					if (st == "true")
					{
						return new JsonReaderItem { Token = JsonToken.Boolean, Data = "true" };
					}
					if (st == "false")
					{
						return new JsonReaderItem { Token = JsonToken.Boolean, Data = "false" };
					}
					if (st == "null")
					{
						return new JsonReaderItem { Token = JsonToken.Null, Data = "null" };
					}
					double v;
					if (double.TryParse(st, out v))
					{
						return new JsonReaderItem { Token = JsonToken.Number, Data = st };
					}
					return new JsonReaderItem { Token = JsonToken.Error, Data = st };
			}
		}

		private bool isBasic(char ch)
		{
			return (char.IsNumber(ch)) || (char.IsLower(ch)) || (char.IsUpper(ch)) || (ch == '+') || (ch == '-') || (ch == '.');
		}
	}

	public enum JsonToken
	{
		ObjectStart, ObjectEnd,
		ArrayStart, ArrayEnd,
		String, Boolean, Number,
		Colon, Comma, Null, Error,
		End
	}

	public class JsonReaderItem
	{
		public JsonToken Token { get; set; }

		public String Data { get; set; }
	}

	#endregion
}
