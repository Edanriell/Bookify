using System.Net.Http.Json;
using Bookify.Application.Abstractions.Authentication;
using Bookify.Domain.Abstractions;
using Bookify.Infrastructure.Authentication.Models;
using Microsoft.Extensions.Options;

namespace Bookify.Infrastructure.Authentication;

// This service is also going to be a typed HTTP client and it needs access to the Keycloak options
// instance to access some configuration values. 
internal sealed class JwtService : IJwtService
{
	private static readonly Error AuthenticationFailed = new(
		"Keycloak.AuthenticationFailed",
		"Failed to acquire access token do to authentication failure");

	private readonly HttpClient _httpClient;
	private readonly KeycloakOptions _keycloakOptions;

	public JwtService(HttpClient httpClient, IOptions<KeycloakOptions> keycloakOptions)
	{
		_httpClient = httpClient;
		_keycloakOptions = keycloakOptions.Value;
	}

	public async Task<Result<string>> GetAccessTokenAsync(
		string email,
		string password,
		CancellationToken cancellationToken = default)
	{
		try
		{
			// We are sending an encoded form request containing our email and
			// password and the authentication client credentials. 
			var authRequestParameters = new KeyValuePair<string, string>[]
										{
											new("client_id", _keycloakOptions.AuthClientId),
											new("client_secret", _keycloakOptions.AuthClientSecret),
											new("scope", "openid email"),
											new("grant_type", "password"),
											new("username", email),
											new("password", password)
										};

			var authorizationRequestContent = new FormUrlEncodedContent(authRequestParameters);

			// Then we are sending a POST request tot the token endpoint. 
			var response = await _httpClient.PostAsync("", authorizationRequestContent, cancellationToken);

			response.EnsureSuccessStatusCode();

			// And we are parsing back an authorization token response. 
			var authorizationToken = await response.Content.ReadFromJsonAsync<AuthorizationToken>();

			if (authorizationToken is null) return Result.Failure<string>(AuthenticationFailed);

			// If all of this succeeds, we just return an access token from this method.
			return authorizationToken.AccessToken;
		}
		catch (HttpRequestException)
		{
			return Result.Failure<string>(AuthenticationFailed);
		}
	}
}