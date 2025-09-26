using Bookify.Modules.Bookings.Domain.Apartments;
using Bookify.Modules.Bookings.Domain.Bookings;
using Bookify.Modules.Bookings.Domain.Customers;
using Bookify.Modules.Bookings.Domain.Reviews;
using Bookify.Modules.Bookings.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Bookify.Modules.Bookings.Infrastructure;

public static class DependencyInjection
{
	public static IServiceCollection AddBookingsModule(this IServiceCollection services)
	{
		services.AddTransient<PricingService>();

		AddPersistence(services);

		return services;
	}

	private static void AddPersistence(IServiceCollection services)
	{
		services.AddScoped<IApartmentRepository, ApartmentRepository>();

		services.AddScoped<IBookingRepository, BookingRepository>();

		services.AddScoped<IReviewRepository, ReviewRepository>();

		services.AddScoped<ICustomerRepository, CustomerRepository>();
	}
}
