using Bookify.Domain.Abstractions;

namespace Bookify.Modules.Bookings.Domain.Bookings.Events;

public sealed record BookingCancelledDomainEvent(Guid BookingId) : IDomainEvent;
