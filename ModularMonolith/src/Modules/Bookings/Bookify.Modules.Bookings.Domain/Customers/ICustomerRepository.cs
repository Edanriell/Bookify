namespace Bookify.Modules.Bookings.Domain.Customers;

public interface ICustomerRepository
{
	Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
