using Bookify.Domain.Shared;
using Bookify.Modules.Bookings.Domain.Customers;

namespace Bookify.Application.UnitTests.Users;

internal static class CustomerData
{
	public static readonly Email Email = new("test@test.com");
	public static Guid CustomerId { get; set; } = Guid.NewGuid();

	public static Customer Create()
	{
		return Customer.Create(CustomerId, Email);
	}
}
