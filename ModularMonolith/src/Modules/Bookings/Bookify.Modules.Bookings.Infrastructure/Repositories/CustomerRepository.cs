using Bookify.Domain.Shared;
using Bookify.Modules.Bookings.Domain.Customers;
using Bookify.Modules.Users.Api;

namespace Bookify.Modules.Bookings.Infrastructure.Repositories;

internal sealed class CustomerRepository : ICustomerRepository
{
	private readonly IUsersApi _usersApi;

	public CustomerRepository(IUsersApi usersApi)
	{
		_usersApi = usersApi;
	}

	public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
	{
		UserResponse? user = await _usersApi.GetAsync(id, cancellationToken);

		if (user is null)
		{
			return null;
		}

		return Customer.Create(user.Id, new Email(user.Email));
	}
}
