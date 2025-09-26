using Bookify.Infrastructure;
using Bookify.Infrastructure.Repositories;
using Bookify.Modules.Bookings.Domain.Reviews;

namespace Bookify.Modules.Bookings.Infrastructure.Repositories;

internal sealed class ReviewRepository : Repository<Review>, IReviewRepository
{
	public ReviewRepository(ApplicationDbContext dbContext)
		: base(dbContext)
	{
	}
}
