namespace Bookify.Application.Abstractions.Caching;

// We use cancellationToken because we want to support cancellation of our methods!
public interface ICacheService
{
	Task<T?> GetAsync <T> ( string key, CancellationToken cancellationToken = default(CancellationToken) );

	// Cache key, cache value, and cache expiration
	Task SetAsync <T> ( string key,
						T value,
						TimeSpan? expiration = null,
						CancellationToken cancellationToken = default(CancellationToken) );

	Task RemoveAsync ( string key, CancellationToken cancellationToken = default(CancellationToken) );
}
