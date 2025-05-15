namespace Bookify.Domain.Abstractions;

// The UnitOfWork and the repository pattern are necessary for defining
// a rich domain model 
public interface IUnitOfWork
{
	// Method SaveChangesAsync takes any changes that are pending in our repositories and persist
	// it in the database. 
	Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
} 