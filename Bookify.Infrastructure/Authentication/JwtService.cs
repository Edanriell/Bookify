using System.Net.Http.Json;
using Bookify.Application.Abstractions.Authentication;
using Bookify.Domain.Abstractions;
using Bookify.Infrastructure.Authentication.Models;
using Microsoft.Extensions.Options;

namespace Bookify.Infrastructure.Authentication;

internal sealed class JwtService : IJwtService
{
	private static readonly Error AuthenticationFailed = new(
			Code : "Keycloak.AuthenticationFailed",
			Name : "Failed to acquire access token do to authentication failure"
		);

	private readonly HttpClient _httpClient;
	private readonly KeycloakOptions _keycloakOptions;

	public JwtService ( HttpClient httpClient, IOptions<KeycloakOptions> keycloakOptions )
	{
		_httpClient = httpClient;
		_keycloakOptions = keycloakOptions.Value;
	}

	public async Task<Result<string>> GetAccessTokenAsync ( string email,
															string password,
															CancellationToken cancellationToken
																= default(CancellationToken) )
	{
		try
		{
			var authRequestParameters = new KeyValuePair<string, string>[]
										{
											new(
													key : "client_id",
													value : _keycloakOptions.AuthClientId
												),
											new(
													key : "client_secret",
													value : _keycloakOptions.AuthClientSecret
												),
											new(
													key : "scope",
													value : "openid email"
												),
											new(
													key : "grant_type",
													value : "password"
												),
											new(
													key : "username",
													value : email
												),
											new(
													key : "password",
													value : password
												)
										};

			using var authorizationRequestContent = new FormUrlEncodedContent (
					nameValueCollection : authRequestParameters
				);

			var response = await _httpClient.PostAsync (
								   requestUri : "",
								   content : authorizationRequestContent,
								   cancellationToken : cancellationToken
							   );

			response.EnsureSuccessStatusCode();

			var authorizationToken = await response.Content.ReadFromJsonAsync<AuthorizationToken> (
											 cancellationToken : cancellationToken
										 );

			if ( authorizationToken is null )
				return Result.Failure<string> (
						error : AuthenticationFailed
					);

			return authorizationToken.AccessToken;
		}
		catch ( HttpRequestException )
		{
			return Result.Failure<string> (
					error : AuthenticationFailed
				);
		}
	}
}
