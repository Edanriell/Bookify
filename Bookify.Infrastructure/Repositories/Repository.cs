using Bookify.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure.Repositories;

// We have a generic constraint on our repository class which 
// requires that the generic type implements the entity base class.
internal abstract class Repository<T> where T : Entity
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
		return await DbContext.Set<T>().FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
	}

	// Adds an entity to the database context
	public void Add(T entity)
	{
		DbContext.Add(entity);
	}
}