using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Bookify.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace Bookify.Infrastructure.Authorization;

// Role-based Authorization
internal sealed class CustomClaimsTransformation : IClaimsTransformation
{
	// Is used to create a service scope
	private readonly IServiceProvider _serviceProvider;

	public CustomClaimsTransformation ( IServiceProvider serviceProvider ) { _serviceProvider = serviceProvider; }

	// This method is giving us an access to the claims principe which allows us to add more 
	// claims to this claims principle object. 
	public async Task<ClaimsPrincipal> TransformAsync ( ClaimsPrincipal principal )
	{
		// We are checking if our claims principle already contains the claims that we will be looking for. 
		// So we say that principle has claim and then we are going to look for a claim where
		// the claim type is equal to the built in claim types and we are going to look for the
		// claim type called role. This is the claim type that is expected by our 
		// authorized attribute in the users controller. Also we are adding sublcaim which will be used
		// in resource based authorization. So we are saying that calim type equals and then we are going to
		// say JWT registered claim names, we are using one in the JSON Web Tokens namespace and we are going to use
		// sub as the expected claim name. So if our claims principle already contains these claims then we can just 
		// return the principle object and leave this method. The reason why we are looking for subclaim is because
		// ASP.NET is going to transform the incoming subclaim on our JSON Web Token into the name identifier claim.
		// So the subclaim itself will not exist anymore. 
		if ( principal.HasClaim (
					 match : claim => claim.Type == ClaimTypes.Role
				 )
		  && principal.HasClaim (
					 match : claim => claim.Type == JwtRegisteredClaimNames.Sub
				 ) )
			return principal;

		// We are creating a new scope
		using var scope = _serviceProvider.CreateScope();

		// We are using scope to resolve our authorization service by specifying the authorization service.
		var authorizationService = scope.ServiceProvider.GetRequiredService<AuthorizationService>();

		// Getting the identity id from the claims principle
		var identityId = principal.GetIdentityId();

		// Obtaining user roles
		var userRoles = await authorizationService.GetRolesForUserAsync (
								identityId : identityId
							);

		// Creating a new ClaimsIdentity instance
		var claimsIdentity = new ClaimsIdentity();

		// Adding claims
		claimsIdentity.AddClaim (
				claim : new Claim (
						type : JwtRegisteredClaimNames.Sub,
						value : userRoles.Id.ToString()
					)
			);

		foreach ( var role in userRoles.Roles )
			claimsIdentity.AddClaim (
					claim : new Claim (
							type : ClaimTypes.Role,
							value : role.Name
						)
				);

		// Accessing original claims principle and adding new identity
		principal.AddIdentity (
				identity : claimsIdentity
			);

		return principal;
	}
}
