using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Bookify.Infrastructure.Authentication;

internal static class ClaimsPrincipalExtensions
{
	public static Guid GetUserId ( this ClaimsPrincipal? principal )
	{
		var userId = principal?.FindFirstValue (
				claimType : JwtRegisteredClaimNames.Sub
			);

		return Guid.TryParse (
					   input : userId,
					   result : out var parsedUserId
				   )
				   ? parsedUserId
				   : throw new ApplicationException (
							 message : "User id is unavailable"
						 );
	}

	public static string GetIdentityId ( this ClaimsPrincipal? principal ) => principal?.FindFirstValue (
																					  claimType : ClaimTypes.
																						  NameIdentifier
																				  )
																		   ?? throw new ApplicationException (
																					  message :
																					  "User identity is unavailable"
																				  );
}
