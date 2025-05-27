using Bookify.Domain.Users;

namespace Bookify.Application.UnitTests.Users;

internal static class UserData
{
	public static readonly FirstName FirstName = new(
			Value : "First"
		);

	public static readonly LastName LastName = new(
			Value : "Last"
		);

	public static readonly Email Email = new(
			Value : "test@test.com"
		);

	public static User Create() => User.Create (
			firstName : FirstName,
			lastName : LastName,
			email : Email
		);
}
