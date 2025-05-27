using Bookify.Domain.Apartments;
using Bookify.Domain.Shared;

namespace Bookify.Application.UnitTests.Apartments;

internal static class ApartmentData
{
	public static Apartment Create() => new(
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
			price : new Money (
					Amount : 100.0m,
					Currency : Currency.Usd
				),
			cleaningFee : Money.Zero(),
			amenities :
			[
			]
		);
}
