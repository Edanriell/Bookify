using System.Security.Claims;
using Bookify.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Bookify.Infrastructure.Authorization;

internal sealed class CustomClaimsTransformation : IClaimsTransformation
{
	private readonly IServiceProvider _serviceProvider;

	public CustomClaimsTransformation ( IServiceProvider serviceProvider ) { _serviceProvider = serviceProvider; }

	public async Task<ClaimsPrincipal> TransformAsync ( ClaimsPrincipal principal )
	{
		if ( principal.Identity is not
			 {
				 IsAuthenticated: true
			 }
		  || ( principal.HasClaim (
					   match : claim => claim.Type == ClaimTypes.Role
				   )
			&& principal.HasClaim (
					   match : claim => claim.Type == JwtRegisteredClaimNames.Sub
				   ) ) )
			return principal;

		using var scope = _serviceProvider.CreateScope();

		var authorizationService = scope.ServiceProvider.GetRequiredService<AuthorizationService>();

		var identityId = principal.GetIdentityId();

		var userRoles = await authorizationService.GetRolesForUserAsync (
								identityId : identityId
							);

		var claimsIdentity = new ClaimsIdentity();

		claimsIdentity.AddClaim (
				claim : new Claim (
						type : JwtRegisteredClaimNames.Sub,
						value : userRoles.UserId.ToString()
					)
			);

		foreach ( var role in userRoles.Roles )
			claimsIdentity.AddClaim (
					claim : new Claim (
							type : ClaimTypes.Role,
							value : role.Name
						)
				);

		principal.AddIdentity (
				identity : claimsIdentity
			);

		return principal;
	}
}
