using Bookify.Application.Abstractions.Caching;
using Bookify.Domain.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bookify.Application.Abstractions.Behaviors;

internal sealed class QueryCachingBehavior <TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
// TRequest must be an ICachedQuery 
	where TRequest : ICachedQuery
	// And also TResponse will have to  be a result
	where TResponse : Result
{
	private readonly ICacheService _cacheService;
	private readonly ILogger<QueryCachingBehavior<TRequest, TResponse>> _logger;

	public QueryCachingBehavior ( ICacheService cacheService,
								  ILogger<QueryCachingBehavior<TRequest, TResponse>> logger )
	{
		_cacheService = cacheService;
		_logger = logger;
	}

	public async Task<TResponse> Handle ( TRequest request,
										  RequestHandlerDelegate<TResponse> next,
										  CancellationToken cancellationToken )
	{
		// We are trying to get back a TResponse for this cache key. We can access the cache key property
		// because our TRequest is an ICachedQuery implementation, and this is the reason we defined
		// this interface in the first place.
		var cachedResult = await _cacheService.GetAsync<TResponse> (
								   key : request.CacheKey,
								   cancellationToken : cancellationToken
							   );

		var name = typeof(TRequest).Name;
		// If a cached result is not null, we are going to log that this is a cache hit
		// and we are going to return a cached value 
		if ( cachedResult is not null )
		{
			// Structured logging containing the query name
			_logger.LogInformation (
					message : "Cache hit for {Query}",
					name
				);

			return cachedResult;
		}

		// Otherwise, we are going to log that we encountered a cache miss and we are just
		// going to execute the query handler as normal.
		_logger.LogInformation (
				message : "Cache miss for {Query}",
				name
			);

		var result = await next();

		// If we get a success result, only then we are going to set the value in the cache using the cache
		// key, the result that we get back, which is a result implementation containing a wrpped value,
		// and also we are passing in the expiration time and the cancellation token. 
		if ( result.IsSuccess )
		{
			await _cacheService.SetAsync (
					key : request.CacheKey,
					value : result,
					expiration : request.Expiration,
					cancellationToken : cancellationToken
				);
		}

		return result;
	}
}
