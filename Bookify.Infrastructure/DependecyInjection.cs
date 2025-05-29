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
	public static IServiceCollection AddInfrastructure ( this IServiceCollection services,
														 IConfiguration configuration )
	{
		services.AddTransient<IDateTimeProvider, DateTimeProvider>();

		services.AddTransient<IEmailService, EmailService>();

		AddPersistence (
				services : services,
				configuration : configuration
			);

		AddCaching (
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

		AddHealthChecks (
				services : services,
				configuration : configuration
			);

		AddApiVersioning (
				services : services
			);

		AddBackgroundJobs (
				services : services,
				configuration : configuration
			);

		return services;
	}

	private static void AddPersistence ( IServiceCollection services, IConfiguration configuration )
	{
		var connectionString = configuration.GetConnectionString (
									   name : "Database"
								   )
							?? throw new ArgumentNullException (
									   paramName : nameof(configuration)
								   );

		services.AddDbContext<ApplicationDbContext> (
				optionsAction : options =>
					options.UseNpgsql (
								connectionString : connectionString
							).
						UseSnakeCaseNamingConvention()
			);

		services.AddScoped<IUserRepository, UserRepository>();

		services.AddScoped<IApartmentRepository, ApartmentRepository>();

		services.AddScoped<IBookingRepository, BookingRepository>();

		services.AddScoped<IReviewRepository, ReviewRepository>();

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
						var keycloakOptions = serviceProvider.GetRequiredService<IOptions<KeycloakOptions>>().
							Value;

						httpClient.BaseAddress = new Uri (
								uriString : keycloakOptions.AdminUrl
							);
					}
				).
			AddHttpMessageHandler<AdminAuthorizationDelegatingHandler>();

		services.AddHttpClient<IJwtService, JwtService> (
				configureClient : ( serviceProvider, httpClient ) =>
				{
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

		services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();

		services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
	}

	private static void AddCaching ( IServiceCollection services, IConfiguration configuration )
	{
		var connectionString = configuration.GetConnectionString (
									   name : "Cache"
								   )
							?? throw new ArgumentNullException (
									   paramName : nameof(configuration)
								   );

		services.AddStackExchangeRedisCache (
				setupAction : options => options.Configuration = connectionString
			);

		services.AddSingleton<ICacheService, CacheService>();
	}

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

	private static void AddApiVersioning ( IServiceCollection services )
	{
		services.AddApiVersioning (
					setupAction : options =>
					{
						options.DefaultApiVersion = new ApiVersion (
								majorVersion : 1
							);
						options.ReportApiVersions = true;
						options.ApiVersionReader = new UrlSegmentApiVersionReader();
					}
				).
			AddMvc().
			AddApiExplorer (
					setupAction : options =>
					{
						options.GroupNameFormat = "'v'V";
						options.SubstituteApiVersionInUrl = true;
					}
				);
	}

	private static void AddBackgroundJobs ( IServiceCollection services, IConfiguration configuration )
	{
		services.Configure<OutboxOptions> (
				config : configuration.GetSection (
						key : "Outbox"
					)
			);

		services.AddQuartz();

		services.AddQuartzHostedService (
				configure : options => options.WaitForJobsToComplete = true
			);

		services.ConfigureOptions<ProcessOutboxMessagesJobSetup>();
	}
}
