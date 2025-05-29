using Bookify.Api.Extensions;
using Bookify.Api.OpenApi;
using Bookify.Application;
using Bookify.Infrastructure;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder (
		args : args
	);

builder.Host.UseSerilog (
		configureLogger : ( context, loggerConfig ) =>
			loggerConfig.ReadFrom.Configuration (
					configuration : context.Configuration
				)
	);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddInfrastructure (
		configuration : builder.Configuration
	);

builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

var app = builder.Build();

if ( app.Environment.IsDevelopment() )
{
	app.UseSwagger();
	app.UseSwaggerUI (
			setupAction : options =>
			{
				foreach ( var description in app.DescribeApiVersions() )
				{
					var url = $"/swagger/{description.GroupName}/swagger.json";
					var name = description.GroupName.ToUpperInvariant();
					options.SwaggerEndpoint (
							url : url,
							name : name
						);
				}
			}
		);

	app.ApplyMigrations();

	// REMARK: Uncomment if you want to seed initial data.
	app.SeedData();
}

app.UseHttpsRedirection();

app.UseRequestContextLogging();

app.UseSerilogRequestLogging();

app.UseCustomExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks (
		pattern : "health",
		options : new HealthCheckOptions
				  {
					  ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
				  }
	);

app.Run();

public partial class Program;

// IMPORTANT
// From Bookify root
// To create a new migration, run the following command:
// dotnet ef migrations add Create_Database --project .\Bookify.Infrastructure\Bookify.Infrastructure.csproj
// --startup-project .\Bookify.Api\Bookify.Api.csproj
