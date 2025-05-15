using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace Bookify.Infrastructure.Authentication.Models;

// Delegating handler is essentially just a wrapper around our HTTP request.
// It is a similar concept to middlewares in our API only this is wrapping
// our HTTP requests made by the HTTP client.
public sealed class AdminAuthorizationDelegatingHandler : DelegatingHandler
{
	private readonly KeycloakOptions _keycloakOptions;

	public AdminAuthorizationDelegatingHandler(IOptions<KeycloakOptions> keycloakOptions)
	{
		_keycloakOptions = keycloakOptions.Value;
	}

	protected override async Task<HttpResponseMessage> SendAsync(
		HttpRequestMessage request,
		CancellationToken cancellationToken)
	{
		// We are obtaining our access token so that we can authenticate with the Keycloak API.
		var authorizationToken = await GetAuthorizationToken(cancellationToken);

		// We are setting this token in the authorization header
		request.Headers.Authorization = new AuthenticationHeaderValue(
			JwtBearerDefaults.AuthenticationScheme,
			authorizationToken.AccessToken);

		// and then we are just passing through the request to the API. 
		var httpResponseMessage = await base.SendAsync(request, cancellationToken);

		// We also ensure that we get back a success status
		// code otherwise this is going to throw an exception. 
		httpResponseMessage.EnsureSuccessStatusCode();

		return httpResponseMessage;
	}

	private async Task<AuthorizationToken> GetAuthorizationToken(CancellationToken cancellationToken)
	{
		// We are specifying our admin client ID and secret, and we expect to get
		// back an authorization token response which contains our access
		// token which we are going to use to authenticate with the KeyCloakApi
		var authorizationRequestParameters = new KeyValuePair<string, string>[]
											 {
												 new("client_id", _keycloakOptions.AdminClientId),
												 new("client_secret", _keycloakOptions.AdminClientSecret),
												 new("scope", "openid email"),
												 new("grant_type", "client_credentials")
											 };

		var authorizationRequestContent = new FormUrlEncodedContent(authorizationRequestParameters);

		// Inside of the GetAuthorizationToken we are sending a client credentials request to the
		// token URL endpoint.
		var authorizationRequest = new HttpRequestMessage(
									   HttpMethod.Post,
									   new Uri(_keycloakOptions.TokenUrl))
								   {
									   Content = authorizationRequestContent
								   };

		var authorizationResponse = await base.SendAsync(authorizationRequest, cancellationToken);

		authorizationResponse.EnsureSuccessStatusCode();
 
		return await authorizationResponse.Content.ReadFromJsonAsync<AuthorizationToken>() ??
			   throw new ApplicationException();
	}
}