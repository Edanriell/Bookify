using Bookify.Domain.Bookings;
using Bookify.Domain.Shared;
using Bookify.Domain.UnitTests.Apartments;
using FluentAssertions;

namespace Bookify.Domain.UnitTests.Bookings;

public class PricingServiceTests
{
	[ Fact ]
	public void CalculatePrice_Should_ReturnCorrectTotalPrice()
	{
		// Arrange
		var price = new Money (
				Amount : 10.0m,
				Currency : Currency.Usd
			);
		var period = DateRange.Create (
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
		var expectedTotalPrice = new Money (
				Amount : price.Amount * period.LengthInDays,
				Currency : price.Currency
			);
		var apartment = ApartmentData.Create (
				price : price
			);
		var pricingService = new PricingService();

		// Act
		var pricingDetails = pricingService.CalculatePrice (
				apartment : apartment,
				period : period
			);

		// Assert
		pricingDetails.TotalPrice.Should().
			Be (
					expected : expectedTotalPrice
				);
	}

	[ Fact ]
	public void CalculatePrice_Should_ReturnCorrectTotalPrice_WhenCleaningFeeIsIncluded()
	{
		// Arrange
		var price = new Money (
				Amount : 10.0m,
				Currency : Currency.Usd
			);
		var cleaningFee = new Money (
				Amount : 99.99m,
				Currency : Currency.Usd
			);
		var period = DateRange.Create (
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
		var expectedTotalPrice = new Money (
				Amount : price.Amount * period.LengthInDays + cleaningFee.Amount,
				Currency : price.Currency
			);
		var apartment = ApartmentData.Create (
				price : price,
				cleaningFee : cleaningFee
			);
		var pricingService = new PricingService();

		// Act
		var pricingDetails = pricingService.CalculatePrice (
				apartment : apartment,
				period : period
			);

		// Assert
		pricingDetails.TotalPrice.Should().
			Be (
					expected : expectedTotalPrice
				);
	}
}
