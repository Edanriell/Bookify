using Bookify.Domain.Abstractions;

namespace Bookify.Modules.Users.Domain.Users.Events;

public sealed record UserCreatedDomainEvent(Guid UserId) : IDomainEvent;
