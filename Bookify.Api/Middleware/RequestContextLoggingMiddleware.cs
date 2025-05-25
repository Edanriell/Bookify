using Serilog.Context;

namespace Bookify.Api.Middleware;

// This implementation gives us the flexibility of either taking in the correlation ID externally from
// another service talking with our API allowign us to trace a single request across multiple
// services in a microservice environment. Otherwise, if this header isn't present, then we are going to use 
// the trace identifier for this API request. 
public class RequestContextLoggingMiddleware
{
	// Represents correlation id header name.
	private const string CorrelationIdHeaderName = "X-Correlation-Id";

	private readonly RequestDelegate _next;

	public RequestContextLoggingMiddleware ( RequestDelegate next ) { _next = next; }

	// Method can be named Invoke or InvokeAsync. This method accepts and HTTP context
	// instance that it can use to process the request delegate. 
	public Task Invoke ( HttpContext httpContext )
	{
		
		// Pushing property into the log context. Get correlation method is taking an HTTP
		// context instance, and the push property method returns a disposable instance,
		// which is why we are using a using statement. 
		using ( LogContext.PushProperty (
					   name : "Correlation,Id",
					   value : GetCorrelationId (
							   httpContext : httpContext
						   )
				   ) )
		{
			// Api request is wrapped into the body of the using statement so that our
			// log context lives for the duration of the API request. 
			return _next (
					context : httpContext
				);
		}
	}

	private static string GetCorrelationId ( HttpContext httpContext )
	{
		// We are pulling out our correlationId from the respective request header if it exists.
		// Otherwise, we are going to generate a value for the correlation ID. 
		// Accessing the request value by taking in the HTTP context and then using the request, and
		// then we can access the header collection, and we will try to get a value with this key.
		// In result, capturing the correlactionId in an out variable. 
		httpContext.Request.Headers.TryGetValue (
				key : CorrelationIdHeaderName,
				value : out var correlationId
			);

		return correlationId.FirstOrDefault() ?? httpContext.TraceIdentifier;
	}
}
