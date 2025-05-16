namespace Bookify.Api.Controllers;

// Role-based Authorization 
// This approach promotes reusability because
// we can specify the constant in multiple places and it is also
// less error prone.
public static class Roles
{
	public const string Registered = "Registered";
}