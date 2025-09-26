using Bookify.Infrastructure;
using Bookify.Infrastructure.Repositories;
using Bookify.Modules.Bookings.Domain.Apartments;

namespace Bookify.Modules.Bookings.Infrastructure.Repositories;

internal sealed class ApartmentRepository : Repository<Apartment>, IApartmentRepository
{
	public ApartmentRepository(ApplicationDbContext dbContext)
		: base(dbContext)
	{
	}
}
