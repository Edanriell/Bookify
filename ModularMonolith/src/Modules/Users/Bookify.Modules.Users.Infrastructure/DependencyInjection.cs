using Bookify.Application.Abstractions.Authentication;
using Bookify.Modules.Users.Application.Abstractions.Authentication;
using Bookify.Modules.Users.Domain.Users;
using Bookify.Modules.Users.Infrastructure.Authentication;
using Bookify.Modules.Users.Infrastructure.Authorization;
using Bookify.Modules.Users.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using AuthenticationOptions = Bookify.Modules.Users.Infrastructure.Authentication.AuthenticationOptions;
using AuthenticationService = Bookify.Modules.Users.Infrastructure.Authentication.AuthenticationService;
using IAuthenticationService = Bookify.Modules.Users.Application.Abstractions.Authentication.IAuthenticationService;

namespace Bookify.Modules.Users.Infrastructure;

public static class DependencyInjection
{
	public static IServiceCollection AddUsersModule(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		AddPersistence(services);

		AddAuthentication(services, configuration);

		AddAuthorization(services);

		return services;
	}

	private static void AddPersistence(IServiceCollection services)
	{
		services.AddScoped<IUserRepository, UserRepository>();
	}

	private static void AddAuthentication(IServiceCollection services, IConfiguration configuration)
	{
		services
			.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer();

		services.Configure<AuthenticationOptions>(configuration.GetSection("Authentication"));

		services.ConfigureOptions<JwtBearerOptionsSetup>();

		services.Configure<KeycloakOptions>(configuration.GetSection("Keycloak"));

		services.AddTransient<AdminAuthorizationDelegatingHandler>();

		services.AddHttpClient<IAuthenticationService, AuthenticationService>((serviceProvider, httpClient) =>
			{
				KeycloakOptions keycloakOptions = serviceProvider.GetRequiredService<IOptions<KeycloakOptions>>().Value;

				httpClient.BaseAddress = new Uri(keycloakOptions.AdminUrl);
			})
			.AddHttpMessageHandler<AdminAuthorizationDelegatingHandler>();

		services.AddHttpClient<IJwtService, JwtService>((serviceProvider, httpClient) =>
		{
			KeycloakOptions keycloakOptions = serviceProvider.GetRequiredService<IOptions<KeycloakOptions>>().Value;

			httpClient.BaseAddress = new Uri(keycloakOptions.TokenUrl);
		});

		services.AddHttpContextAccessor();

		services.AddScoped<IUserContext, UserContext>();
	}

	private static void AddAuthorization(IServiceCollection services)
	{
		services.AddScoped<AuthorizationService>();

		services.AddTransient<IClaimsTransformation, CustomClaimsTransformation>();

		services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();

		services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
	}
}
