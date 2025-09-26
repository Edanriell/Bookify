using System.Data;
using Bookify.Application.Abstractions.Data;
using Bookify.Application.Abstractions.Messaging;
using Bookify.Domain.Abstractions;
using Bookify.Modules.Bookings.Application.Bookings.GetBooking;
using Dapper;

namespace Bookify.Modules.Bookings.Application.Bookings.GetBookings;

internal sealed class GetAllBookingsQueryHandler : IQueryHandler<GetAllBookingsQuery, IReadOnlyList<BookingResponse>>
{
	private readonly ISqlConnectionFactory _sqlConnectionFactory;

	public GetAllBookingsQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
	{
		_sqlConnectionFactory = sqlConnectionFactory;
	}

	public async Task<Result<IReadOnlyList<BookingResponse>>> Handle(
		GetAllBookingsQuery request, CancellationToken cancellationToken)
	{
		using IDbConnection connection = _sqlConnectionFactory.CreateConnection();

		string sql = """
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
		             FROM bookings.bookings
		             """;

		IEnumerable<BookingResponse> bookings = await connection.QueryAsync<BookingResponse>(sql);

		return bookings.AsList();
	}
}
