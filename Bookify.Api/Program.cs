using Bookify.Api.Extensions;
using Bookify.Application;
using Bookify.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
// This method expects an IConfiguration instance, which we
// can pass from builder configuration. 
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();

	app.ApplyMigrations();

	// REMARK: Uncomment if you want to seed initial data.
	// app.SeedData();
}

app.UseHttpsRedirection();

app.UseCustomExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

// IMPORTANT
// From Bookify root
// To create a new migration, run the following command:
// dotnet ef migrations add Create_Database --project .\Bookify.Infrastructure\Bookify.Infrastructure.csproj --startup-project .\Bookify.Api\Bookify.Api.csproj