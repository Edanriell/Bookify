using Bookify.Domain.Users;

namespace Bookify.Application.UnitTests.Users;

internal static class UserData
{
	public static readonly FirstName FirstName = new("First");
	public static readonly LastName LastName = new("Last");
	public static readonly Email Email = new("test@test.com");

	public static User Create()
	{
		return User.Create(FirstName, LastName, Email);
	}
}
