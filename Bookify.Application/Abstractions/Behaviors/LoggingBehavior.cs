using Bookify.Domain.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Bookify.Application.Abstractions.Behaviors;

// Pipeline behavior for Mediator. 
// where TRequest: IBaseCommand is a generic constraint that the TRequest generic argument has to be a base command
// We are doing this because, we want to be running logging for our command pipeline. We don't really care about
// logging our queris, we want our queries to be as fast as possible and return the response to the user straight away,
// while for commands there is a lot of value in adding some additional information through logging. 
public class LoggingBehavior <TRequest, TResponse>
	: IPipelineBehavior<TRequest, TResponse>
//	where TRequest : IBaseCommand
	// If we want to update it to run for all of 
// our mediator requests, which are commands and queries, we can replace this generic constraint
// to target the base request because our command and query abstractions implement
// this interface under the hood. 
	where TRequest : IBaseRequest
	//  Also we are adding one  more generic constraint that the response type  of our
	// requests is always a result  object and we  made sure of this when we were defining our 
	// command and query abstractions. 
	where TResponse : Result
{
//	private readonly ILogger<TRequest> _logger;

	private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

//	public LoggingBehavior(ILogger<TRequest> logger)
//	{
//		_logger = logger;
//	}

	public LoggingBehavior ( ILogger<LoggingBehavior<TRequest, TResponse>> logger ) { _logger = logger; }

	public async Task<TResponse> Handle ( TRequest request,
										  RequestHandlerDelegate<TResponse> next,
										  CancellationToken cancellationToken )
	{
		// We are using reflection to get the type of the current request, and we are taking the
		// name of this type, which is going to be our command name. 
		var name = request.GetType().
			Name;

		// We wrapp all our logic in a try and catch statement
		// Logging > The reason of doing thing up (adding generic constraints) is to improve
		// the logging in our handle method. 
		try
		{
			// Loggin an infromation statement saying that we are executing the command with a particular name
//			_logger.LogInformation("Executing command {Command}", name);
			_logger.LogInformation (
					message : "Executing request {Request}",
					name
				);

			// After that we run our request handler delegate, which is our command handler,
			// we obtain a result instance, and we add another information log
			var result = await next();

			if ( result.IsSuccess )
				_logger.LogInformation (
						message : "Request {Request} processed successfully",
						name
					);
			else
			{
//				_logger.LogError ("Request {Request}  processed with {@Error}", name, result.Error  );
				using ( LogContext.PushProperty (
							   name : "Error",
							   value : result.Error,
							   destructureObjects : true
						   ) )
					_logger.LogError (
							message : "Request {Request} processed with error",
							name
						);
			}

			// That this command was processed successfully
//			_logger.LogInformation("Command {Command} processed successfully", name);
//			_logger.LogInformation (
//					message : "Request {Request} processed successfully",
//					name
//				);

			// And in the end, we just return this result from our pipeline, 
			return result;
		}
		catch ( Exception exception )
		{
			// And if exception occurs we are going to log an error passing in the
			// exception and saying that this command processing failed.
			// We can enrich these logs further with more contextual information like who is the
			// current user, what is the ID of the command, if we assign IDs to commands, what is correlation ID
			// or the request ID and so on. 
//			_logger.LogError(exception, "Command {Command} processing failed", name);
			_logger.LogError (
					exception : exception,
					message : "Request {Request} processing failed",
					name
				);

			throw;
		}
	}
}
