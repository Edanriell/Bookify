using Bookify.Infrastructure;
using Bookify.Infrastructure.Repositories;
using Bookify.Modules.Bookings.Domain.Apartments;
using Bookify.Modules.Bookings.Domain.Bookings;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Modules.Bookings.Infrastructure.Repositories;

public sealed class BookingRepository : Repository<Booking>, IBookingRepository
{
	private static readonly BookingStatus[] ActiveBookingStatuses =
	{
		BookingStatus.Reserved,
		BookingStatus.Confirmed,
		BookingStatus.Completed
	};

	public BookingRepository(ApplicationDbContext dbContext)
		: base(dbContext)
	{
	}

	public async Task<bool> IsOverlappingAsync(
		Apartment apartment,
		DateRange duration,
		CancellationToken cancellationToken = default)
	{
		return await DbContext
			.Set<Booking>()
			.AnyAsync(
				booking =>
					booking.ApartmentId == apartment.Id &&
					booking.Duration.Start <= duration.End &&
					booking.Duration.End >= duration.Start &&
					ActiveBookingStatuses.Contains(booking.Status),
				cancellationToken);
	}
}
