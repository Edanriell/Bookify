namespace Bookify.Application.Abstractions.Authentication;

// Contains information about current user
public interface IUserContext
{
	// Resource-based Authorization
	Guid UserId { get; }

	string IdentityId { get; }
}