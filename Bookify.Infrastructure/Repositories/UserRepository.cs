using Bookify.Domain.Users;

namespace Bookify.Infrastructure.Repositories;

internal sealed class UserRepository : Repository<User>,
									   IUserRepository
{
	public UserRepository ( ApplicationDbContext dbContext )
		: base (
				dbContext : dbContext
			)
	{
	}

	public override void Add ( User user )
	{
		foreach ( var role in user.Roles )
			DbContext.Attach (
					entity : role
				);

		DbContext.Add (
				entity : user
			);
	}
}
