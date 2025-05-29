using Bookify.Application.Abstractions.Behaviors;
using Bookify.Domain.Bookings;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Bookify.Application;

public static class DependencyInjection
{
	public static IServiceCollection AddApplication ( this IServiceCollection services )
	{
		services.AddMediatR (
				configuration : configuration =>
				{
					configuration.RegisterServicesFromAssembly (
							assembly : typeof(DependencyInjection).Assembly
						);

					configuration.AddOpenBehavior (
							openBehaviorType : typeof(LoggingBehavior<,>)
						);

					configuration.AddOpenBehavior (
							openBehaviorType : typeof(ValidationBehavior<,>)
						);

					configuration.AddOpenBehavior (
							openBehaviorType : typeof(QueryCachingBehavior<,>)
						);
				}
			);

		services.AddValidatorsFromAssembly (
				assembly : typeof(DependencyInjection).Assembly,
				includeInternalTypes : true
			);

		services.AddTransient<PricingService>();

		return services;
	}
}
