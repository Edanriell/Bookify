using Bookify.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure.Authorization;

// Role-based Authorization
internal sealed class AuthorizationService
{
	private readonly ApplicationDbContext _dbContext;

	public AuthorizationService(ApplicationDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	// Returns roles for the current user
	// identityId will come from the name identifier claim 
	public async Task<UserRolesResponse> GetRolesForUserAsync(string identityId)
	{
		var roles = await _dbContext.Set<User>()
					   .Where(user => user.IdentityId == identityId)
						// Projection
					   .Select(user => new UserRolesResponse
									   {
										   Id = user.Id,
										   Roles = user.Roles.ToList()
									   })
						// Materializing data from the database
					   .FirstAsync();

		return roles;
	}

	// Permission-based Authorization
	public async Task<HashSet<string>> GetPermissionsForUserAsync(string identityId)
	{
		// Fetch user
		// Filter user by identityId
		// Select all permissions from user's roles
		// All will be flattened into a collection of permissions
		var permissions = await _dbContext.Set<User>()
							 .Where(user => user.IdentityId == identityId)
							 .SelectMany(user => user.Roles.Select(role => role.Permissions))
							 .FirstAsync();

		// Converting data into HashSet by creating a permissions set variable,
		// and we are going to use our permissions to project the name of
		// permission, and we are going to materialize this into a hash set instance, which will get rid of any duplicates.
		var permissionsSet = permissions.Select(p => p.Name).ToHashSet();

		return permissionsSet;
	}
}