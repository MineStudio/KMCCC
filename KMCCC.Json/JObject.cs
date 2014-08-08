using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMCCC.Json
{
	public abstract class JNode
	{
		public JNode()
		{

		}

		public String Name { get; set; }

		public JNode Parent { get; set; }

		internal abstract StringBuilder WriteToString(StringBuilder sb);

		public override string ToString()
		{
			return WriteToString(new StringBuilder()).ToString();
		}
	}

	#region JNull

	public class JNull : JNode
	{
		public JNull()
		{

		}

		internal override StringBuilder WriteToString(StringBuilder sb)
		{
			return sb.Append("null");
		}
	}

	#endregion

	#region JString

	public class JString : JNode
	{
		public JString()
		{
			v = String.Empty;
		}

		public String v;

		public String Value
		{
			get { return this.v; }
			set { if (value == null) { v = String.Empty; } else { v = value; } }
		}

		internal override StringBuilder WriteToString(StringBuilder sb)
		{
			return Value == null ? sb.Append("\"\"") : sb.Append('\"').Append(Value).Append('\"');
		}
	}

	#endregion

	#region JBoolean

	public class JBoolean : JNode
	{
		public JBoolean()
		{
			v = false;
		}

		public Boolean v;

		public Boolean Value
		{
			get { return this.v; }
			set { v = value; }
		}

		internal override StringBuilder WriteToString(StringBuilder sb)
		{
			return v ? sb.Append("true") : sb.Append("false");
		}
	}

	#endregion

	#region JNumber

	public class JNumber : JNode
	{
		public JNumber()
		{
			v = 0;
		}

		public Double v;

		public Double Value
		{
			get { return this.v; }
			set { v = value; }
		}

		internal override StringBuilder WriteToString(StringBuilder sb)
		{
			return sb.Append(v.ToString());
		}
	}

	#endregion

	#region JArray

	public class JArray : JNode, IEnumerable<JNode>
	{
		public JArray()
		{
			v = new List<JNode>();
		}

		public void Add(JNode item)
		{
			v.Add(item);
		}

		private List<JNode> v;

		public List<JNode> Children
		{
			get { return v; }
			set { if (value == null) { this.v = new List<JNode>(); } else { v = value; } }
		}

		public JNode this[int index]
		{
			get
			{
				if (index < 0) { throw new IndexOutOfRangeException(); }
				if (index >= Children.Count) { throw new IndexOutOfRangeException(); }
				return Children[index];
			}
			set
			{
				if (index < 0) { throw new IndexOutOfRangeException(); }
				Children[index] = value;
			}
		}


		public IEnumerator<JNode> GetEnumerator()
		{
			return Children.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Children.GetEnumerator();
		}

		internal override StringBuilder WriteToString(StringBuilder sb)
		{
			sb.Append('[');
			bool first = true;
			foreach (var node in v)
			{
				if (first) { first = false; } else { sb.Append(','); }
				node.WriteToString(sb);
			}
			return sb.Append(']');
		}
	}

	#endregion

	#region JObject

	public class JObject : JNode, IDictionary<String, JNode>
	{
		public JObject()
		{
			v = new Dictionary<string, JNode>();
		}

		private Dictionary<String, JNode> v;

		public Dictionary<String, JNode> Children
		{
			get { return v; }
			set { if (value == null) { v = new Dictionary<string, JNode>(); } else { v = value; } }
		}

		internal override StringBuilder WriteToString(StringBuilder sb)
		{
			sb.Append('{');
			bool first = true;
			foreach (var pair in v)
			{
				if (first) { first = false; } else { sb.Append(','); }
				sb.Append('"').Append(pair.Key).Append('"').Append(':');
				pair.Value.WriteToString(sb);
			}
			return sb.Append('}');
		}

		public void Add(string key, JNode value)
		{
			v.Add(key, value);
		}

		public bool ContainsKey(string key)
		{
			return v.ContainsKey(key);
		}

		public ICollection<string> Keys
		{
			get { return v.Keys; }
		}

		public bool Remove(string key)
		{
			return v.Remove(key);
		}

		public bool TryGetValue(string key, out JNode value)
		{
			return v.TryGetValue(key, out value);
		}

		public ICollection<JNode> Values
		{
			get { return v.Values; }
		}

		public JNode this[string key]
		{
			get
			{
				return v[key];
			}
			set
			{
				v[key] = value;
			}
		}

		public void Add(KeyValuePair<string, JNode> item)
		{
			v.Add(item.Key, item.Value);
		}

		public void Clear()
		{
			v.Clear();
		}

		public bool Contains(KeyValuePair<string, JNode> item)
		{
			return v.Contains(item);
		}

		[Obsolete]
		public void CopyTo(KeyValuePair<string, JNode>[] array, int index)
		{
			throw new NotImplementedException();
		}

		public int Count
		{
			get { return v.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(KeyValuePair<string, JNode> item)
		{
			return v.Remove(item.Key);
		}

		public IEnumerator<KeyValuePair<string, JNode>> GetEnumerator()
		{
			return v.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return v.GetEnumerator();
		}
	}

	#endregion
}
