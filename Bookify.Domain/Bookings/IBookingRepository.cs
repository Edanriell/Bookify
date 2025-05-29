using Bookify.Domain.Apartments;

namespace Bookify.Domain.Bookings;

public interface IBookingRepository
{
	Task<Booking?> GetByIdAsync ( Guid id, CancellationToken cancellationToken = default(CancellationToken) );

	Task<bool> IsOverlappingAsync ( Apartment apartment,
									DateRange duration,
									CancellationToken cancellationToken = default(CancellationToken) );

	void Add ( Booking booking );
}
