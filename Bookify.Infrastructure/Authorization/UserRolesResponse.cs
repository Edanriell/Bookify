using Bookify.Domain.Users;

namespace Bookify.Infrastructure.Authorization;

// Role-based Authorization 
public sealed class UserRolesResponse
{
	// User id
	public Guid Id { get; init; }

	// Roles assigned to the user, default value empty list
	public List<Role> Roles { get; init; } = [];
}