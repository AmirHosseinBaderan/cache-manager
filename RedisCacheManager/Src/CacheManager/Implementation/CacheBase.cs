using CacheManager.Abstraction;
using CacheManager.Configuration;
using CacheManager.Core;
using Microsoft.Extensions.Logging;

namespace CacheManager.Implementation;

internal class CacheBase(ICacheDb cacheDb, ILogger<CacheBase> logger) : ICacheBase
{
    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await cacheDb.DisposeAsync();
    }

    public async Task<RedisValue> GetItemAsync(string key)
    {
        try
        {
            IDatabase? db = await cacheDb.GetDataBaseAsync();
            return db is null
                ? RedisValue.Null
                : await db.StringGetAsync(key);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception in GetItemAsync");
            return RedisValue.Null;
        }
    }

    public async Task<RedisValue> GetOrSetItemAsync(string key, Func<Task<RedisValue>> action,
        Func<RedisValue, bool>? setIf = null)
    {
        try
        {
            IDatabase? db = await cacheDb.GetDataBaseAsync();
            if (db is null)
                return await action();

            RedisValue value = await db.StringGetAsync(key);
            if (value.IsNullOrEmpty)
            {
                var res = await action();
                if (setIf is null || setIf(res))
                    return await SetItemAsync(key, res);
            }

            return value;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception in GetOrSetItemAsync");
            return await action();
        }
    }

    public async Task<RedisValue> GetOrSetItemAsync(string key, CacheDuration cacheDuration,
        Func<Task<RedisValue>> action, Func<RedisValue, bool>? setIf = null)
    {
        try
        {
            IDatabase? db = await cacheDb.GetDataBaseAsync();
            if (db is null)
                return await action();

            RedisValue value = await db.StringGetAsync(key);
            if (value.IsNullOrEmpty)
            {
                var res = await action();
                if (setIf is null || setIf(res))
                    return await SetItemAsync(key, res, cacheDuration.ToTimeSpan());
            }

            return value;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception in GetOrSetItemAsync");
            return await action();
        }
    }

    public async Task<RedisValue> GetOrSetItemAsync(string key, Func<RedisValue> action,
        Func<RedisValue, bool>? setIf = null)
    {
        try
        {
            IDatabase? db = await cacheDb.GetDataBaseAsync();
            if (db is null)
                return action();

            RedisValue value = await db.StringGetAsync(key);
            if (value.IsNullOrEmpty)
            {
                var res = action();
                if (setIf is null || setIf(res))
                    return await SetItemAsync(key, res);
            }

            return value;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception in GetOrSetItemAsync");
            return action();
        }
    }

    public async Task<RedisValue> GetOrSetItemAsync(string key, CacheDuration cacheDuration, Func<RedisValue> action,
        Func<RedisValue, bool>? setIf = null)
    {
        try
        {
            IDatabase? db = await cacheDb.GetDataBaseAsync();
            if (db is null)
                return action();

            RedisValue value = await db.StringGetAsync(key);
            if (value.IsNullOrEmpty)
            {
                var res = action();
                if (setIf is null || setIf(res))
                    return await SetItemAsync(key, res, cacheDuration.ToTimeSpan());
            }

            return value;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception in GetOrSetItemAsync");
            return action();
        }
    }

    public async Task RemoveItemAsync(string key)
    {
        try
        {
            IDatabase? db = await cacheDb.GetDataBaseAsync();
            if (db is null)
                return;

            await db.StringGetDeleteAsync(key);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception in RemoveItemAsync");
            return;
        }
    }

    public async Task<RedisValue> SetItemAsync(string key, RedisValue? obj)
        => await SetItemAsync(key, obj, cacheTime: null);

    public async Task<RedisValue> SetItemAsync(string key, RedisValue? obj, TimeSpan? cacheTime)
    {
        try
        {
            IDatabase? db = await cacheDb.GetDataBaseAsync();
            if (db is null || obj is null)
                return obj ?? RedisValue.Null;

            await db.StringSetAsync(key, (RedisValue)obj, cacheTime);
            return obj ?? RedisValue.Null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception in SetItemAsync");
            return obj ?? RedisValue.Null;
        }
    }
}