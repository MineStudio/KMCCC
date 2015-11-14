using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LitJson;

namespace KMCCC.Modules.Yggdrasil
{

	#region

	#endregion

	/// <summary>
	///     Yggdrasil(正版验证)客户端
	/// </summary>
	public class YggdrasilClient
	{
		public const string MojnagAuthServer = @"https://authserver.mojang.com";
		private string Auth_Authentication => _authServer + "/authenticate";
		private string Auth_Refresh => _authServer + "/refresh";
		private readonly object _locker = new object();
		private readonly string _authServer;

		public YggdrasilClient(string authServer = null) : this(authServer, Guid.NewGuid())
		{
		}

		public YggdrasilClient(Guid clientToken) : this(null, clientToken)
		{
		}

		public YggdrasilClient(string authServer, Guid clientToken)
		{
			_authServer = authServer ?? MojnagAuthServer;
			ClientToken = clientToken;
		}

		public Guid ClientToken { get; }
		public Guid AccessToken { get; private set; }
		public Guid UUID { get; private set; }
		public string DisplayName { get; private set; }
		public string Properties { get; private set; }
		public string AccountType { get; private set; }

		public void Clear()
		{
			lock (_locker)
			{
				AccessToken = Guid.Empty;
				UUID = Guid.Empty;
				DisplayName = string.Empty;
				Properties = string.Empty;
				AccountType = string.Empty;
			}
		}

		private void UpdateFomrResponse(AuthenticationResponse response)
		{
			AccessToken = Guid.Parse(response.AccessToken);
			if (response.User != null)
			{
				AccountType = response.User.Legacy ? "Legacy" : "Mojang";
				Properties = response.User.Properties != null
					? response.User.Properties.ToJson()
					: "{}";
			}
			else
			{
				AccountType = "Mojang";
				Properties = "{}";
			}
			DisplayName = response.SelectedProfile.Name;
			UUID = Guid.Parse(response.SelectedProfile.Id);
		}

		#region Refresh

		public bool Refresh(bool twitchEnabled = true)
		{
			lock (_locker)
			{
				try
				{
					var wc = new WebClient();
					var requestBody = JsonMapper.ToJson(new RefreshRequest
					{
						Agent = Agent.Minecraft,
						AccessToken = AccessToken.ToString("N"),
						RequestUser = twitchEnabled,
						ClientToken = ClientToken.ToString("N")
					});
					var responseBody = wc.UploadString(new Uri(Auth_Refresh), requestBody);
					var response = JsonMapper.ToObject<AuthenticationResponse>(responseBody);
					if (response.AccessToken == null)
					{
						return false;
					}
					if (response.SelectedProfile == null)
					{
						return false;
					}
					UpdateFomrResponse(response);
					return true;
				}
				catch (Exception)
				{
					return false;
				}
			}
		}

		public bool Refresh(Guid accessToken, bool twitchEnabled = true)
		{
			lock (_locker)
			{
				Clear();
				try
				{
					var wc = new WebClient();
					var requestBody = JsonMapper.ToJson(new RefreshRequest
					{
						Agent = Agent.Minecraft,
						AccessToken = accessToken.ToString("N"),
						RequestUser = twitchEnabled,
						ClientToken = ClientToken.ToString("N")
					});
					var responseBody = wc.UploadString(new Uri(Auth_Refresh), requestBody);
					var response = JsonMapper.ToObject<AuthenticationResponse>(responseBody);
					if (response.AccessToken == null)
					{
						return false;
					}
					if (response.SelectedProfile == null)
					{
						return false;
					}
					UpdateFomrResponse(response);
					return true;
				}
				catch (Exception)
				{
					return false;
				}
			}
		}

		#endregion

		#region Authenticate

		public bool Authenticate(string email, string password, bool twitchEnabled = true)
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
						ClientToken = ClientToken.ToString("N")
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
					UpdateFomrResponse(response);
					return true;
				}
				catch (Exception)
				{
					return false;
				}
			}
		}

		public Task<bool> AuthenticateAsync(string email, string password, bool twitchEnabled = true,
			CancellationToken token = default(CancellationToken))
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
					ClientToken = ClientToken.ToString("N")
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
						UpdateFomrResponse(response);
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

		#endregion
	}

	#region Request & Response

	public class RefreshRequest
	{
		[JsonPropertyName("agent")]
		public Agent Agent { get; set; }

		[JsonPropertyName("accessToken")]
		public string AccessToken { get; set; }

		[JsonPropertyName("requestUser")]
		public bool RequestUser { get; set; }

		[JsonPropertyName("clientToken")]
		public string ClientToken { get; set; }
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

	#endregion

	public class Agent
	{
		public static readonly Agent Minecraft = new Agent {Name = "Minecraft", Version = 1};

		[JsonPropertyName("name")]
		public string Name { get; set; }

		[JsonPropertyName("version")]
		public int Version { get; set; }
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