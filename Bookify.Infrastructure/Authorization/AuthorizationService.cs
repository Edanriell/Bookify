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
}