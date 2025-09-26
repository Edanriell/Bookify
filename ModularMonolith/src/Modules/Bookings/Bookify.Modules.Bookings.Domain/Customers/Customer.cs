using Bookify.Domain.Abstractions;
using Bookify.Domain.Shared;

namespace Bookify.Modules.Bookings.Domain.Customers;

public sealed class Customer : Entity
{
	private Customer()
	{
	}

	public Email Email { get; private set; }

	public static Customer Create(Guid id, Email email)
	{
		var customer = new Customer
		{
			Id = id,
			Email = email
		};

		return customer;
	}
}
