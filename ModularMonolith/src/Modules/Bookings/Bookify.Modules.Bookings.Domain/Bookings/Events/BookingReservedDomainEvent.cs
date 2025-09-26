using Bookify.Domain.Abstractions;

namespace Bookify.Modules.Bookings.Domain.Bookings.Events;

public sealed record BookingReservedDomainEvent(Guid BookingId) : IDomainEvent;
