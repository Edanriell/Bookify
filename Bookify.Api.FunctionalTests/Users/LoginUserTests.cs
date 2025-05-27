using System.Net;
using System.Net.Http.Json;
using Bookify.Api.Controllers.Users;
using Bookify.Api.FunctionalTests.Infrastructure;
using FluentAssertions;

namespace Bookify.Api.FunctionalTests.Users;

public class LoginUserTests : BaseFunctionalTest
{
	private const string Email = "login@test.com";
	private const string Password = "12345";

	public LoginUserTests ( FunctionalTestWebAppFactory factory )
		: base (
				factory : factory
			)
	{
	}

	[ Fact ]
	public async Task Login_ShouldReturnUnauthorized_WhenUserDoesNotExist()
	{
		// Arrange
		var request = new LogInUserRequest (
				Email : Email,
				Password : Password
			);

		// Act
		var response = await HttpClient.PostAsJsonAsync (
							   requestUri : "api/v1/users/login",
							   value : request
						   );

		// Assert
		response.StatusCode.Should().
			Be (
					expected : HttpStatusCode.Unauthorized
				);
	}

	[ Fact ]
	public async Task Login_ShouldReturnOk_WhenUserExists()
	{
		// Arrange
		var registerRequest = new RegisterUserRequest (
				Email : Email,
				FirstName : "first",
				LastName : "last",
				Password : Password
			);
		await HttpClient.PostAsJsonAsync (
				requestUri : "api/v1/users/register",
				value : registerRequest
			);

		var request = new LogInUserRequest (
				Email : Email,
				Password : Password
			);

		// Act
		var response = await HttpClient.PostAsJsonAsync (
							   requestUri : "api/v1/users/login",
							   value : request
						   );

		// Assert
		response.StatusCode.Should().
			Be (
					expected : HttpStatusCode.OK
				);
	}
}
