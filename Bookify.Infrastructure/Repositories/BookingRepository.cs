using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure.Repositories;

internal sealed class BookingRepository : Repository<Booking>, IBookingRepository
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

	// We have to implement this method in the repository itself
	// because it's not satisfied by the generic repository.
	// This method implements the optimistic check to see if there are any bookings for this apartment in the specified date range
	// 
	public async Task<bool> IsOverlappingAsync(
		Apartment apartment,
		DateRange duration,
		CancellationToken cancellationToken = default)
	{
		// We need to check that the booking status for this date range if it exists is one of the statuses
		// that mean that there is an overlap such as the reserve confirmed and completed status. 
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