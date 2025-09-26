using System.Data;
using Bookify.Application.Abstractions.Data;
using Bookify.Application.Abstractions.Messaging;
using Bookify.Domain.Abstractions;
using Dapper;

namespace Bookify.Modules.Bookings.Application.Reviews.GetReview;

internal sealed class GetAllReviewsQueryHandler : IQueryHandler<GetAllReviewsQuery, IReadOnlyList<ReviewResponse>>
{
	private readonly ISqlConnectionFactory _sqlConnectionFactory;

	public GetAllReviewsQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
	{
		_sqlConnectionFactory = sqlConnectionFactory;
	}

	public async Task<Result<IReadOnlyList<ReviewResponse>>> Handle(
		GetAllReviewsQuery request, CancellationToken cancellationToken)
	{
		using IDbConnection connection = _sqlConnectionFactory.CreateConnection();

		string sql = """
		             SELECT
		                 id AS Id,
		                 apartment_id AS ApartmentId,
		                 booking_id AS BookingId,
		                 user_id AS UserId,
		                 rating AS Rating,
		                 comment AS Comment,
		                 created_on_utc AS CreatedOnUtc
		             FROM bookings.reviews
		             """;

		IEnumerable<ReviewResponse> reviews = await connection.QueryAsync<ReviewResponse>(sql);

		return reviews.AsList();
	}
}
