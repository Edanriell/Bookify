using Bookify.Domain.Shared;
using Bookify.Modules.Bookings.Domain.Apartments;

namespace Bookify.Domain.UnitTests.Apartments;

internal static class ApartmentData
{
	public static Apartment Create(Money price, Money? cleaningFee = null)
	{
		return new Apartment(
			Guid.NewGuid(),
			new Name("Test apartment"),
			new Description("Test description"),
			new Address("Country", "State", "ZipCode", "City", "Street"),
			price,
			cleaningFee ?? Money.Zero(),
			[]);
	}
}
