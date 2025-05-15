using Bookify.Application.Exceptions;
using Bookify.Domain.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure;

public sealed class ApplicationDbContext : DbContext, IUnitOfWork
{
	private readonly IPublisher _publisher;

	// Constructor accepts the DBContextOption class, and it is passing it to the BaseClass constructor, accepting this type.
	public ApplicationDbContext(DbContextOptions options, IPublisher publisher)
		: base(options)
	{
		_publisher = publisher;
	}

	public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		try
		{
			// We are calling the base save changes async method, then
			var result = await base.SaveChangesAsync(cancellationToken);

			// We are going to await PublishDomainEvents
			// There are some caveats of this approach to publish domain events.
			// What we are doing here is potentially problematic. We are saving changes to the database, and okay this is atomic,
			// and this is either going to succeed or fail, but when we are publishing a set of domain events which is another
			// transaction altogether. The domain event handlers could be doing all sorts of things like
			// calling the database or other external services, and the handlers themselves could also fail. 
			// This is going to cause an exception which is going to fail the SaveChangesAsync method. 
			// However, the original call to the database was already successfully completed. So in the case of a domain event handler
			// failure, it would appear as though the whole transaction failed, but that wouldn't be the case. 
			// This is a major concern with this approach, and it's something that we need to be aware of if we want to be running this
			// in production. There are also implementations that go ahead and publish the domain events before calling SaveChangesAsync The reasoning
			// is then the handling of the domain events becomes part of the overall transaction by EF Core. While this is inherently true, the 
			// whole concept of an event is something that is a fact. An event is something that has already happened in our system, and publishing
			// the domain events before actually persiting the original changes that triggered these events makes absolutely no sense. 
			// Which is why we go ahead and publish our domain events only after persisting these changes in the database. 
			// But there is a fix OutBox pattern!
			await PublishDomainEventsAsync();

			return result;
		}
		catch (DbUpdateConcurrencyException ex)
		{
			// We are catching dbUpdateConcurrencyException. This is an exception that is 
			// thrown by the database when we have a concurrency violation at the database level. 
			// The reason for creating a custom exception is so that, we don't leak EntityFramework details into our
			// application layer, we are abstracting EntityFramework behind this custom exception. 
			// However, we are passing the exception instance as the inner exception so that it's available for logging
			// and further inspecting. 
			// So now our database context, which is unit of work, is going to be handling the DbUpdateConcurrencyException
			// and throwing a concurrency exception instance, which is known to our application project. 
			throw new ConcurrencyException("Concurrency exception occurred.", ex);
		}
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

	private async Task PublishDomainEventsAsync()
	{
		// Using ChangeTracker on EF Core to grab the entity entries which
		var domainEvents = ChangeTracker
			// implement the entity class
		   .Entries<Entity>()
			// We are selecting the actual entity entry instance 
			// which is our entity clas, then we are calling SelectMany
		   .Select(entry => entry.Entity)
			// To map the list of domain events from each entity. 
		   .SelectMany(entity =>
			{
				// We are calling entity.GetDomainEvents and saving them in a variable
				var domainEvents = entity.GetDomainEvents();

				// then we are clearing the domain events from our entities
				// IMPORTANT! Clearing events is important because when we publish the domain events,
				// we don't know what could be happening in the handlers.
				// There could be another database context created that could use the
				// same entity and add another domain event, and this is going to 
				// cause strange behavior. 
				entity.ClearDomainEvents();

				// and returning the domain events
				return domainEvents;
			})
		   .ToList();

		// We have domain events extracted, and then we just iterate over them
		// one by one and call publisherPublish to publish the domain event.
		// This is going to trigger the respective domain event handlers which we defined in the
		// application layer. 
		foreach (var domainEvent in domainEvents) await _publisher.Publish(domainEvent);
	}
}