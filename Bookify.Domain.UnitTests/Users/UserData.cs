using Bookify.Domain.Users;

namespace Bookify.Domain.UnitTests.Users;

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
}
