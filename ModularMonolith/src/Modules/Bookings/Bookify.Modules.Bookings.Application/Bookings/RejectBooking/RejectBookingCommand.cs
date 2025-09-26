using Bookify.Application.Abstractions.Messaging;

namespace Bookify.Modules.Bookings.Application.Bookings.RejectBooking;

public sealed record RejectBookingCommand(Guid BookingId) : ICommand;
