using Bookify.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure;

public sealed class ApplicationDbContext : DbContext, IUnitOfWork
{
	// Constructor accepts the DBContextOption class, and it is passing it to the BaseClass constructor, accepting this type.
	public ApplicationDbContext(DbContextOptions options) : base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		// We are specifying the assembly containing our database context. 
		// When our model is being configured, it's going to scan this assembly
		// find our entity configurations that we added in the configurations folder, and
		// apply them to the entity framework data model. 
		// IMPORTANT! There is no risk to forget to apply our entity configuration, because it's going
		// to be applied automatically. 
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

		base.OnModelCreating(modelBuilder);
	}
}