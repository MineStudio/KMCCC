using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LitJson;

namespace KMCCC.Yggdrasil
{
	public class YggdrasilClient
	{
		public const String MojnagAuthServer = @"https://authserver.mojang.com";

		public const String Auth_Authentication = @"https://authserver.mojang.com/authenticate";

		public Guid ClientToken { get; private set; }

		public Guid AccessToken { get; private set; }

		public String DisplayName { get; private set; }

		public bool TwitchEnabled { get; private set; }
	}

	public class AuthenticationRequest
	{
		[JsonPropertyName("agent")]
		public Agent Agent { get; set; }

		[JsonPropertyName("username")]
		public String Email { get; set; }
		
		[JsonPropertyName("password")]
		public String Password { get; set; }

		[JsonPropertyName("requesetUser")]
		public Boolean RequestUser { get; set; }
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
}
