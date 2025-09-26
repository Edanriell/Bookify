using Bookify.Domain.Shared;
using Bookify.Domain.UnitTests.Apartments;
using Bookify.Modules.Bookings.Domain.Apartments;
using Bookify.Modules.Bookings.Domain.Bookings;
using FluentAssertions;

namespace Bookify.Domain.UnitTests.Bookings;

public class PricingServiceTests
{
	[Fact]
	public void CalculatePrice_Should_ReturnCorrectTotalPrice()
	{
		// Arrange
		var price = new Money(10.0m, Currency.Usd);
		var period = DateRange.Create(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 10));
		var expectedTotalPrice = new Money(price.Amount * period.LengthInDays, price.Currency);
		Apartment apartment = ApartmentData.Create(price);
		var pricingService = new PricingService();

		// Act
		PricingDetails pricingDetails = pricingService.CalculatePrice(apartment, period);

		// Assert
		pricingDetails.TotalPrice.Should().Be(expectedTotalPrice);
	}

	[Fact]
	public void CalculatePrice_Should_ReturnCorrectTotalPrice_WhenCleaningFeeIsIncluded()
	{
		// Arrange
		var price = new Money(10.0m, Currency.Usd);
		var cleaningFee = new Money(99.99m, Currency.Usd);
		var period = DateRange.Create(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 10));
		decimal basePrice = price.Amount * period.LengthInDays;
		decimal totalAmount = basePrice + cleaningFee.Amount;
		var expectedTotalPrice = new Money(totalAmount, price.Currency);
		Apartment apartment = ApartmentData.Create(price, cleaningFee);
		var pricingService = new PricingService();

		// Act
		PricingDetails pricingDetails = pricingService.CalculatePrice(apartment, period);

		// Assert
		pricingDetails.TotalPrice.Should().Be(expectedTotalPrice);
	}
}
