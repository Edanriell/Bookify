using Bookify.Application.Abstractions.Behaviors;
using Bookify.Domain.Bookings;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Bookify.Application;

// Static class which is responsible for registering the services that are
// specific to the application layer.
public static class DependencyInjection
{
	// This public static method returns an instance of the IServiceCollection interface
	// which is used for configuring DI in .NET
	// Introducing application-specific services 
	public static IServiceCollection AddApplication(this IServiceCollection services)
	{
		// Registering MediatR services, calling AddMediatR method, which accepts an action
		// on the MediatorServiceConfiguration and which instance is used to configure
		// the required services for Mediator. Mainly, this is going to be wiring up the command
		// and command handlers and the query and query handlers
		services.AddMediatR(configuration =>
		{
			// To achieve this, we are going to say configuration, and we are going to 
			// call register services from assembly, and then we are going to give it an assembly instance.
			// The typeof(DependencyInjection).Assembly consequently is the application project.
			configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);

			// Registering logging behavior
			// Now, when we send our command, it's going to first enter the logging behavior, run the logging statement and then execute
			// the command handler before returning the response
			configuration.AddOpenBehavior(typeof(LoggingBehavior<,>));

			configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
		});

		// We need to register our validators with dependency injection.
		// All we need to do is to give the assembly where our validators
		// are defined. This is the application assembly. 
		//  This is going to scan the assembly and register any validators as an IValidator instance which we are using in our validation behaviour. 
		services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

		services.AddTransient<PricingService>();

		return services;
	}
}