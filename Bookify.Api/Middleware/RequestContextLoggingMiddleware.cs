using Serilog.Context;

namespace Bookify.Api.Middleware;

internal sealed class RequestContextLoggingMiddleware ( RequestDelegate next )
{
	private const string CorrelationIdHeaderName = "X-Correlation-Id";

	public Task Invoke ( HttpContext context )
	{
		using ( LogContext.PushProperty (
					   name : "CorrelationId",
					   value : GetCorrelationId (
							   context : context
						   )
				   ) )
			return next.Invoke (
					context : context
				);
	}

	private static string GetCorrelationId ( HttpContext context )
	{
		context.Request.Headers.TryGetValue (
				key : CorrelationIdHeaderName,
				value : out var correlationId
			);

		return correlationId.FirstOrDefault() ?? context.TraceIdentifier;
	}
}
