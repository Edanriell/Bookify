using Bookify.Domain.Users;

namespace Bookify.Infrastructure.Repositories;

internal sealed class UserRepository : Repository<User>, IUserRepository
{
	public UserRepository(ApplicationDbContext dbContext)
		: base(dbContext)
	{
	}

	// Role-based Authorization
	// Here we are iterating over all of the roles on our user
	// entity and attach them to the database context. 
	// This will tell EF Core that any roles present on our user
	// object are already inside of the database and you don't need
	// to insert them again. 
	public override void Add(User user)
	{
		foreach (var role in user.Roles) DbContext.Attach(role);

		// We are adding user to the change tracker, that it is going
		// to be persisted when we call save changes async. 
		DbContext.Add(user);
	}
}