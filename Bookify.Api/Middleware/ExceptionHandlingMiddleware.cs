using Bookify.Application.Abstractions.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Api.Middleware;

// We are defining our middleware by convention, which means that we have
// one public method called invokeAsync. 
public class ExceptionHandlingMiddleware
{
	private readonly ILogger<ExceptionHandlingMiddleware> _logger;
	private readonly RequestDelegate _next;

	public ExceptionHandlingMiddleware(
		RequestDelegate next,
		ILogger<ExceptionHandlingMiddleware> logger)
	{
		_next = next;
		_logger = logger;
	}

	// It accepts an HTTP context, and we are injecting the request delegate 
	// from the constructor. We are also injecting an ILogger instance so that we can log my exceptions.
	public async Task InvokeAsync(HttpContext context)
	{
		try
		{
			// We are wrapping our exception of the request delegate in a try catch statement,
			// and it the catch block we are exemining the exception that we get to extract
			// some exception details 
			await _next(context);
		}
		catch (Exception exception)
		{
			_logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

			var exceptionDetails = GetExceptionDetails(exception);

			// We are creating a new problem details instance.
			var problemDetails = new ProblemDetails
								 {
									 Status = exceptionDetails.Status,
									 Type = exceptionDetails.Type,
									 Title = exceptionDetails.Title,
									 Detail = exceptionDetails.Detail
								 };

			// Then we are expending this problem details instance with an error's property
			// in case of a validation exception it is going to contain our collection of errors inside. 
			if (exceptionDetails.Errors is not null) problemDetails.Extensions["errors"] = exceptionDetails.Errors;

			// We are setting the appropriate status code, and we are writing the problem
			// details as a JSON response in our response body. 
			context.Response.StatusCode = exceptionDetails.Status;

			await context.Response.WriteAsJsonAsync(problemDetails);
		}
	}

	// In this method, all we are doing is writing a switch expression on the exception and
	// checking if this is a validation exception, which is an exception that we defined in the
	// application project, and we are throwing it in our validation behaviour mediator pipeline.
	// If we do end up catching this exception, then we have a validation failure, and we are going
	// to return an appropriate response. We also want to capture any errors that were part of this validation
	// exception and return them with the exception details. 
	private static ExceptionDetails GetExceptionDetails(Exception exception)
	{
		return exception switch
			   {
				   ValidationException validationException => new ExceptionDetails(
					   StatusCodes.Status400BadRequest,
					   "ValidationFailure",
					   "Validation error",
					   "One or more validation errors has occurred",
					   validationException.Errors),
				   _ => new ExceptionDetails(
					   // If it is not validation exception, then we have an internal
					   // server error and there is nothing we can do. 
					   StatusCodes.Status500InternalServerError,
					   "ServerError",
					   "Server error",
					   "An unexpected error has occurred",
					   null)
			   };
	}

	internal record ExceptionDetails(
		int Status,
		string Type,
		string Title,
		string Detail,
		IEnumerable<object>? Errors);
}