using System.Net.Http.Json;
using Bookify.Api.FunctionalTests.Users;
using Bookify.Modules.Users.Infrastructure.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.Keycloak;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace Bookify.Api.FunctionalTests.Infrastructure;

public class FunctionalTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
	private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
		.WithImage("postgres:latest")
		.WithDatabase("bookify")
		.WithUsername("postgres")
		.WithPassword("postgres")
		.Build();

	private readonly KeycloakContainer _keycloakContainer = new KeycloakBuilder()
		.WithImage("quay.io/keycloak/keycloak:latest")
		.WithResourceMapping(
			new FileInfo(".files/bookify-realm-export.json"),
			new FileInfo("/opt/keycloak/data/import/realm.json"))
		.WithCommand("--import-realm")
		.Build();

	private readonly RedisContainer _redisContainer = new RedisBuilder()
		.WithImage("redis:latest")
		.Build();

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

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		Environment.SetEnvironmentVariable("ConnectionStrings:Database", _dbContainer.GetConnectionString());
		Environment.SetEnvironmentVariable("ConnectionStrings:Cache", _redisContainer.GetConnectionString());

		builder.ConfigureTestServices(services =>
		{
			string? keycloakAddress = _keycloakContainer.GetBaseAddress();

			services.Configure<KeycloakOptions>(o =>
			{
				o.AdminUrl = $"{keycloakAddress}admin/realms/bookify/";
				o.TokenUrl = $"{keycloakAddress}realms/bookify/protocol/openid-connect/token";
			});

			services.Configure<AuthenticationOptions>(o =>
			{
				o.Issuer = $"{keycloakAddress}realms/bookify/";
				o.MetadataUrl = $"{keycloakAddress}realms/bookify/.well-known/openid-configuration";
			});
		});
	}

	private async Task InitializeTestUserAsync()
	{
		try
		{
			using HttpClient httpClient = CreateClient();

			await httpClient.PostAsJsonAsync("api/v1/users/register", UserData.RegisterTestUserRequest);
		}
		catch
		{
			// Do nothing.
		}
	}
}
