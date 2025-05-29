using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Exceptions;
using Bookify.Domain.Abstractions;
using Bookify.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Bookify.Infrastructure;

public sealed class ApplicationDbContext : DbContext,
										   IUnitOfWork
{
	private static readonly JsonSerializerSettings JsonSerializerSettings = new()
																			{
																				TypeNameHandling = TypeNameHandling.All
																			};

	private readonly IDateTimeProvider _dateTimeProvider;

	public ApplicationDbContext ( DbContextOptions options,
								  IDateTimeProvider dateTimeProvider )
		: base (
				options : options
			)
	{
		_dateTimeProvider = dateTimeProvider;
	}

	public override async Task<int> SaveChangesAsync ( CancellationToken cancellationToken
														   = default(CancellationToken) )
	{
		try
		{
			AddDomainEventsAsOutboxMessages();

			var result = await base.SaveChangesAsync (
								 cancellationToken : cancellationToken
							 );

			return result;
		}
		catch ( DbUpdateConcurrencyException ex )
		{
			throw new ConcurrencyException (
					message : "Concurrency exception occurred.",
					innerException : ex
				);
		}
	}

	protected override void OnModelCreating ( ModelBuilder modelBuilder )
	{
		modelBuilder.ApplyConfigurationsFromAssembly (
				assembly : typeof(ApplicationDbContext).Assembly
			);

		base.OnModelCreating (
				modelBuilder : modelBuilder
			);
	}

	private void AddDomainEventsAsOutboxMessages()
	{
		var outboxMessages = ChangeTracker.Entries<Entity>().
			Select (
					selector : entry => entry.Entity
				).
			SelectMany (
					selector : entity =>
					{
						var domainEvents = entity.GetDomainEvents();

						entity.ClearDomainEvents();

						return domainEvents;
					}
				).
			Select (
					selector : domainEvent => new OutboxMessage (
							id : Guid.NewGuid(),
							occurredOnUtc : _dateTimeProvider.UtcNow,
							type : domainEvent.GetType().
								Name,
							content : JsonConvert.SerializeObject (
									value : domainEvent,
									settings : JsonSerializerSettings
								)
						)
				).
			ToList();

		AddRange (
				entities : outboxMessages
			);
	}
}
