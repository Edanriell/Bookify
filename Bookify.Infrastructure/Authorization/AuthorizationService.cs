using Bookify.Application.Abstractions.Caching;
using Bookify.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure.Authorization;

// Role-based Authorization
internal sealed class AuthorizationService
{
	private readonly ICacheService _cacheService;
	private readonly ApplicationDbContext _dbContext;

	public AuthorizationService ( ApplicationDbContext dbContext, ICacheService cacheService )
	{
		_dbContext = dbContext;
		_cacheService = cacheService;
	}

	// Returns roles for the current user
	// identityId will come from the name identifier claim 
	// Caching
	// Here is implemented the cache aside pattern, where we are first trying to
	// access the data in the cache and returning it if it is there. Otherwise we are fetching the data from
	// the database and then caching it for subsequent requests.
	public async Task<UserRolesResponse> GetRolesForUserAsync ( string identityId )
	{
		// Defining a cache key value, which is going to be an interpolated string. 
		// IdentityId is specific for the current user!
		var cacheKey = $"auth:roles-{identityId}";

		// We are trying to get cached roles from our cache if they are present
		var cachedRoles = await _cacheService.GetAsync<UserRolesResponse> (
								  key : cacheKey
							  );

		// If cached roles are not null, then we are going to return them from this method.
		// This means that we encountered a cache hit, and we can return the cached value.
		if ( cachedRoles is not null )
			return cachedRoles;

		// Caching
		// Otherwise we are going to run the database query as normal, get the roles from the database
		var roles = await _dbContext.Set<User>().
						Where (
								predicate : user => user.IdentityId == identityId
							)
						// Projection
					   .
						Select (
								selector : user => new UserRolesResponse
												   {
													   Id = user.Id,
													   Roles = user.Roles.ToList()
												   }
							)
						// Materializing data from the database
					   .
						FirstAsync();

		// Caching
		// And the we are going to cache this value by calling cacheService setAsync, and we will pass in the cache key
		// the roles as the value
		await _cacheService.SetAsync (
				key : cacheKey,
				value : roles
			);

		return roles;
	}

	// Permission-based Authorization
	public async Task<HashSet<string>> GetPermissionsForUserAsync ( string identityId )
	{
		var cacheKey = $"auth:permissions-{identityId}";

		var cachedPermissions = await _cacheService.GetAsync<HashSet<string>> (
										key : cacheKey
									);

		if ( cachedPermissions is not null )
			return cachedPermissions;

		// Fetch user
		// Filter user by identityId
		// Select all permissions from user's roles
		// All will be flattened into a collection of permissions
		var permissions = await _dbContext.Set<User>().
							  Where (
									  predicate : user => user.IdentityId == identityId
								  ).
							  SelectMany (
									  selector : user => user.Roles.Select (
											  role => role.Permissions
										  )
								  ).
							  FirstAsync();

		// Converting data into HashSet by creating a permissions set variable,
		// and we are going to use our permissions to project the name of
		// permission, and we are going to materialize this into a hash set instance, which will get rid of any
		// duplicates.
		var permissionsSet = permissions.Select (
					selector : p => p.Name
				).
			ToHashSet();

		await _cacheService.SetAsync (
				key : cacheKey,
				value : permissionsSet
			);

		return permissionsSet;
	}
}
