using System.Reflection;
using Bookify.Application.Abstractions.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Bookify.Application;

public static class DependencyInjection
{
	public static IServiceCollection AddApplication(
		this IServiceCollection services,
		Assembly[] moduleAssemblies)
	{
		services.AddMediatR(configuration =>
		{
			configuration.RegisterServicesFromAssemblies(moduleAssemblies);

			configuration.AddOpenBehavior(typeof(LoggingBehavior<,>));

			configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));

			configuration.AddOpenBehavior(typeof(QueryCachingBehavior<,>));
		});

		services.AddValidatorsFromAssemblies(moduleAssemblies, includeInternalTypes: true);

		return services;
	}
}
