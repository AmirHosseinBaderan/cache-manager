using CacheManager.Abstraction;
using CacheManager.Core;
using CacheManager.Core.Connection;
using StackExchange.Redis;

namespace CacheManager.Implementation;

/// <summary>
/// Implementation of Redis Bitmap operations.
/// Bitmaps are ideal for tracking boolean states or counting occurrences efficiently.
/// </summary>
public class RedisBitmapCache : IRedisBitmapCache
{
    private readonly ICacheCore _cacheCore;
    private readonly ICacheDb _cacheDb;

    public RedisBitmapCache(ICacheCore cacheCore, ICacheDb cacheDb)
    {
        _cacheCore = cacheCore;
        _cacheDb = cacheDb;
    }

    /// <inheritdoc />
    public async Task<bool> SetBitAsync(string key, long offset, bool value)
    {
        var db = await _cacheDb.GetDatabaseAsync().ConfigureAwait(false);
        return await db.StringSetBitAsync(key, offset, value).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<bool> GetBitAsync(string key, long offset)
    {
        var db = await _cacheDb.GetDatabaseAsync().ConfigureAwait(false);
        return await db.StringGetBitAsync(key, offset).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<long> BitCountAsync(string key, long start = 0, long end = -1)
    {
        var db = await _cacheDb.GetDatabaseAsync().ConfigureAwait(false);
        return await db.StringBitCountAsync(key, start, end).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<long> BitAndAsync(string destination, IEnumerable<string> keys)
    {
        var db = await _cacheDb.GetDatabaseAsync().ConfigureAwait(false);
        var keysArray = keys.Select(k => (RedisKey)k).ToArray();
        return await db.StringBitOperationAsync(Bitwise.And, destination, keysArray).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<long> BitOrAsync(string destination, IEnumerable<string> keys)
    {
        var db = await _cacheDb.GetDatabaseAsync().ConfigureAwait(false);
        var keysArray = keys.Select(k => (RedisKey)k).ToArray();
        return await db.StringBitOperationAsync(Bitwise.Or, destination, keysArray).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<long> BitXorAsync(string destination, IEnumerable<string> keys)
    {
        var db = await _cacheDb.GetDatabaseAsync().ConfigureAwait(false);
        var keysArray = keys.Select(k => (RedisKey)k).ToArray();
        return await db.StringBitOperationAsync(Bitwise.Xor, destination, keysArray).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<long> BitNotAsync(string destination, string source)
    {
        var db = await _cacheDb.GetDatabaseAsync().ConfigureAwait(false);
        return await db.StringBitOperationAsync(Bitwise.Not, destination, source).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<long> BitPositionAsync(string key, bool bit, long start = 0, long end = -1)
    {
        var db = await _cacheDb.GetDatabaseAsync().ConfigureAwait(false);
        return await db.StringBitPositionAsync(key, bit, start, end).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<long> BitLengthAsync(string key)
    {
        var db = await _cacheDb.GetDatabaseAsync().ConfigureAwait(false);
        return await db.StringBitLengthAsync(key).ConfigureAwait(false);
    }
}
