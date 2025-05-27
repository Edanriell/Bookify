using Asp.Versioning;
using Bookify.Application.Abstractions.Authentication;
using Bookify.Application.Abstractions.Caching;
using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Abstractions.Data;
using Bookify.Application.Abstractions.Email;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;
using Bookify.Domain.Reviews;
using Bookify.Domain.Users;
using Bookify.Infrastructure.Authentication;
using Bookify.Infrastructure.Authorization;
using Bookify.Infrastructure.Caching;
using Bookify.Infrastructure.Clock;
using Bookify.Infrastructure.Data;
using Bookify.Infrastructure.Email;
using Bookify.Infrastructure.Outbox;
using Bookify.Infrastructure.Repositories;
using Dapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Quartz;
using AuthenticationOptions = Bookify.Infrastructure.Authentication.AuthenticationOptions;
using AuthenticationService = Bookify.Infrastructure.Authentication.AuthenticationService;
using IAuthenticationService = Bookify.Application.Abstractions.Authentication.IAuthenticationService;

namespace Bookify.Infrastructure;

public static class DependencyInjection
{
	// Extension method on the IServiceCollection interface
	public static IServiceCollection AddInfrastructure ( this IServiceCollection services,
														 IConfiguration configuration )
	{
		services.AddTransient<IDateTimeProvider, DateTimeProvider>();

		services.AddTransient<IEmailService, EmailService>();

		AddPersistence (
				services : services,
				configuration : configuration
			);

		AddAuthentication (
				services : services,
				configuration : configuration
			);

		AddAuthorization (
				services : services
			);

		// We do this to be able to configure caching at runtime
		AddCaching (
				services : services,
				configuration : configuration
			);

		// Registering all health checks with dependency injection.
		AddHealthChecks (
				services : services,
				configuration : configuration
			);

		AddApiVersioning (
				services : services
			);

		// Transactional Outbox Pattern
		AddBackgroundJobs (
				services : services,
				configuration : configuration
			);

		return services;
	}

	private static void AddPersistence ( IServiceCollection services, IConfiguration configuration )
	{
		// We are using the IConfiguration instance to get the value of the connection string
		// from our application settings. This is basically the connection string that
		// EFCore is going to use connect to our PostgreSQL database instance. 
		var connectionString =
			configuration.GetConnectionString (
					name : "Database"
				)
		 ?? throw new ArgumentNullException (
					paramName : nameof(configuration)
				);

		// To register EFCore we need to call the AddDbContext method which is exposed by EntityFrameworkCore
		// We specify our database context as the generic argument, and then on the DatabaseContextOptionsBuilder we 
		// need to call the specific provider that we are using as our database, in our case it would be Postgre
		// so we have to call the UseNpgsql method and give it the connection string so that it can connect to our
		// database.
		// IMPORTANT! EFCore, by convention, is going to use a title case for the names of the tables and columns in our
		// database model.
		// Postgre on the other hand prefers snake case. 
		services.AddDbContext<ApplicationDbContext> (
				optionsAction : options =>
				{
					// We are using EFCore.NamingConventions to set explicit snake case, and this is going to align 
					// our table and column names with the SnakeCaseNamingConvention which is what Postgre is using and
					// also the SQL
					// that we added in the application project was referencing this naming convention for the columns
					// and tables. 
					options.UseNpgsql (
								connectionString : connectionString
							).
						UseSnakeCaseNamingConvention();
				}
			);

		services.AddScoped<IUserRepository, UserRepository>();

		services.AddScoped<IApartmentRepository, ApartmentRepository>();

		services.AddScoped<IBookingRepository, BookingRepository>();

		services.AddScoped<IReviewRepository, ReviewRepository>();

		// We use a service provider to resolve the database context and use it as a unit of work implementation. 
		services.AddScoped<IUnitOfWork> (
				implementationFactory : sp => sp.GetRequiredService<ApplicationDbContext>()
			);

		services.AddSingleton<ISqlConnectionFactory> (
				implementationFactory : _ =>
					new SqlConnectionFactory (
							connectionString : connectionString
						)
			);

		SqlMapper.AddTypeHandler (
				handler : new DateOnlyTypeHandler()
			);
	}

