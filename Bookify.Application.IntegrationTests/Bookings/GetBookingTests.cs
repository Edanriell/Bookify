using Bookify.Application.Bookings.GetBooking;
using Bookify.Application.IntegrationTests.Infrastructure;
using Bookify.Domain.Bookings;
using FluentAssertions;

namespace Bookify.Application.IntegrationTests.Bookings;

public class GetBookingTests : BaseIntegrationTest
{
	private static readonly Guid BookingId = Guid.NewGuid();

	public GetBookingTests ( IntegrationTestWebAppFactory factory )
		: base (
				factory : factory
			)
	{
	}

	[ Fact ]
	public async Task GetBooking_ShouldReturnFailure_WhenBookingIsNotFound()
	{
		// Arrange
		var query = new GetBookingQuery (
				BookingId : BookingId
			);

		// Act
		var result = await Sender.Send (
							 request : query
						 );

		// Assert
		result.Error.Should().
			Be (
					expected : BookingErrors.NotFound
				);
	}
}
