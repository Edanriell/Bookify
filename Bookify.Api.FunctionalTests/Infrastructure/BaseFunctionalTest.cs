using System.Net.Http.Json;
using Bookify.Api.Controllers.Users;
using Bookify.Api.FunctionalTests.Users;
using Bookify.Application.Users.LogInUser;

namespace Bookify.Api.FunctionalTests.Infrastructure;

public abstract class BaseFunctionalTest : IClassFixture<FunctionalTestWebAppFactory>
{
	protected readonly HttpClient HttpClient;

	protected BaseFunctionalTest ( FunctionalTestWebAppFactory factory ) { HttpClient = factory.CreateClient(); }

	protected async Task<string> GetAccessToken()
	{
		var loginResponse = await HttpClient.PostAsJsonAsync (
									requestUri : "api/v1/users/login",
									value : new LogInUserRequest (
											Email : UserData.RegisterTestUserRequest.Email,
											Password : UserData.RegisterTestUserRequest.Password
										)
								);

		var accessTokenResponse = await loginResponse.Content.ReadFromJsonAsync<AccessTokenResponse>();

		return accessTokenResponse!.AccessToken;
	}
}
