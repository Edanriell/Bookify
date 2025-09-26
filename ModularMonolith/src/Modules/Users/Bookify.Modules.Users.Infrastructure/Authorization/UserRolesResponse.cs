using Bookify.Modules.Users.Domain.Users;

namespace Bookify.Modules.Users.Infrastructure.Authorization;

internal sealed class UserRolesResponse
{
	public Guid UserId { get; init; }

	public List<Role> Roles { get; init; } = [];
}
