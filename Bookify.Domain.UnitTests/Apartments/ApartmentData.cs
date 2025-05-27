using Bookify.Domain.Apartments;
using Bookify.Domain.Shared;

namespace Bookify.Domain.UnitTests.Apartments;

internal static class ApartmentData
{
	public static Apartment Create ( Money price, Money? cleaningFee = null ) => new(
			id : Guid.NewGuid(),
			name : new Name (
					Value : "Test apartment"
				),
			description : new Description (
					Value : "Test description"
				),
			address : new Address (
					Country : "Country",
					State : "State",
					ZipCode : "ZipCode",
					City : "City",
					Street : "Street"
				),
			price : price,
			cleaningFee : cleaningFee ?? Money.Zero(),
			amenities :
			[
			]
		);
}
