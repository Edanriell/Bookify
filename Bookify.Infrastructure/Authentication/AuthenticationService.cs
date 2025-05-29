using System.Net.Http.Json;
using Bookify.Application.Abstractions.Authentication;
using Bookify.Domain.Users;
using Bookify.Infrastructure.Authentication.Models;

namespace Bookify.Infrastructure.Authentication;

internal sealed class AuthenticationService : IAuthenticationService
{
	private const string PasswordCredentialType = "password";

	private readonly HttpClient _httpClient;

	public AuthenticationService ( HttpClient httpClient ) { _httpClient = httpClient; }

	public async Task<string> RegisterAsync ( User user,
											  string password,
											  CancellationToken cancellationToken = default(CancellationToken) )
	{
		var userRepresentationModel = UserRepresentationModel.FromUser (
				user : user
			);

		userRepresentationModel.Credentials = new CredentialRepresentationModel[]
											  {
												  new()
												  {
													  Value = password,
													  Temporary = false,
													  Type = PasswordCredentialType
												  }
											  };

		var response = await _httpClient.PostAsJsonAsync (
							   requestUri : "users",
							   value : userRepresentationModel,
							   cancellationToken : cancellationToken
						   );

		return ExtractIdentityIdFromLocationHeader (
				httpResponseMessage : response
			);
	}

	private static string ExtractIdentityIdFromLocationHeader ( HttpResponseMessage httpResponseMessage )
	{
		const string usersSegmentName = "users/";

		var locationHeader = httpResponseMessage.Headers.Location?.PathAndQuery;

		if ( locationHeader is null )
			throw new InvalidOperationException (
					message : "Location header can't be null"
				);

		var userSegmentValueIndex = locationHeader.IndexOf (
				value : usersSegmentName,
				comparisonType : StringComparison.InvariantCultureIgnoreCase
			);

		var userIdentityId = locationHeader.Substring (
				startIndex : userSegmentValueIndex + usersSegmentName.Length
			);

		return userIdentityId;
	}
}
