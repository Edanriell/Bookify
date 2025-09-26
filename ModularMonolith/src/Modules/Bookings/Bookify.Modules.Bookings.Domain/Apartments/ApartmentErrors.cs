using Bookify.Domain.Abstractions;

namespace Bookify.Modules.Bookings.Domain.Apartments;

public static class ApartmentErrors
{
	public static readonly Error NotFound = new(
		"Apartment.NotFound",
		"The apartment with the specified identifier was not found");
}