	private static void AddAuthentication ( IServiceCollection services, IConfiguration configuration )
	{
		// Configuration for authentication. We are calling addAuthentication method
		// and we can optionally set up the authentication scheme. We can access the authentication scheme
		// exposed on the JWTBearerDefaults class and the value of this constant is bearer.
		// We are also calling the AddJWTBearer method which we can use to set up the JWTBearer options. 
		// It has a lot of important properties which are used for validating access tokens.
		services.AddAuthentication (
					defaultScheme : JwtBearerDefaults.AuthenticationScheme
				).
			AddJwtBearer();

		services.Configure<AuthenticationOptions> (
				config : configuration.GetSection (
						key : "Authentication"
					)
			);

		services.ConfigureOptions<JwtBearerOptionsSetup>();

		services.Configure<KeycloakOptions> (
				config : configuration.GetSection (
						key : "Keycloak"
					)
			);

		services.AddTransient<AdminAuthorizationDelegatingHandler>();

		services.AddHttpClient<IAuthenticationService, AuthenticationService> (
					configureClient : ( serviceProvider, httpClient ) =>
					{
						// Resolving Keycloak options instance and set the base address
						// on our HTTP client so that we don't have to configure it on every request. 
						var keycloakOptions = serviceProvider.GetRequiredService<IOptions<KeycloakOptions>>().
							Value;

						httpClient.BaseAddress = new Uri (
								uriString : keycloakOptions.AdminUrl
							);
					}
				).
			AddHttpMessageHandler<AdminAuthorizationDelegatingHandler>();

		// We are registering the JWT service as an HTTP client. And this is a typed
		// HTTP client implementation, which means that we can inject an HTTP client instance inside of
		// the JWT service class. 
		services.AddHttpClient<IJwtService, JwtService> (
				configureClient : ( serviceProvider, httpClient ) =>
				{
					// Resolving Keycloak options instance, so that we can configure the token URL for the
					// base address of this HTTP client. 
					var keycloakOptions = serviceProvider.GetRequiredService<IOptions<KeycloakOptions>>().
						Value;

					httpClient.BaseAddress = new Uri (
							uriString : keycloakOptions.TokenUrl
						);
				}
			);

		services.AddHttpContextAccessor();

		services.AddScoped<IUserContext, UserContext>();
	}

	private static void AddAuthorization ( IServiceCollection services )
	{
		services.AddScoped<AuthorizationService>();

		services.AddTransient<IClaimsTransformation, CustomClaimsTransformation>();

		// Permission-based Authorization
		services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();

		services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
	}

	private static void AddCaching ( IServiceCollection services, IConfiguration configuration )
	{
		// Fetching a connection string using the IConfiguration instance
		// And we are looking ofr a cconnection string named cache. 
		var connectionString = configuration.GetConnectionString (
									   name : "Cache"
								   )
							?? throw new ArgumentNullException (
									   paramName : nameof(configuration)
								   );

		// Setting the configuration property on the Redis cache options, which is just a way
		// for us to pass in the connection string. 
		services.AddStackExchangeRedisCache (
				setupAction : options => options.Configuration = connectionString
			);

		// Configuring cache service as a singleton implementation of the ICacheService interface. 
		services.AddSingleton<ICacheService, CacheService>();
	}

