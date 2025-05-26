using Microsoft.Extensions.Caching.Distributed;

namespace Bookify.Infrastructure.Caching;

public static class CacheOptions
{
	public static DistributedCacheEntryOptions DefaultExpiration => new()
																	{
																		AbsoluteExpirationRelativeToNow
																			= TimeSpan.FromMinutes (
																					minutes : 1
																				)
																	};

	// Create method allows us to pass in an expiration time span. If the value that is passed in is not null,
	// We are going to create a new distributed cache entry options instance and set the absolute expiration
	// relative to null. If the expiration object is null, then we are going to use the default expiration time
	// which is one minute
	public static DistributedCacheEntryOptions Create ( TimeSpan? expiration ) => expiration is not null
			? new DistributedCacheEntryOptions
			  {
				  AbsoluteExpirationRelativeToNow = expiration
			  }
			: DefaultExpiration;
}
