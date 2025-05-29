using Bookify.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Api.Middleware;

internal sealed class ExceptionHandlingMiddleware
{
	private readonly ILogger<ExceptionHandlingMiddleware> _logger;
	private readonly RequestDelegate _next;

	public ExceptionHandlingMiddleware ( RequestDelegate next,
										 ILogger<ExceptionHandlingMiddleware> logger )
	{
		_next = next;
		_logger = logger;
	}

	public async Task InvokeAsync ( HttpContext context )
	{
		try
		{
			await _next (
					context : context
				);
		}
		catch ( Exception exception )
		{
			_logger.LogError (
					exception : exception,
					message : "Exception occurred: {Message}",
					exception.Message
				);

			var exceptionDetails = GetExceptionDetails (
					exception : exception
				);

			var problemDetails = new ProblemDetails
								 {
									 Status = exceptionDetails.Status,
									 Type = exceptionDetails.Type,
									 Title = exceptionDetails.Title,
									 Detail = exceptionDetails.Detail
								 };

			if ( exceptionDetails.Errors is not null )
				problemDetails.Extensions[key : "errors"] = exceptionDetails.Errors;

			context.Response.StatusCode = exceptionDetails.Status;

			await context.Response.WriteAsJsonAsync (
					value : problemDetails
				);
		}
	}

	private static ExceptionDetails GetExceptionDetails ( Exception exception )
	{
		return exception switch
			   {
				   ValidationException validationException => new ExceptionDetails (
						   Status : StatusCodes.Status400BadRequest,
						   Type : "ValidationFailure",
						   Title : "Validation error",
						   Detail : "One or more validation errors has occurred",
						   Errors : validationException.Errors
					   ),
				   _ => new ExceptionDetails (
						   Status : StatusCodes.Status500InternalServerError,
						   Type : "ServerError",
						   Title : "Server error",
						   Detail : "An unexpected error has occurred",
						   Errors : null
					   )
			   };
	}

	internal sealed record ExceptionDetails ( int Status,
											  string Type,
											  string Title,
											  string Detail,
											  IEnumerable<object>? Errors );
}
