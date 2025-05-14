using System.Text.Json.Serialization;

namespace Bookify.Infrastructure.Authentication.Models;

// This AuthorizationToken is used to authenticate to the KeyCloak Admin API.
public sealed class AuthorizationToken
{
	[JsonPropertyName("access_token")] public string AccessToken { get; init; } = string.Empty;
}