using Bookify.Api.Controllers.Users;

namespace Bookify.Api.FunctionalTests.Users;

internal static class UserData
{
	public static RegisterUserRequest RegisterTestUserRequest = new(
			Email : "test@test.com",
			FirstName : "test",
			LastName : "test",
			Password : "12345"
		);
}
