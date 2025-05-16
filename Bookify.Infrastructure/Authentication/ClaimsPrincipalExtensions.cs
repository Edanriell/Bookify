using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

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

	// Resource-based Authorization
	public static Guid GetUserId(this ClaimsPrincipal? principal)
	{
		var userId = principal?.FindFirstValue(JwtRegisteredClaimNames.Sub);

		return Guid.TryParse(userId, out var parsedUserId)
				   ? parsedUserId
				   : throw new ApplicationException("User identifier is unavailable");
	}
}