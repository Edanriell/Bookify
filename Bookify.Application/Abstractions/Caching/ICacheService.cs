namespace Bookify.Application.Abstractions.Caching;

public interface ICacheService
{
	Task<T?> GetAsync <T> ( string key, CancellationToken cancellationToken = default(CancellationToken) );

	Task SetAsync <T> ( string key,
						T value,
						TimeSpan? expiration = null,
						CancellationToken cancellationToken = default(CancellationToken) );

	Task RemoveAsync ( string key, CancellationToken cancellationToken = default(CancellationToken) );
}
