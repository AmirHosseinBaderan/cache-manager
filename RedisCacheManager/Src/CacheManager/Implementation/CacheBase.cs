namespace CacheManager.Implementation;

/// <summary>
/// Provides a high-level abstraction over Redis cache operations,
/// including Get, Set, and Remove, with detailed logging and exception handling.
/// </summary>
internal class CacheBase(ICacheDb cacheDb, ILogger<CacheBase> logger) : ICacheBase
{
    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await cacheDb.DisposeAsync();
    }

    /// <inheritdoc />
    public async Task<RedisValue> GetItemAsync(string key)
    {
        try
        {
            logger.LogDebug("Attempting to retrieve cache key: {Key}", key);
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable when fetching key: {Key}", key);
                return RedisValue.Null;
            }

            var value = await db.StringGetAsync(key);
            if (value.IsNullOrEmpty)
                logger.LogInformation("Cache miss for key: {Key}", key);
            else
                logger.LogInformation("Cache hit for key: {Key}", key);

            return value;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving cache key: {Key}", key);
            return RedisValue.Null;
        }
    }

    /// <inheritdoc />
    public async Task<RedisValue> GetOrSetItemAsync(string key, Func<Task<RedisValue>> action,
        Func<RedisValue, bool>? setIf = null)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in GetOrSetItemAsync for key: {Key}", key);
                return await action();
            }

            var value = await db.StringGetAsync(key);
            if (value.IsNullOrEmpty)
            {
                logger.LogInformation("Cache miss — generating and setting new value for key: {Key}", key);
                var res = await action();
                if (setIf is null || setIf(res))
                    return await SetItemAsync(key, res);
                return res;
            }

            logger.LogDebug("Cache hit for key: {Key}", key);
            return value;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception in GetOrSetItemAsync for key: {Key}", key);
            return await action();
        }
    }

    /// <inheritdoc />
    public async Task<RedisValue> GetOrSetItemAsync(string key, CacheDuration cacheDuration,
        Func<Task<RedisValue>> action, Func<RedisValue, bool>? setIf = null)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in GetOrSetItemAsync (with duration) for key: {Key}",
                    key);
                return await action();
            }

            var value = await db.StringGetAsync(key);
            if (value.IsNullOrEmpty)
            {
                logger.LogInformation("Cache miss — setting new value for key: {Key} with duration {Duration}", key,
                    cacheDuration);
                var res = await action();
                if (setIf is null || setIf(res))
                    return await SetItemAsync(key, res, cacheDuration.ToTimeSpan());
                return res;
            }

            logger.LogDebug("Cache hit for key: {Key}", key);
            return value;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception in GetOrSetItemAsync (with duration) for key: {Key}", key);
            return await action();
        }
    }

    /// <inheritdoc />
    public async Task<RedisValue> GetOrSetItemAsync(string key, Func<RedisValue> action,
        Func<RedisValue, bool>? setIf = null)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis unavailable in GetOrSetItemAsync (sync) for key: {Key}", key);
                return action();
            }

            var value = await db.StringGetAsync(key);
            if (value.IsNullOrEmpty)
            {
                logger.LogInformation("Cache miss (sync) — generating value for key: {Key}", key);
                var res = action();
                if (setIf is null || setIf(res))
                    return await SetItemAsync(key, res);
                return res;
            }

            logger.LogDebug("Cache hit (sync) for key: {Key}", key);
            return value;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception in GetOrSetItemAsync (sync) for key: {Key}", key);
            return action();
        }
    }

    /// <inheritdoc />
    public async Task<RedisValue> GetOrSetItemAsync(string key, CacheDuration cacheDuration, Func<RedisValue> action,
        Func<RedisValue, bool>? setIf = null)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis unavailable in GetOrSetItemAsync (sync, duration) for key: {Key}", key);
                return action();
            }

            var value = await db.StringGetAsync(key);
            if (value.IsNullOrEmpty)
            {
                logger.LogInformation("Cache miss (sync) — setting key: {Key} with duration {Duration}", key,
                    cacheDuration);
                var res = action();
                if (setIf is null || setIf(res))
                    return await SetItemAsync(key, res, cacheDuration.ToTimeSpan());
                return res;
            }

            logger.LogDebug("Cache hit (sync, duration) for key: {Key}", key);
            return value;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception in GetOrSetItemAsync (sync, duration) for key: {Key}", key);
            return action();
        }
    }

    /// <inheritdoc />
    public async Task RemoveItemAsync(string key)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis unavailable in RemoveItemAsync for key: {Key}", key);
                return;
            }

            await db.StringGetDeleteAsync(key);
            logger.LogInformation("Cache entry removed for key: {Key}", key);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error removing cache key: {Key}", key);
        }
    }

    /// <inheritdoc />
    public async Task<RedisValue> SetItemAsync(string key, RedisValue? obj)
        => await SetItemAsync(key, obj, cacheTime: null);

    /// <inheritdoc />
    public async Task<RedisValue> SetItemAsync(string key, RedisValue? obj, TimeSpan? cacheTime)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null || obj is null)
            {
                logger.LogWarning("Redis unavailable or value null when setting key: {Key}", key);
                return obj ?? RedisValue.Null;
            }

            await db.StringSetAsync(key, obj.Value, cacheTime);
            logger.LogInformation("Cache key set: {Key} (TTL: {TTL})", key, cacheTime?.ToString() ?? "None");

            return obj.Value;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception in SetItemAsync for key: {Key}", key);
            return obj ?? RedisValue.Null;
        }
    }
}