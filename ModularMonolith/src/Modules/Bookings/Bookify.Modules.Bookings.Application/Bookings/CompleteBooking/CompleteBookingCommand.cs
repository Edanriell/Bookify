using Bookify.Application.Abstractions.Messaging;

namespace Bookify.Modules.Bookings.Application.Bookings.CompleteBooking;

public sealed record CompleteBookingCommand(Guid BookingId) : ICommand;
