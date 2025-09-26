using Microsoft.AspNetCore.Authorization;

namespace Bookify.Modules.Users.Infrastructure.Authorization;

public sealed class HasPermissionAttribute : AuthorizeAttribute
{
	public HasPermissionAttribute(string permission)
		: base(permission)
	{
	}
}
