using System.Buffers;
using System.Text.Json;
using Bookify.Application.Abstractions.Caching;
using Microsoft.Extensions.Caching.Distributed;

namespace Bookify.Infrastructure.Caching;

internal sealed class CacheService : ICacheService
{
	private readonly IDistributedCache _cache;

	public CacheService ( IDistributedCache cache ) { _cache = cache; }

	public async Task<T?> GetAsync <T> ( string key, CancellationToken cancellationToken = default(CancellationToken) )
	{
		var bytes = await _cache.GetAsync (
							key : key,
							token : cancellationToken
						);

		return bytes is null
				   ? default(T?)
				   : Deserialize<T> (
						   bytes : bytes
					   );
	}

	public Task SetAsync <T> ( string key,
							   T value,
							   TimeSpan? expiration = null,
							   CancellationToken cancellationToken = default(CancellationToken) )
	{
		var bytes = Serialize (
				value : value
			);

		return _cache.SetAsync (
				key : key,
				value : bytes,
				options : CacheOptions.Create (
						expiration : expiration
					),
				token : cancellationToken
			);
	}

	public Task RemoveAsync ( string key, CancellationToken cancellationToken = default(CancellationToken) )
		=> _cache.RemoveAsync (
				key : key,
				token : cancellationToken
			);

	private static T Deserialize <T> ( byte[] bytes ) => JsonSerializer.Deserialize<T> (
			utf8Json : bytes
		)!;

	private static byte[] Serialize <T> ( T value )
	{
		var buffer = new ArrayBufferWriter<byte>();

		using var writer = new Utf8JsonWriter (
				bufferWriter : buffer
			);

		JsonSerializer.Serialize (
				writer : writer,
				value : value
			);

		return buffer.WrittenSpan.ToArray();
	}
}