	// HealthChecks are useful because we can set up monitoring support on our healthcheck endpoint.
	// Most cloud providers allow us to configure an endpoint that is going to be 
	// monitored every few seconds or every few minutes, and as long as this endpoint 
	// returns a 200 OK response, nothing bad is going to happen. But if we get a 503 service
	// unavailable, meaning that our API isn't able to function as expected, then we can configure
	// what is going to happen. For example, we can figure out that our database isn't running, and
	// we can restart it. We can determine that our API is completely unavailable, and it is time to implement
	// a failover scenario where we can configure the cloud provider to spin up a new application instance and 
	// terminate the old one. And we can also plug in some alerting where we can start getting email or
	// Slack notifications when a health check fails. 
	private static void AddHealthChecks ( IServiceCollection services, IConfiguration configuration )
	{
		services.AddHealthChecks().
			AddNpgSql (
					connectionString : configuration.GetConnectionString (
							name : "Database"
						)!
				).
			AddRedis (
					redisConnectionString : configuration.GetConnectionString (
							name : "Cache"
						)!
				).
			AddUrlGroup (
					uri : new Uri (
							uriString : configuration[key : "KeyCloak:BaseUrl"]!
						),
					httpMethod : HttpMethod.Get,
					name : "keycloak"
				);
	}

	// Api versioning is the process of managing and tracking changes to our api. This also involves
	// communicating changes about our API to our API consumers, and if there is one constant with API development
	// is that we can't avoid changes to our API.
	// There is 3 types of api versioning. Query parameter versioning, where we specify the API version as query
	// parameter
	// in our URL. Header API versioning, where we pass the API version using a custom request header. URL versioning
	// where the API version is part of the actual route. URL versioning is also the most popular approach to API
	// versioning.
	// We should version API as soon as we make a breaking change. What constitutes a breaking change should
	// be an agreement that we make as a team, for example. Any changes to the behaviour of our API constitute a
	// breaking change.
	// Adding or removing API endpoints is a breaking change. Changing the request parameters without default values or
	// adding
	// or removing fields to the API response could also be a breaking change. Some of these API changes will break our
	// API
	// consumers, and they are definitely dangerous. 
	private static void AddApiVersioning ( IServiceCollection services )
	{
		services.AddApiVersioning (
					setupAction : options =>
					{
						// Setting the default API version to 1. This is a sensible default value if we
						// are just introducing API versioning to our solution.
						options.DefaultApiVersion = new ApiVersion (
								majorVersion : 1
							);
						// Attaching an HTTP header called API supported versions
						// that is going to return the API version supported by the current 
						// endpoint.It is also going to add the API deprecated versions response header
						// reporting the deprecated API versions.
						options.ReportApiVersions = true;
						// This actually determines what kind of API versioning we want to be using. We pass in
						// UrlSegmentApiVersionReader because we want to implement URL versioning.
						// 
						options.ApiVersionReader = new UrlSegmentApiVersionReader();
						// Header API versioning is also supported. We are going to use the HeaderApiVersionReader
						// Passing in as argument the name of the header that we are going to use.
//						options.ApiVersionReader = new HeaderApiVersionReader("X-Version-Id");
						// Query version is also supported. We are going to use the QueryStringApiVersionReader
//						options.ApiVersionReader = new QueryStringApiVersionReader();
						// Api version reader also supports multiple types of API versioning
//						options.ApiVersionReader = ApiVersionReader.Combine (
//								apiVersionReader : new HeaderApiVersionReader(),
//								new UrlSegmentApiVersionReader()
//							);
					}
				).
			AddMvc().
			// Allows us to configure api explorer options
			AddApiExplorer (
					setupAction : options =>
					{
						// Lowercacse v matches in route v 
						// and uppercase V is {version:apiVersion}
						options.GroupNameFormat = "'v'V";
						// In swagger, we will se a version of endpoint as {version:apiVersion}
						options.SubstituteApiVersionInUrl = true;
					}
				);
	}

	// Transactional Outbox Pattern
	private static void AddBackgroundJobs ( IServiceCollection services, IConfiguration configuration )
	{
		// Mapping an outbox section to class OutboxOptions.
		services.Configure<OutboxOptions> (
				config : configuration.GetSection (
						key : "Outbox"
					)
			);

		services.AddQuartz();

		// Waiting for jobs to complete when the application is shutting down. 
		services.AddQuartzHostedService (
				configure : options => options.WaitForJobsToComplete = true
			);

		services.ConfigureOptions<ProcessOutboxMessagesJobSetup>();
	}
}
