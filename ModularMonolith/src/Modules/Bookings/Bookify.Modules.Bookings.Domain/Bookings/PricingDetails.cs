using Bookify.Domain.Shared;

namespace Bookify.Modules.Bookings.Domain.Bookings;

public sealed record PricingDetails(
	Money PriceForPeriod,
	Money CleaningFee,
	Money AmenitiesUpCharge,
	Money TotalPrice);
