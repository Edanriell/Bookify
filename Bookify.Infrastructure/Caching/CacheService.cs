using System.Buffers;
using System.Text.Json;
using Bookify.Application.Abstractions.Caching;
using Microsoft.Extensions.Caching.Distributed;

namespace Bookify.Infrastructure.Caching;

internal sealed class CacheService : ICacheService
{
	// Memory cache
	private readonly IDistributedCache _cache;

	public CacheService ( IDistributedCache cache ) { _cache = cache; }

	public async Task<T?> GetAsync <T> ( string key, CancellationToken cancellationToken = default(CancellationToken) )
	{
		// Installed caching library returns an array of bytes. This means that we need to take care of an deserializing
		// the array of bytes into an object in memory. 
		var bytes = await _cache.GetAsync (
							key : key,
							token : cancellationToken
						);

		// if bytes that we are getting back from our cache are null then we will return a default value from the
		// getAsync method
		// Otherwise we are going to try deserialize the array of bytes into the generic T value that we get as a
		// generic
		// argument
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

	// Here we are using null forgiving argument because we are confident that if the array of bytes is not null
	// we will be able to deserialize the value back into an object of type T.
	private static T Deserialize <T> ( byte[] bytes ) => JsonSerializer.Deserialize<T> (
			utf8Json : bytes
		)!;

	private static byte[] Serialize <T> ( T value )
	{
		// We are using buffer to accept our array of bytes.
		var buffer = new ArrayBufferWriter<byte>();

		using var writer = new Utf8JsonWriter (
				bufferWriter : buffer
			);

		// And we are serializing the object value using a UTF-8 JSON writer.
		JsonSerializer.Serialize (
				writer : writer,
				value : value
			);

		// When serialization is completed, we can take the array of bytes by calling
		// the buffer, accessing the written span property and calling ToArray.
		return buffer.WrittenSpan.ToArray();
	}
}
