using Bookify.Application.Abstractions.Data;
using Bookify.Application.Abstractions.Messaging;
using Bookify.Domain.Abstractions;
using Dapper;

namespace Bookify.Application.Bookings.GetBooking;

// GetBookingQuery is the query argument and BookingResponse is the result by returning this query. 
internal sealed class GetBookingQueryHandler : IQueryHandler<GetBookingQuery, BookingResponse>
{
	private readonly ISqlConnectionFactory _sqlConnectionFactory;

	public GetBookingQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
	{
		_sqlConnectionFactory = sqlConnectionFactory;
	}

	public async Task<Result<BookingResponse>> Handle(GetBookingQuery request, CancellationToken cancellationToken)
	{
		// Getting back a database connection object.
		using var connection = _sqlConnectionFactory.CreateConnection();

		// Our query with the argument BookingId. 
		// This is considered to be a pragmatic solution because we are using SQL
		// to directly access our read model and the response from our query without any indirection. 
		// The downside is that we are not completely abstracting our persistence concerns, in this case, a SQL database
		// from our query handlers, but the benefits are just too many to count. 
		// First of all, this is very simple. All we need is to create a database connection, write our SQL statement and
		// execute the query with dapper, which is a few lines of code. Then this is going to be really performant because dapper
		// is one of the fastest object mappers out there, and there is also the benefit of being able to define our read models
		// at the database level by defining database views. This is something that our SQL developer can take care of, and we just consume
		// the views without having to write very complicated queries in our code. The standard argument of not being able to switch a database
		// without an abstraction really doesn't make a lot of sense because first of all, if we are switching databases, we have a lot more
		// problems than just using SQL in the application layer. Second, if we end up moving from an SQL database to something like a document
		// database or a column store, we are going to have to rewrite our entire persistence layer, so an abstraction wouldn't really save us here
		// and lastly, if we do decide to introduce an abstraction to just wrap all of the code of our query and more (code below), then query handlers become pointless.
		// They would just be wrappers around the query service that is delegating all of the work to the infrastructure project, for example. 
		// But the query handlers themselves wouldn't have any added value, so at that point we could consider getting rid of the query handlers altogether. 
		const string sql = """
						   SELECT
						       id AS Id,
						       apartment_id AS ApartmentId,
						       user_id AS UserId,
						       status AS Status,
						       price_for_period_amount AS PriceAmount,
						       price_for_period_currency AS PriceCurrency,
						       cleaning_fee_amount AS CleaningFeeAmount,
						       cleaning_fee_currency AS CleaningFeeCurrency,
						       amenities_up_charge_amount AS AmenitiesUpChargeAmount,
						       amenities_up_charge_currency AS AmenitiesUpChargeCurrency,
						       total_price_amount AS TotalPriceAmount,
						       total_price_currency AS TotalPriceCurrency,
						       duration_start AS DurationStart,
						       duration_end AS DurationEnd,
						       created_on_utc AS CreatedOnUtc
						   FROM bookings
						   WHERE id = @BookingId
						   """;

		// Executing query with dapper 
		var booking = await connection.QueryFirstOrDefaultAsync<BookingResponse>(
						  sql,
						  // Anonymous object contains parameters for query which is the BookingId
						  // because we are trying to fetch booking by the ID.
						  new
						  {
							  request.BookingId
						  });

		//  returning a booking object
		return booking;
	}
}