using System.Net.Http.Json;
using Bookify.Api.FunctionalTests.Users;
using Bookify.Application.Abstractions.Data;
using Bookify.Infrastructure;
using Bookify.Infrastructure.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.Keycloak;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace Bookify.Api.FunctionalTests.Infrastructure;

public class FunctionalTestWebAppFactory : WebApplicationFactory<Program>,
										   IAsyncLifetime
{
	private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder().WithImage (
				image : "postgres:latest"
			).
		WithDatabase (
				database : "bookify"
			).
		WithUsername (
				username : "postgres"
			).
		WithPassword (
				password : "postgres"
			).
		Build();

	private readonly KeycloakContainer _keycloakContainer = new KeycloakBuilder().WithResourceMapping (
				source : new FileInfo (
						fileName : ".files/bookify-realm-export.json"
					),
				target : new FileInfo (
						fileName : "/opt/keycloak/data/import/realm.json"
					)
			).
		WithCommand (
				"--import-realm"
			).
		Build();

	private readonly RedisContainer _redisContainer = new RedisBuilder().WithImage (
				image : "redis:latest"
			).
		Build();

	public async Task InitializeAsync()
	{
		await _dbContainer.StartAsync();
		await _redisContainer.StartAsync();
		await _keycloakContainer.StartAsync();

		await InitializeTestUserAsync();
	}

	public new async Task DisposeAsync()
	{
		await _dbContainer.StopAsync();
		await _redisContainer.StopAsync();
		await _keycloakContainer.StopAsync();
	}

	protected override void ConfigureWebHost ( IWebHostBuilder builder )
	{
		builder.ConfigureTestServices (
				servicesConfiguration : services =>
				{
					services.RemoveAll (
							serviceType : typeof(DbContextOptions<ApplicationDbContext>)
						);

					services.AddDbContext<ApplicationDbContext> (
							optionsAction : options =>
								options.UseNpgsql (
											connectionString : _dbContainer.GetConnectionString()
										).
									UseSnakeCaseNamingConvention()
						);

					services.RemoveAll (
							serviceType : typeof(ISqlConnectionFactory)
						);

					services.AddSingleton<ISqlConnectionFactory> (
							implementationFactory : _ =>
								new SqlConnectionFactory (
										connectionString : _dbContainer.GetConnectionString()
									)
						);

					services.Configure<RedisCacheOptions> (
							configureOptions : redisCacheOptions =>
								redisCacheOptions.Configuration = _redisContainer.GetConnectionString()
						);

					var keycloakAddress = _keycloakContainer.GetBaseAddress();

					services.Configure<KeycloakOptions> (
							configureOptions : o =>
							{
								o.AdminUrl = $"{keycloakAddress}admin/realms/bookify/";
								o.TokenUrl = $"{keycloakAddress}realms/bookify/protocol/openid-connect/token";
							}
						);

					services.Configure<AuthenticationOptions> (
							configureOptions : o =>
							{
								o.Issuer = $"{keycloakAddress}realms/bookify/";
								o.MetadataUrl = $"{keycloakAddress}realms/bookify/.well-known/openid-configuration";
							}
						);
				}
			);
	}

	private async Task InitializeTestUserAsync()
	{
		var httpClient = CreateClient();

		await httpClient.PostAsJsonAsync (
				requestUri : "api/v1/users/register",
				value : UserData.RegisterTestUserRequest
			);
	}
}
