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


// Accessing our web application builder, and we are going to 
// access the host property so that we can configure our host builder. 
// After we have an extension method available that is called UseSerilog, and we are going to
// use an overload of this method that gives us access to the host builder context and
// the logger configuration, and what we are going to do is to configure our Serilog configuration
// from our application settings. We are using host builder context to provide
// the IConfiguration instance so that Serilog can be configured from the application settings.
builder.Host.UseSerilog (
		// Allows us to configure Serilog from appsettings.json
		configureLogger : ( context, configuration ) => configuration.ReadFrom.Configuration (
				configuration : context.Configuration
			)
	);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
// This method expects an IConfiguration instance, which we
// can pass from builder configuration. 
builder.Services.AddInfrastructure (
		configuration : builder.Configuration
	);

// Health checks are an automated way for us to know what is the health of our system.
// This can include our API itself and also any external components that we might be using. 
// For example, we are using PostgreSQL database, a Redis cache and Keycloak as an external identity provider.
// builder.Services.AddHealthChecks().
//	AddCheck<CustomSqlHealthCheck> (
//			name : "custom-sql"
//		);

builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

var app = builder.Build();

if ( app.Environment.IsDevelopment() )
{
	app.UseSwagger();
	app.UseSwaggerUI (
			setupAction : options =>
			{
				// Passing a delegate for the Swagger UI options so that we call
				// some methods in this class, and we are using
				// the webapplication here to call the DescribeApiVersions method, which is going to 
				// return a collection of API version descriptions. 
				var descriptions = app.DescribeApiVersions();

				// Then we iterate over this collection and create the URL for our API version
				// and the API version name, and we are going to call the Swagger endpoint method
				// to configure the path to the respective Swagger JSON document.
				foreach ( var description in descriptions )
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
	// app.SeedData();
}

app.UseHttpsRedirection();

app.UseRequestContextLogging();

// Introduce middleware that is going to hook into our incoming requests
// and start logging useful information about the processing of our API requests,
// such as the status codes, request times, any exceptions, and so on. 
app.UseSerilogRequestLogging();

app.UseCustomExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseHealthChecks (
		path : "/health",
		options : new HealthCheckOptions
				  {
					  ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
				  }
	);

app.Run();

// IMPORTANT
// From Bookify root
// To create a new migration, run the following command:
// dotnet ef migrations add Create_Database --project .\Bookify.Infrastructure\Bookify.Infrastructure.csproj
// --startup-project .\Bookify.Api\Bookify.Api.csproj

// Custom health check
// public class CustomSqlHealthCheck ( ISqlConnectionFactory sqlConnectionFactory ) : IHealthCheck
// {
//	public async Task<HealthCheckResult> CheckHealthAsync ( HealthCheckContext context,
//															CancellationToken cancellationToken
//																= default(CancellationToken) )
//	{
// // Database can fail so we are using try catch 
//		try
//		{
// // We are using using statement to properly dispose of any resources. 
//			using var connection = sqlConnectionFactory.CreateConnection();
// 
//			await connection.ExecuteScalarAsync (
//					sql : "SELECT 1;"
//				);
// 
//			return HealthCheckResult.Healthy();
//		}
//		catch ( Exception e )
//		{
//			return HealthCheckResult.Unhealthy (
//					exception : e
//				);
//		}
//	}
// }
