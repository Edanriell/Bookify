using Bookify.Domain.Abstractions;

namespace Bookify.Modules.Bookings.Domain.Customers;

public static class CustomerErrors
{
	public static readonly Error NotFound = new(
		"Customer.NotFound",
		"The user with the specified identifier was not found");
}
