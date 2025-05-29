using Microsoft.AspNetCore.Authorization;

namespace Bookify.Infrastructure.Authorization;

public sealed class HasRoleAttribute : AuthorizeAttribute
{
	public HasRoleAttribute ( string role ) { Roles = role; }

	public HasRoleAttribute ( params string[] roles )
	{
		Roles = string.Join (
				separator : ",",
				value : roles
			);
	}
}
