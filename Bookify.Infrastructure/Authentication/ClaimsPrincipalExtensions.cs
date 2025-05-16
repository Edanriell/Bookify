using System.Security.Claims;

namespace Bookify.Infrastructure.Authentication;

// Role-based Authorization
internal static class ClaimsPrincipalExtensions
{
	// Looks for name identifier claim
	public static string GetIdentityId(this ClaimsPrincipal? principal)
	{
		return principal?.FindFirstValue(ClaimTypes.NameIdentifier) ??
			   throw new ApplicationException("User identity is unavailable");
	}
}