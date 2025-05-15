using Bookify.Application.Abstractions.Data;
using Bookify.Application.Abstractions.Messaging;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Bookings;
using Dapper;

namespace Bookify.Application.Apartments.SearchApartments;

internal sealed class SearchApartmentsQueryHandler
	: IQueryHandler<SearchApartmentsQuery, IReadOnlyList<ApartmentResponse>>
{
	private static readonly int[] ActiveBookingStatuses =
	{
		(int)BookingStatus.Reserved,
		(int)BookingStatus.Confirmed,
		(int)BookingStatus.Completed
	};

	private readonly ISqlConnectionFactory _sqlConnectionFactory;

	public SearchApartmentsQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
	{
		_sqlConnectionFactory = sqlConnectionFactory;
	}

	public async Task<Result<IReadOnlyList<ApartmentResponse>>> Handle(SearchApartmentsQuery request,
																	   CancellationToken cancellationToken)
	{
		// If StartDate is greater than EndDate we return an empty list.
		if (request.StartDate > request.EndDate) return new List<ApartmentResponse>();

		using var connection = _sqlConnectionFactory.CreateConnection();

		const string sql = """
						   SELECT
						       a.id AS Id,
						       a.name AS Name,
						       a.description AS Description,
						       a.price_amount AS Price,
						       a.price_currency AS Currency,
						       a.address_country AS Country,
						       a.address_state AS State,
						       a.address_zip_code AS ZipCode,
						       a.address_city AS City,
						       a.address_street AS Street
						   FROM apartments AS a
						   WHERE NOT EXISTS
						   (
						       SELECT 1
						       FROM bookings AS b
						       WHERE
						           b.apartment_id = a.id AND
						           b.duration_start <= @EndDate AND
						           b.duration_end >= @StartDate AND
						           b.status = ANY(@ActiveBookingStatuses)
						   )
						   """;

		// Sending the query to the database and mapping the response into the apartment response
		// Dapper allows us to query multiple objects from the database. We are querying for an apartment
		// response and an address response and the final result is an apartment response type. 
		var apartments = await connection
							.QueryAsync<ApartmentResponse, AddressResponse, ApartmentResponse>(
								 sql,
								 // This function allows us to handle the projection from the database.
								 (apartment, address) =>
								 {
									 // We want to stitch Apartment and Address together by saying that the
									 // apartment address is the address returned in the same row 
									 apartment.Address = address;

									 return apartment;
								 }, new
									{
										request.StartDate,
										request.EndDate,
										ActiveBookingStatuses
										// We can determine that the address starts from the country column 
										// so we are telling that to dapper by specifying the split on value to be Country
										// and what dapper is going to do is it's going to map first part of  properties into one object and second part properties to another object (Apartment and Address). 
									},
								 splitOn: "Country");

		return apartments.ToList();
	}
}