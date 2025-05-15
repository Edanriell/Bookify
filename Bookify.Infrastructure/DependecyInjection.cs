using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Abstractions.Data;
using Bookify.Application.Abstractions.Email;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;
using Bookify.Domain.Users;
using Bookify.Infrastructure.Authentication;
using Bookify.Infrastructure.Authentication.Models;
using Bookify.Infrastructure.Clock;
using Bookify.Infrastructure.Data;
using Bookify.Infrastructure.Email;
using Bookify.Infrastructure.Repositories;
using Dapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using AuthenticationOptions = Bookify.Infrastructure.Authentication.AuthenticationOptions;
using AuthenticationService = Microsoft.AspNetCore.Authentication.AuthenticationService;

namespace Bookify.Infrastructure;

public static class DependencyInjection
{
	// Extension method on the IServiceCollection interface
	public static IServiceCollection AddInfrastructure(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		services.AddTransient<IDateTimeProvider, DateTimeProvider>();

		services.AddTransient<IEmailService, EmailService>();

		AddPersistence(services, configuration);

		// Configuration for authentication. We are calling addAuthentication method
		// and we can optionally set up the authentication scheme. We can access the authentication scheme
		// exposed on the JWTBearerDefaults class and the value of this constant is bearer.
		// We are also calling the AddJWTBearer method which we can use to set up the JWTBearer options. 
		// It has a lot of important properties which are used for validating access tokens.
		services
		   .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
		   .AddJwtBearer();

		services.Configure<AuthenticationOptions>(configuration.GetSection("Authentication"));

		services.ConfigureOptions<JwtBearerOptionsSetup>();

		services.Configure<KeycloakOptions>(configuration.GetSection("Keycloak"));

		services.AddTransient<AdminAuthorizationDelegatingHandler>();

		services.AddHttpClient<IAuthenticationService, AuthenticationService>((serviceProvider, httpClient) =>
			{
				// Resolving Keycloak options instance and set the base address
				// on our HTTP client so that we don't have to configure it on every request. 
				var keycloakOptions = serviceProvider.GetRequiredService<IOptions<KeycloakOptions>>().Value;

				httpClient.BaseAddress = new Uri(keycloakOptions.AdminUrl);
			})
		   .AddHttpMessageHandler<AdminAuthorizationDelegatingHandler>();

		return services;
	}

	private static void AddPersistence(IServiceCollection services, IConfiguration configuration)
	{
		// We are using the IConfiguration instance to get the value of the connection string
		// from our application settings. This is basically the connection string that
		// EFCore is going to use connect to our PostgreSQL database instance. 
		var connectionString =
			configuration.GetConnectionString("Database") ??
			throw new ArgumentNullException(nameof(configuration));

		// To register EFCore we need to call the AddDbContext method which is exposed by EntityFrameworkCore
		// We specify our database context as the generic argument, and then on the DatabaseContextOptionsBuilder we 
		// need to call the specific provider that we are using as our database, in our case it would be Postgre
		// so we have to call the UseNpgsql method and give it the connection string so that it can connect to our
		// database.
		// IMPORTANT! EFCore, by convention, is going to use a title case for the names of the tables and columns in our database model.
		// Postgre on the other hand prefers snake case. 
		services.AddDbContext<ApplicationDbContext>(options =>
		{
			// We are using EFCore.NamingConventions to set explicit snake case, and this is going to align 
			// our table and column names with the SnakeCaseNamingConvention which is what Postgre is using and also the SQL
			// that we added in the application project was referencing this naming convention for the columns and tables. 
			options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();
		});

		services.AddScoped<IUserRepository, UserRepository>();

		services.AddScoped<IApartmentRepository, ApartmentRepository>();

		services.AddScoped<IBookingRepository, BookingRepository>();

		// We use a service provider to resolve the database context and use it as a unit of work implementation. 
		services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());

		services.AddSingleton<ISqlConnectionFactory>(_ =>
			new SqlConnectionFactory(connectionString));

		SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
	}
}