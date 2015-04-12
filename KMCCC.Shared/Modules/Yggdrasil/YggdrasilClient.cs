namespace KMCCC.Modules.Yggdrasil
{
	#region

	using System;
	using System.Collections.Generic;
	using System.Net;
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;
	using LitJson;

	#endregion

	/// <summary>
	///     Yggdrasil(正版验证)客户端
	/// </summary>
	public class YggdrasilClient
	{
		public const string MojnagAuthServer = @"https://authserver.mojang.com";

		public const string Auth_Authentication = @"https://authserver.mojang.com/authenticate";
		private readonly object _locker = new object();

		public YggdrasilClient() : this(Guid.NewGuid())
		{
		}

		public YggdrasilClient(Guid clientToken)
		{
			ClientToken = clientToken;
		}

		public Guid ClientToken { get; private set; }

		public Guid AccessToken { get; private set; }

		public Guid UUID { get; private set; }

		public string DisplayName { get; private set; }

		public string Properties { get; private set; }

		public string AccountType { get; private set; }

		public bool Authenticate(string email, string password, Boolean twitchEnabled)
		{
			lock (_locker)
			{
				Clear();
				try
				{
					var wc = new WebClient();
					var requestBody = JsonMapper.ToJson(new AuthenticationRequest
					{
						Agent = Agent.Minecraft,
						Email = email,
						Password = password,
						RequestUser = twitchEnabled,
						ClientToken = ClientToken.ToString()
					});
					var responseBody = wc.UploadString(new Uri(Auth_Authentication), requestBody);
					var response = JsonMapper.ToObject<AuthenticationResponse>(responseBody);
					if (response.AccessToken == null)
					{
						return false;
					}
					if (response.SelectedProfile == null)
					{
						return false;
					}
					AccessToken = Guid.Parse(response.AccessToken);
					if (response.User != null)
					{
						AccountType = response.User.Legacy ? "Legacy" : "Mojang";
						if (response.User.Properties != null)
						{
							Properties = response.User.Properties.ToJson();
						}
					}
					else
					{
						AccountType = "Mojang";
					}
					DisplayName = response.SelectedProfile.Name;
					UUID = Guid.Parse(response.SelectedProfile.Id);
					return true;
				}
				catch (Exception)
				{
					return false;
				}
			}
		}

		public Task<bool> AuthenticateAsync(string email, string password, Boolean twitchEnabled, CancellationToken token)
		{
			Clear();
			var task = new TaskCompletionSource<bool>(token);
			try
			{
				var wc = new WebClient();
				var requestBody = JsonMapper.ToJson(new AuthenticationRequest
				{
					Agent = Agent.Minecraft,
					Email = email,
					Password = password,
					RequestUser = twitchEnabled,
					ClientToken = ClientToken.ToString()
				});
				wc.UploadStringCompleted += (sender, e) =>
				{
					try
					{
						if (e.Error != null)
						{
							task.SetException(e.Error);
							return;
						}
						var response = JsonMapper.ToObject<AuthenticationResponse>(e.Result);
						if ((response.AccessToken == null) || (response.SelectedProfile == null))
						{
							task.SetResult(false);
							return;
						}
						AccessToken = Guid.Parse(response.AccessToken);
						if (response.User != null)
						{
							AccountType = response.User.Legacy ? "Legacy" : "Mojang";
							if (response.User.Properties != null)
							{
								Properties = response.User.Properties.ToJson();
							}
						}
						else
						{
							AccountType = "Mojang";
						}
						DisplayName = response.SelectedProfile.Name;
						UUID = Guid.Parse(response.SelectedProfile.Id);
						task.SetResult(true);
					}
					catch (Exception exception)
					{
						task.SetException(exception);
					}
				};
				wc.UploadStringAsync(new Uri(Auth_Authentication), requestBody);
				return task.Task;
			}
			catch (Exception exception)
			{
				task.SetException(exception);
				return task.Task;
			}
		}

		public void Clear()
		{
			lock (_locker)
			{
				AccessToken = Guid.Empty;
				UUID = Guid.Empty;
				DisplayName = String.Empty;
				Properties = String.Empty;
				AccountType = String.Empty;
			}
		}
	}

	public class AuthenticationRequest
	{
		[JsonPropertyName("agent")]
		public Agent Agent { get; set; }

		[JsonPropertyName("username")]
		public string Email { get; set; }

		[JsonPropertyName("password")]
		public string Password { get; set; }

		[JsonPropertyName("requestUser")]
		public bool RequestUser { get; set; }

		[JsonPropertyName("clientToken")]
		public string ClientToken { get; set; }
	}

	public class Agent
	{
		public static readonly Agent Minecraft = new Agent {Name = "Minecraft", Version = 1};

		[JsonPropertyName("name")]
		public string Name { get; set; }

		[JsonPropertyName("version")]
		public int Version { get; set; }
	}

	public class AuthenticationResponse
	{
		[JsonPropertyName("clientToken")]
		public string ClientToken { get; set; }

		[JsonPropertyName("accessToken")]
		public string AccessToken { get; set; }

		[JsonPropertyName("availableProfiles")]
		public List<GameProfile> AvailableProfiles { get; set; }

		[JsonPropertyName("selectedProfile")]
		public GameProfile SelectedProfile { get; set; }

		[JsonPropertyName("user")]
		public User User { get; set; }

		[JsonPropertyName("error")]
		public string Error { get; set; }

		[JsonPropertyName("errorMessage")]
		public string ErrorMessage { get; set; }

		[JsonPropertyName("cause")]
		public string Cause { get; set; }
	}

	public class User
	{
		[JsonPropertyName("id")]
		public string Id { get; set; }

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
		public string Id { get; set; }

		[JsonPropertyName("name")]
		public string Name { get; set; }
	}

	internal static class Extensions
	{
		internal static string ToJson(this List<Property> properties)
		{
			var sb = new StringBuilder().Append('{');
			foreach (var item in properties)
			{
				sb.Append('\"').Append(item.Name).Append("\":[\"").Append(item.Value).Append("\"]");
			}
			return sb.Append("}").ToString();
		}
	}
}