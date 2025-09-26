using Bookify.Application.Abstractions.Messaging;
using Bookify.Modules.Bookings.Application.Bookings.GetBooking;

namespace Bookify.Modules.Bookings.Application.Bookings.GetBookings;

public sealed record GetAllBookingsQuery : IQuery<IReadOnlyList<BookingResponse>>;
