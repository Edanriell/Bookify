using Bookify.Api.Extensions;
using Bookify.Application;
using Bookify.Infrastructure;
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

var app = builder.Build();

if ( app.Environment.IsDevelopment() )
{
	app.UseSwagger();
	app.UseSwaggerUI();

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

app.Run();

// IMPORTANT
// From Bookify root
// To create a new migration, run the following command:
// dotnet ef migrations add Create_Database --project .\Bookify.Infrastructure\Bookify.Infrastructure.csproj
// --startup-project .\Bookify.Api\Bookify.Api.csproj
