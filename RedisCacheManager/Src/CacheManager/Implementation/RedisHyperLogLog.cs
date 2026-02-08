using CacheManager.Abstraction;
using CacheManager.Core.Connection;
using StackExchange.Redis;

namespace CacheManager.Implementation;

/// <summary>
///     Implementation of Redis HyperLogLog operations
/// </summary>
public class RedisHyperLogLog : IRedisHyperLogLog
{
    private readonly ICacheCore _core;

    public RedisHyperLogLog(CacheCore core)
    {
        _core = core;
    }

    public RedisHyperLogLog(ICacheCore core)
    {
        _core = core;
    }

    /// <inheritdoc />
    public async Task<bool> PfAddAsync(string key, params string[] values)
    {
        var db = _core.GetDatabase();
        var redisValues = values.Select(v => (RedisValue)v).ToArray();
        return await db.HyperLogLogAddAsync(key, redisValues);
    }

    /// <inheritdoc />
    public async Task<long> PfCountAsync(params string[] keys)
    {
        var db = _core.GetDatabase();
        return await db.HyperLogLogLengthAsync(keys);
    }

    /// <inheritdoc />
    public async Task<bool> PfMergeAsync(string destination, params string[] sourceKeys)
    {
        var db = _core.GetDatabase();
        await db.HyperLogLogMergeAsync(destination, sourceKeys);
        return true;
    }
}
