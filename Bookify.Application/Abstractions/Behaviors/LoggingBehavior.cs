using Bookify.Domain.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Bookify.Application.Abstractions.Behaviors;

internal sealed class LoggingBehavior <TRequest, TResponse>
	: IPipelineBehavior<TRequest, TResponse>
	where TRequest : IBaseRequest
	where TResponse : Result
{
	private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

	public LoggingBehavior ( ILogger<LoggingBehavior<TRequest, TResponse>> logger ) { _logger = logger; }

	public async Task<TResponse> Handle ( TRequest request,
										  RequestHandlerDelegate<TResponse> next,
										  CancellationToken cancellationToken )
	{
		var requestName = request.GetType().
			Name;

		try
		{
			_logger.LogInformation (
					message : "Executing request {RequestName}",
					requestName
				);

			var result = await next();

			if ( result.IsSuccess )
				_logger.LogInformation (
						message : "Request {RequestName} processed successfully",
						requestName
					);
			else
			{
				using ( LogContext.PushProperty (
							   name : "Error",
							   value : result.Error,
							   destructureObjects : true
						   ) )
					_logger.LogError (
							message : "Request {RequestName} processed with error",
							requestName
						);
			}

			return result;
		}
		catch ( Exception exception )
		{
			_logger.LogError (
					exception : exception,
					message : "Request {RequestName} processing failed",
					requestName
				);

			throw;
		}
	}
}
