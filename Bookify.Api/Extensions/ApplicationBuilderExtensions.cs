using Bookify.Api.Middleware;
using Bookify.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Api.Extensions;

public static class ApplicationBuilderExtensions
{
	public static void ApplyMigrations ( this IApplicationBuilder app )
	{
		// This method is used only for local development purposes to take the application builder
		// and create a scope, use this scope to resolve my database context and then apply
		// any pending migrations to our database.
		using var scope = app.ApplicationServices.CreateScope();

		using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

		dbContext.Database.Migrate();
	}

	public static void UseCustomExceptionHandler ( this IApplicationBuilder app )
	{
		app.UseMiddleware<ExceptionHandlingMiddleware>();
	}

	public static IApplicationBuilder UseRequestContextLogging ( this IApplicationBuilder app )
	{
		app.UseMiddleware<RequestContextLoggingMiddleware>();

		return app;
	}
}
