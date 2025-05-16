using Bookify.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Bookify.Infrastructure.Authorization;

// Permission-based Authorization
// Custom authorization handler
internal sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
	// Authorization service is to used to obtain the permissions for the
	// current user and then check if they satisfy the permission requirement. 
	private readonly IServiceProvider _serviceProvider;

	// Injecting IServiceProvider interface, we have to use this approach because
	// we are going to register the authorization handler as a transient service.
	public PermissionAuthorizationHandler(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	// Inside of this method, we have to figure out if the current user is authenticated with our
	// API and if they have the required set of permissions. 
	protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
														 PermissionRequirement requirement)
	{
		// If it is not an object where the IsAuthenticated value is true, return from this method
		// Basically, if the user is not authenticated, we won't even check for permissions.
		// 
		if (context.User.Identity is not { IsAuthenticated: true }) return;

		// Creating custom service scope
		using var scope = _serviceProvider.CreateScope();

		// Resolving the authorization service using created scope, in which we inject AuthorizationService
		var authorizationService = scope.ServiceProvider.GetRequiredService<AuthorizationService>();

		// Then we use the tried and true approach of first obtaining the identity ID from the
		// authenticated user context. 
		var identityId = context.User.GetIdentityId();

		// Getting the set of permissions of user
		// The disadvantage with this approach to implement permission - based authorization
		// is that we have to query the database every time to obtain the set of permissions for the 
		// currently authenticated user. We can greatly improve this by introducing a caching mechanism in the authorization service.
		// The same applies to (CustomClaimsTransformation) our custom claims transformation class. 
		// var userRoles = await authorizationService.GetRolesForUserAsync(identityId);
		var permissions = await authorizationService.GetPermissionsForUserAsync(identityId);

		// Then we are searching for required permissions, and if they are found (match) we are going 
		// to call succeed on the authorization handler context and return from this method.
		if (permissions.Contains(requirement.Permission)) context.Succeed(requirement);
	}
}