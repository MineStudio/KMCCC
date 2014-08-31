using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LitJson;

namespace KMCCC.Yggdrasil
{
	using Tools;

	/// <summary>
	/// Yggdrasil(正版验证)客户端
	/// </summary>
	public class YggdrasilClient
	{
		public const String MojnagAuthServer = @"https://authserver.mojang.com";

		public const String Auth_Authentication = @"https://authserver.mojang.com/authenticate";
		public YggdrasilClient() : this(Guid.NewGuid()) { }

		public YggdrasilClient(Guid ClientToken)
		{
			this.ClientToken = ClientToken;
		}

		public Guid ClientToken { get; private set; }

		public Guid AccessToken { get; private set; }

		public Guid UUID { get; private set; }

		public String DisplayName { get; private set; }

		public String Properties { get; private set; }

		public String AccountType { get; private set; }

		private object locker = new object();

		public bool Authenticate(String Email, String Password, Boolean TwitchEnabled)
		{
			lock (locker)
			{
				Clear();
				try
				{
					AuthenticationRequest request = new AuthenticationRequest { Agent = Agent.Minecraft, Email = Email, Password = Password, RequestUser = TwitchEnabled, ClientToken = ClientToken.ToString() };
					var requestBody = JsonMapper.ToJson(request);
					var responseBody = HttpGP.Post(new Uri(Auth_Authentication), requestBody);
					var response = JsonMapper.ToObject<AuthenticationResponse>(responseBody);
					if (response.AccessToken == null)
					{
						return false;
					}
					else if (response.SelectedProfile == null)
					{
						return false;
					}
					else
					{
						this.AccessToken = Guid.Parse(response.AccessToken);
						if (response.User!=null)
						{
							this.AccountType = response.User.Legacy ? "Legacy" : "Mojang";
							if (response.User.Properties != null)
							{
								this.Properties = response.User.Properties.ToJson();
							}
						}
						else { this.AccountType = "Mojang"; }
						this.DisplayName = response.SelectedProfile.name;
						this.UUID = Guid.Parse(response.SelectedProfile.ID);
						return true;
					}
				}
				catch (Exception)
				{
					return false;
				}
			}
		}

		public void Clear()
		{
			lock (locker)
			{
				this.AccessToken = Guid.Empty;
				this.UUID = Guid.Empty;
				this.DisplayName = String.Empty;
				this.Properties = String.Empty;
				this.AccountType = String.Empty;
			}
		}
	}

	public class AuthenticationRequest
	{
		[JsonPropertyName("agent")]
		public Agent Agent { get; set; }

		[JsonPropertyName("username")]
		public String Email { get; set; }

		[JsonPropertyName("password")]
		public String Password { get; set; }

		[JsonPropertyName("requestUser")]
		public Boolean RequestUser { get; set; }

		[JsonPropertyName("clientToken")]
		public String ClientToken { get; set; }
	}

	public class Agent
	{
		public static readonly Agent Minecraft = new Agent { Name = "Minecraft", Version = 1 };

		[JsonPropertyName("name")]
		public String Name { get; set; }

		[JsonPropertyName("version")]
		public Int32 Version { get; set; }
	}

	public class AuthenticationResponse
	{
		[JsonPropertyName("clientToken")]
		public String ClientToken { get; set; }

		[JsonPropertyName("accessToken")]
		public String AccessToken { get; set; }

		[JsonPropertyName("availableProfiles")]
		public List<GameProfile> AvailableProfiles { get; set; }

		[JsonPropertyName("selectedProfile")]
		public GameProfile SelectedProfile { get; set; }

		[JsonPropertyName("user")]
		public User User { get; set; }

		[JsonPropertyName("error")]
		public String Error { get; set; }

		[JsonPropertyName("errorMessage")]
		public String ErrorMessage { get; set; }

		[JsonPropertyName("cause")]
		public String Cause { get; set; }
	}

	public class User
	{
		[JsonPropertyName("id")]
		public string ID { get; set; }

		[JsonPropertyName("properties")]
		public List<Property> Properties { get; set; }

		[JsonPropertyName("legacy")]
		public bool Legacy { get; set; }
	}

	public class Property
	{
		[JsonPropertyName("name")]
		public string Name { get; set; }

		[JsonPropertyName("value")]
		public string Value { get; set; }
	}

	public class GameProfile
	{
		[JsonPropertyName("id")]
		public String ID { get; set; }

		[JsonPropertyName("name")]
		public String name { get; set; }
	}

	internal static class Extensions
	{
		internal static String ToJson(this List<Property> properties)
		{
			StringBuilder sb = new StringBuilder().Append('{');
			foreach (var item in properties)
			{
				sb.Append('\"').Append(item.Name).Append("\":[\"").Append(item.Value).Append("\"]");
			}
			return sb.Append("}").ToString();
		}
	}
}
