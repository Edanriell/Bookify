using Bookify.Domain.Abstractions;

namespace Bookify.Modules.Bookings.Domain.Bookings.Events;

public sealed record BookingCompletedDomainEvent(Guid BookingId) : IDomainEvent;
