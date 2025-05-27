using Bookify.Domain.Bookings;
using Bookify.Domain.Bookings.Events;
using Bookify.Domain.Shared;
using Bookify.Domain.UnitTests.Apartments;
using Bookify.Domain.UnitTests.Infrastructure;
using Bookify.Domain.UnitTests.Users;
using Bookify.Domain.Users;
using FluentAssertions;

namespace Bookify.Domain.UnitTests.Bookings;

public class BookingTests : BaseTest
{
	[ Fact ]
	public void Reserve_Should_RaiseBookingReservedDomainEvent()
	{
		// Arrange
		var user = User.Create (
				firstName : UserData.FirstName,
				lastName : UserData.LastName,
				email : UserData.Email
			);
		var price = new Money (
				Amount : 10.0m,
				Currency : Currency.Usd
			);
		var duration = DateRange.Create (
				start : new DateOnly (
						year : 2024,
						month : 1,
						day : 1
					),
				end : new DateOnly (
						year : 2024,
						month : 1,
						day : 10
					)
			);
		var apartment = ApartmentData.Create (
				price : price
			);
		var pricingService = new PricingService();

		// Act
		var booking = Booking.Reserve (
				apartment : apartment,
				userId : user.Id,
				duration : duration,
				utcNow : DateTime.UtcNow,
				pricingService : pricingService
			);

		// Assert
		var bookingReservedDomainEvent = AssertDomainEventWasPublished<BookingReservedDomainEvent> (
				entity : booking
			);

		bookingReservedDomainEvent.BookingId.Should().
			Be (
					expected : booking.Id
				);
	}
}
