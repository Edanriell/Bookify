using Bookify.Application.Abstractions.Messaging;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bookify.Application.Abstractions.Behaviors;

// Pipeline behavior for Mediator. 
// where TRequest: IBaseCommand is a generic constraint that the TRequest generic argument has to be a base command
// We are doing this because, we want to be running logging for our command pipeline. We don't really care about
// logging our queris, we want our queries to be as fast as possible and return the response to the user straight away,
// while for commands there is a lot of value in adding some additional information through logging. 
public class LoggingBehavior<TRequest, TResponse>
	: IPipelineBehavior<TRequest, TResponse>
	where TRequest : IBaseCommand
{
	private readonly ILogger<TRequest> _logger;

	public LoggingBehavior(ILogger<TRequest> logger)
	{
		_logger = logger;
	}

	public async Task<TResponse> Handle(
		TRequest request,
		RequestHandlerDelegate<TResponse> next,
		CancellationToken cancellationToken)
	{
		// We are using reflection to get the type of the current request, and we are taking the
		// name of this type, which is going to be our command name. 
		var name = request.GetType().Name;

		// We wrapp all our logic in a try and catch statement
		try
		{
			// Loggin an infromation statement saying that we are executing the command with a particular name
			_logger.LogInformation("Executing command {Command}", name);

			// After that we run our request handler delegate, which is our command handler,
			// we obtain a result instance, and we add another information log
			var result = await next();

			// That this command was processed successfully
			_logger.LogInformation("Command {Command} processed successfully", name);

			// And in the end, we just return this result from our pipeline, 
			return result;
		}
		catch (Exception exception)
		{
			// And if exception occurs we are going to log an error passing in the
			// exception and saying that this command processing failed.
			// We can enrich these logs further with more contextual information like who is the
			// current user, what is the ID of the command, if we assign IDs to commands, what is correlation ID
			// or the request ID and so on. 
			_logger.LogError(exception, "Command {Command} processing failed", name);

			throw;
		}
	}
}