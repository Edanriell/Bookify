using Bookify.Domain.Shared;
using Bookify.Domain.UnitTests.Apartments;
using Bookify.Domain.UnitTests.Infrastructure;
using Bookify.Domain.UnitTests.Users;
using Bookify.Modules.Bookings.Domain.Apartments;
using Bookify.Modules.Bookings.Domain.Bookings;
using Bookify.Modules.Bookings.Domain.Bookings.Events;
using Bookify.Modules.Users.Domain.Users;
using FluentAssertions;

namespace Bookify.Domain.UnitTests.Bookings;

public class BookingTests : BaseTest
{
	[Fact]
	public void Reserve_Should_RaiseBookingReservedDomainEvent()
	{
		// Arrange
		var user = User.Create(UserData.FirstName, UserData.LastName, UserData.Email);
		var price = new Money(10.0m, Currency.Usd);
		var duration = DateRange.Create(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 10));
		Apartment apartment = ApartmentData.Create(price);
		var pricingService = new PricingService();

		// Act  
		var booking = Booking.Reserve(apartment, user.Id, duration, DateTime.UtcNow, pricingService);

		// Assert
		BookingReservedDomainEvent bookingReservedDomainEvent =
			AssertDomainEventWasPublished<BookingReservedDomainEvent>(booking);

		bookingReservedDomainEvent.BookingId.Should().Be(booking.Id);
	}
}
