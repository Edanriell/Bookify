using Bookify.Application.Abstractions.Messaging;

namespace Bookify.Modules.Bookings.Application.Bookings.CancelBooking;

public sealed record CancelBookingCommand(Guid BookingId) : ICommand;
