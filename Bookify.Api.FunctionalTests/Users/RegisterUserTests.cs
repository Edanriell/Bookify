using System.Net;
using System.Net.Http.Json;
using Bookify.Api.Controllers.Users;
using Bookify.Api.FunctionalTests.Infrastructure;
using FluentAssertions;

namespace Bookify.Api.FunctionalTests.Users;

public class RegisterUserTests : BaseFunctionalTest
{
	public RegisterUserTests ( FunctionalTestWebAppFactory factory )
		: base (
				factory : factory
			)
	{
	}

	[ Theory ]
	[ InlineData (
			"",
			"first",
			"last",
			"12345"
		) ]
	[ InlineData (
			"test.com",
			"first",
			"last",
			"12345"
		) ]
	[ InlineData (
			"@test.com",
			"first",
			"last",
			"12345"
		) ]
	[ InlineData (
			"test@",
			"first",
			"last",
			"12345"
		) ]
	[ InlineData (
			"test@test.com",
			"",
			"last",
			"12345"
		) ]
	[ InlineData (
			"test@test.com",
			"first",
			"",
			"12345"
		) ]
	[ InlineData (
			"test@test.com",
			"first",
			"last",
			""
		) ]
	[ InlineData (
			"test@test.com",
			"first",
			"last",
			"1"
		) ]
	[ InlineData (
			"test@test.com",
			"first",
			"last",
			"12"
		) ]
	[ InlineData (
			"test@test.com",
			"first",
			"last",
			"123"
		) ]
	[ InlineData (
			"test@test.com",
			"first",
			"last",
			"1234"
		) ]
	public async Task Register_ShouldReturnBadRequest_WhenRequestIsInvalid ( string email,
																			 string firstName,
																			 string lastName,
																			 string password )
	{
		// Arrange
		var request = new RegisterUserRequest (
				Email : email,
				FirstName : firstName,
				LastName : lastName,
				Password : password
			);

		// Act
		var response = await HttpClient.PostAsJsonAsync (
							   requestUri : "api/v1/users/register",
							   value : request
						   );

		// Assert
		response.StatusCode.Should().
			Be (
					expected : HttpStatusCode.BadRequest
				);
	}

	[ Fact ]
	public async Task Register_ShouldReturnOk_WhenRequestIsValid()
	{
		// Arrange
		var request = new RegisterUserRequest (
				Email : "create@test.com",
				FirstName : "first",
				LastName : "last",
				Password : "12345"
			);

		// Act
		var response = await HttpClient.PostAsJsonAsync (
							   requestUri : "api/v1/users/register",
							   value : request
						   );

		// Assert
		response.StatusCode.Should().
			Be (
					expected : HttpStatusCode.OK
				);
	}
}
