using Asp.Versioning.ApiExplorer;
using Bookify.Api.Extensions;
using Bookify.Api.OpenApi;
using Bookify.Application;
using Bookify.Infrastructure;
using Bookify.Modules.Bookings.Infrastructure;
using Bookify.Modules.Users.Application;
using Bookify.Modules.Users.Infrastructure;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) =>
	loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication(
[
	AssemblyReference.Assembly,
	Bookify.Modules.Bookings.Application.AssemblyReference.Assembly
]);
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddUsersModule(builder.Configuration);

builder.Services.AddBookingsModule();

builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(options =>
	{
		foreach (ApiVersionDescription description in app.DescribeApiVersions())
		{
			string url = $"/swagger/{description.GroupName}/swagger.json";
			string name = description.GroupName.ToUpperInvariant();
			options.SwaggerEndpoint(url, name);
		}
	});

	app.ApplyMigrations();

	// REMARK: Uncomment if you want to seed initial data.
	// app.SeedData();
}

app.UseHttpsRedirection();

app.UseRequestContextLogging();

app.UseSerilogRequestLogging();

app.UseCustomExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("health", new HealthCheckOptions
{
	ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

await app.RunAsync();

public partial class Program;
