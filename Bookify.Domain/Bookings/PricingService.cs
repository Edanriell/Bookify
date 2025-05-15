using Bookify.Domain.Apartments;

namespace Bookify.Domain.Bookings;

public class PricingService
{
	public PricingDetails CalculatePrice(Apartment apartment, DateRange period)
	{
		// Saving currency of apartment price
		var currency = apartment.Price.Currency;

		// Calculating price for periods length in days effectively multiplying the apartment price by the duration of the stay
		var priceForPeriod = new Money(
			apartment.Price.Amount * period.LengthInDays,
			currency);

		// Calculating upcharge for specific amenities that are available on the apartment
		// The business decided that a garden or a mountain view incur a 5% upcharge
		// air conditioning is a 1% upcharge, and parking is a 1% upcharge
		// Add these all together to the final upcharge
		decimal percentageUpCharge = 0;
		foreach (var amenity in apartment.Amenities)
			percentageUpCharge += amenity switch
								  {
									  Amenity.GardenView or Amenity.MountainView => 0.05m,
									  Amenity.AirConditioning => 0.01m,
									  Amenity.Parking => 0.01m,
									  _ => 0
								  };

		var amenitiesUpCharge = Money.Zero(currency);
		// If upcharge is greater then zero, we apply the upcharge to the price for the
		// upcharge to the  price for the current period by multiplying the percentage points
		if (percentageUpCharge > 0)
			amenitiesUpCharge = new Money(
				priceForPeriod.Amount * percentageUpCharge,
				currency);

		// Calculating total price starting from zero
		var totalPrice = Money.Zero(currency);

		// Adding price for period
		totalPrice += priceForPeriod;

		// If apartment fee is not zero, then also we are adding cleaning fee
		if (!apartment.CleaningFee.IsZero()) totalPrice += apartment.CleaningFee;

		// Adding amenities up charge to the total price regardless
		totalPrice += amenitiesUpCharge;

		return new PricingDetails(priceForPeriod, apartment.CleaningFee, amenitiesUpCharge, totalPrice);
	}
}