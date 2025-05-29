namespace Bookify.Domain.Users;

public interface IUserRepository
{
	Task<User?> GetByIdAsync ( Guid id, CancellationToken cancellationToken = default(CancellationToken) );

	void Add ( User user );
}
