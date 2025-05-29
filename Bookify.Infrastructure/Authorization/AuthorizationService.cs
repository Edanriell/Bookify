using Bookify.Application.Abstractions.Caching;
using Bookify.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure.Authorization;

internal sealed class AuthorizationService
{
	private readonly ICacheService _cacheService;
	private readonly ApplicationDbContext _dbContext;

	public AuthorizationService ( ApplicationDbContext dbContext, ICacheService cacheService )
	{
		_dbContext = dbContext;
		_cacheService = cacheService;
	}

	public async Task<UserRolesResponse> GetRolesForUserAsync ( string identityId )
	{
		var cacheKey = $"auth:roles-{identityId}";
		var cachedRoles = await _cacheService.GetAsync<UserRolesResponse> (
								  key : cacheKey
							  );

		if ( cachedRoles is not null )
			return cachedRoles;

		UserRolesResponse roles = await _dbContext.Set<User>().
									  Where (
											  predicate : u => u.IdentityId == identityId
										  ).
									  Select (
											  u => new UserRolesResponse
												   {
													   UserId = u.Id,
													   Roles = u.Roles.ToList()
												   }
										  ).
									  FirstAsync();

		await _cacheService.SetAsync (
				key : cacheKey,
				value : roles
			);

		return roles;
	}

	public async Task<HashSet<string>> GetPermissionsForUserAsync ( string identityId )
	{
		var cacheKey = $"auth:permissions-{identityId}";
		var cachedPermissions = await _cacheService.GetAsync<HashSet<string>> (
										key : cacheKey
									);

		if ( cachedPermissions is not null )
			return cachedPermissions;

		var permissions = await _dbContext.Set<User>().
							  Where (
									  predicate : u => u.IdentityId == identityId
								  ).
							  SelectMany (
									  selector : u => u.Roles.Select (
											  r => r.Permissions
										  )
								  ).
							  FirstAsync();

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
