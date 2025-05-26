using Bookify.Application.Abstractions.Caching;

namespace Bookify.Application.Bookings.GetBooking;

// public sealed record GetBookingQuery(Guid BookingId) : IQuery<BookingResponse>;

// Caching
public sealed record GetBookingQuery ( Guid BookingId ) : ICachedQuery<BookingResponse>
{
	// We are using bookingId to create a dynamic cache key.
	public string CacheKey => $"bookings-{BookingId}";

	// If null here is peresent we are using the default value which is 1 minute
	public TimeSpan? Expiration => null;
}
