using MediatR;

namespace Bookify.Domain.Abstractions;

// Interface representing all domain events in our system
// Domain event is something of significance that has occurred in the domain, and we want to notify
// other components in our system that something has happened.

// MediatR notifications are used to implement the publish subscribe pattern.
// We publish our domain event, and we could have one or more subscribers to this event that want to handle it.
public interface IDomainEvent : INotification
{
}