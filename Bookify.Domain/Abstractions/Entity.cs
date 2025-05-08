namespace Bookify.Domain.Abstractions;

// Domain entity must have two important qualities, it should have an id (identity), also it must be continuous,
// meaning that an entity is important throughout the lifetime of our application, and because it has an id, it can evolve and change over time
// Abstract classes cannot have an instance, they must be inherited
public abstract class Entity
{
	// Property _domainEvents contains all domain events that are raised on this particular entity instance. 
	private readonly List<IDomainEvent> _domainEvents = new();

	protected Entity(Guid id)
	{
		Id = id;
	}

	// Fix for migrations
	protected Entity()
	{
	}

	// When we set init setter, it means once we define this entity, its id is set for a life
	// Two entities are equal if their Id is the same.
	// If this is very important for our system, we can override the Equals method and also implement the IEquatable interface
	public Guid Id { get; init; }

	// Method GetDomainEvents enables us to fetch all of the domain events that are raised on a particular entity instance.
	public IReadOnlyList<IDomainEvent> GetDomainEvents()
	{
		return _domainEvents.ToList();
	}

	// Method ClearDomainEvents clears all of the domain events that are raised on a particular entity instance.
	public void ClearDomainEvents()
	{
		_domainEvents.Clear();
	}

	// Method RaiseDomainEvent adds a domain event to the list of domain events that are raised on a particular entity instance.
	protected void RaiseDomainEvent(IDomainEvent domainEvent)
	{
		_domainEvents.Add(domainEvent);
	}
}