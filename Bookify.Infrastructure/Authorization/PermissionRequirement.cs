using Microsoft.AspNetCore.Authorization;

namespace Bookify.Infrastructure.Authorization;

// Permission-based Authorization
internal sealed class PermissionRequirement : IAuthorizationRequirement
{
	public PermissionRequirement(string permission)
	{
		Permission = permission;
	}

	public string Permission { get; }
}