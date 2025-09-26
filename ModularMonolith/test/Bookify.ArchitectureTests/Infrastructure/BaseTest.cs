using System.Reflection;
using Bookify.Modules.Bookings.Application.Bookings.GetBooking;
using Bookify.Modules.Bookings.Domain.Bookings;
using Bookify.Modules.Bookings.Infrastructure.Repositories;
using Bookify.Modules.Users.Application.Users.GetLoggedInUser;
using Bookify.Modules.Users.Domain.Users;
using Bookify.Modules.Users.Infrastructure.Repositories;

namespace Bookify.ArchitectureTests.Infrastructure;

public abstract class BaseTest
{
	protected const string ApiNamespace = "Bookify.Api";

	protected const string UsersNamespace = "Bookify.Modules.Users";
	protected const string UsersPublicApiNamespace = "Bookify.Modules.Users.Api";

	protected const string BookingsNamespace = "Bookify.Modules.Bookings";
	protected const string BookingsPublicApiNamespace = "Bookify.Modules.Bookings.Api";

	public static class Users
	{
		public static readonly Assembly ApplicationAssembly = typeof(GetLoggedInUserQuery).Assembly;
		public static readonly Assembly DomainAssembly = typeof(User).Assembly;
		public static readonly Assembly InfrastructureAssembly = typeof(UserRepository).Assembly;
		public static readonly Assembly PresentationAssembly = typeof(Program).Assembly;
	}

	public static class Bookings
	{
		public static readonly Assembly ApplicationAssembly = typeof(GetBookingQuery).Assembly;
		public static readonly Assembly DomainAssembly = typeof(Booking).Assembly;
		public static readonly Assembly InfrastructureAssembly = typeof(BookingRepository).Assembly;
		public static readonly Assembly PresentationAssembly = typeof(Program).Assembly;
	}
}
