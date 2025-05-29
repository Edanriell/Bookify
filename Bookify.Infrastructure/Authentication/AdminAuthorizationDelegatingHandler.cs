using System.Net.Http.Headers;
using System.Net.Http.Json;
using Bookify.Infrastructure.Authentication.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace Bookify.Infrastructure.Authentication;

internal sealed class AdminAuthorizationDelegatingHandler : DelegatingHandler
{
	private readonly KeycloakOptions _keycloakOptions;

	public AdminAuthorizationDelegatingHandler ( IOptions<KeycloakOptions> keycloakOptions )
	{
		_keycloakOptions = keycloakOptions.Value;
	}

	protected override async Task<HttpResponseMessage> SendAsync ( HttpRequestMessage request,
																   CancellationToken cancellationToken )
	{
		var authorizationToken = await GetAuthorizationToken (
										 cancellationToken : cancellationToken
									 );

		request.Headers.Authorization = new AuthenticationHeaderValue (
				scheme : JwtBearerDefaults.AuthenticationScheme,
				parameter : authorizationToken.AccessToken
			);

		var httpResponseMessage = await base.SendAsync (
										  request : request,
										  cancellationToken : cancellationToken
									  );

		httpResponseMessage.EnsureSuccessStatusCode();

		return httpResponseMessage;
	}

	private async Task<AuthorizationToken> GetAuthorizationToken ( CancellationToken cancellationToken )
	{
		var authorizationRequestParameters = new KeyValuePair<string, string>[]
											 {
												 new(
														 key : "client_id",
														 value : _keycloakOptions.AdminClientId
													 ),
												 new(
														 key : "client_secret",
														 value : _keycloakOptions.AdminClientSecret
													 ),
												 new(
														 key : "scope",
														 value : "openid email"
													 ),
												 new(
														 key : "grant_type",
														 value : "client_credentials"
													 )
											 };

		var authorizationRequestContent = new FormUrlEncodedContent (
				nameValueCollection : authorizationRequestParameters
			);

		using var authorizationRequest = new HttpRequestMessage (
											 method : HttpMethod.Post,
											 requestUri : new Uri (
													 uriString : _keycloakOptions.TokenUrl
												 )
										 )
										 {
											 Content = authorizationRequestContent
										 };

		var authorizationResponse = await base.SendAsync (
											request : authorizationRequest,
											cancellationToken : cancellationToken
										);

		authorizationResponse.EnsureSuccessStatusCode();

		return await authorizationResponse.Content.ReadFromJsonAsync<AuthorizationToken> (
					   cancellationToken : cancellationToken
				   )
			?? throw new ApplicationException();
	}
}
