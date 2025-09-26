using Bookify.Application.Abstractions.Messaging;

namespace Bookify.Modules.Bookings.Application.Bookings.ConfirmBooking;

public sealed record ConfirmBookingCommand(Guid BookingId) : ICommand;
