using Bookify.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure;

public sealed class ApplicationDbContext : DbContext, IUnitOfWork
{
	// Constructor accepts the DBContextOption class, and it is passing it to the BaseClass constructor, accepting this type.
	public ApplicationDbContext(DbContextOptions options) : base(options)
	{
	}
}