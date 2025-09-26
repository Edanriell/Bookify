using Bookify.Domain.Abstractions;

namespace Bookify.Modules.Bookings.Domain.Bookings.Events;

public sealed record BookingConfirmedDomainEvent(Guid BookingId) : IDomainEvent;
