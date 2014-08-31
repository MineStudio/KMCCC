using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace KMCCC.Tools
{
	public static class HttpGP
	{
		public static String Get(Uri uri)
		{
			try
			{
				HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
				request.Method = "GET";
				request.ContentType = "application/octet-stream";
				HttpWebResponse response = (HttpWebResponse)request.GetResponse();
				using (var stream = response.GetResponseStream())
				{
					String str = String.Empty;
					byte[] bytes = null;
					string[] Strings = new string[1024];
					uint count = 0;
					long total = response.ContentLength;
					int now = 0;
					while (total > 0)
					{
						bytes = new byte[65537];
						now = (total >= 65536) ? 65536 : (int)total;
						now = stream.Read(bytes, 0, now);
						Strings[count] = Encoding.UTF8.GetString(bytes, 0, now);
						str += Strings[count];
						count++;
						total -= now;
					}
					return str;
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				return String.Empty;
			}
		}

		public static String Post(Uri uri, String data) 
		{
			try
			{
				HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
				request.Method = "POST";
				var requeststream = request.GetRequestStream();
				requeststream.Write(UTF8Encoding.UTF8.GetBytes(data),0,UTF8Encoding.UTF8.GetByteCount(data));
				requeststream.Flush();
				requeststream.Close();
				HttpWebResponse response = (HttpWebResponse)request.GetResponse();
				using (var stream = response.GetResponseStream())
				{
					String str = String.Empty;
					byte[] bytes = null;
					string[] Strings = new string[1024];
					uint count = 0;
					long total = response.ContentLength;
					int now = 0;
					while (total > 0)
					{
						bytes = new byte[65537];
						now = (total >= 65536) ? 65536 : (int)total;
						now = stream.Read(bytes, 0, now);
						Strings[count] = Encoding.UTF8.GetString(bytes, 0, now);
						str += Strings[count];
						count++;
						total -= now;
					}
					return str;
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				return String.Empty;
			}
		}
	}
}