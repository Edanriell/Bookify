using System.Text.Json.Serialization;

namespace Bookify.Infrastructure.Authentication.Models;

internal sealed class AuthorizationToken
{
	[ JsonPropertyName (
			name : "access_token"
		) ]
	public string AccessToken { get; init; } = string.Empty;
}
