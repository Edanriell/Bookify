using Bookify.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure.Repositories;

// We have a generic constraint on our repository class which 
// requires that the generic type implements the entity base class.
internal abstract class Repository<T>
	where T : Entity
{
	protected readonly ApplicationDbContext DbContext;

	protected Repository(ApplicationDbContext dbContext)
	{
		DbContext = dbContext;
	}

	// Fetches an entity by the id
	public async Task<T?> GetByIdAsync(
		Guid id,
		CancellationToken cancellationToken = default)
	{
		return await DbContext
				  .Set<T>()
				  .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
	}

	// Adds an entity to the database context
//	public void Add(T entity)
//	{
//		DbContext.Add(entity);
//	}

	// Role-based Authorization
	// If we know how EF Core works under the hood
	// we might realize that we have a slight problem with how EF Core treats
	// existing objects acting as entities, in this case registered role.
	// EF Core will attempt to insert this role into the database which is going to
	// cause a duplicate key exception. 
	public virtual void Add(T entity)
	{
		DbContext.Add(entity);
	}
}