using Bookify.Infrastructure;
using Bookify.Infrastructure.Repositories;
using Bookify.Modules.Users.Domain.Users;

namespace Bookify.Modules.Users.Infrastructure.Repositories;

public sealed class UserRepository : Repository<User>, IUserRepository
{
	public UserRepository(ApplicationDbContext dbContext)
		: base(dbContext)
	{
	}

	public override void Add(User user)
	{
		foreach (Role role in user.Roles)
		{
			DbContext.Attach(role);
		}

		DbContext.Add(user);
	}
}
