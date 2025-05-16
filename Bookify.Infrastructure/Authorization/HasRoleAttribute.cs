using Microsoft.AspNetCore.Authorization;

namespace Bookify.Infrastructure.Authorization;

// Role-based Authorization
// Custom attribute, Alternative approach!
public sealed class HasRoleAttribute : AuthorizeAttribute
{
	public HasRoleAttribute(string role)
	{
		Roles = role;
	}

	public HasRoleAttribute(params string[] roles)
	{
		Roles = string.Join(",", roles);
	}
}