using System.Net.Http.Json;
using Bookify.Application.Abstractions.Authentication;
using Bookify.Domain.Users;
using Bookify.Infrastructure.Authentication.Models;

namespace Bookify.Infrastructure.Authentication;

internal sealed class AuthenticationService : IAuthenticationService
{
	private const string PasswordCredentialType = "password";

	// Typed HTTP client that is configured to access the Key Cloak RESTful API.
	private readonly HttpClient _httpClient;

	public AuthenticationService(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	public async Task<string> RegisterAsync(
		User user,
		string password,
		CancellationToken cancellationToken = default)
	{
		var userRepresentationModel = UserRepresentationModel.FromUser(user);

		// credentials of our user so that they can be able to log in in to Key Cloak. 
		userRepresentationModel.Credentials = new CredentialRepresentationModel[]
											  {
												  new()
												  {
													  Value = password,
													  Temporary = false,
													  Type = PasswordCredentialType
												  }
											  };

		// We need to send a POST request to this route containing a user representation
		// model, and we are specifying the 
		var response = await _httpClient.PostAsJsonAsync(
						   "users",
						   userRepresentationModel,
						   cancellationToken);

		return ExtractIdentityIdFromLocationHeader(response);
	}

	private static string ExtractIdentityIdFromLocationHeader(
		HttpResponseMessage httpResponseMessage)
	{
		const string usersSegmentName = "users/";

		var locationHeader = httpResponseMessage.Headers.Location?.PathAndQuery;

		if (locationHeader is null) throw new InvalidOperationException("Location header can't be null");

		var userSegmentValueIndex = locationHeader.IndexOf(
			usersSegmentName,
			StringComparison.InvariantCultureIgnoreCase);

		// Here we're extracting of the user identity ID
		// from the location header on the response because the 
		// response itself doesn't return anything in the response body.
		var userIdentityId = locationHeader.Substring(
			userSegmentValueIndex + usersSegmentName.Length);

		return userIdentityId;
	}
}