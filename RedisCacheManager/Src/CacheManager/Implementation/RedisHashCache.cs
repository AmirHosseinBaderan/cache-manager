namespace CacheManager.Implementation;

/// <summary>
/// Implementation of Redis Hash operations.
/// </summary>
internal class RedisHashCache(ICacheDb cacheDb, ILogger<RedisHashCache> logger) : IRedisHashCache
{
    public async Task<bool> HashSetAsync(string key, string field, RedisValue value)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in HashSetAsync for key: {Key}", key);
                return false;
            }

            var result = await db.HashSetAsync(key, field, value);
            logger.LogDebug("HashSet: Key={Key}, Field={Field}, Success={Success}", key, field, result);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in HashSetAsync for key: {Key}, field: {Field}", key, field);
            return false;
        }
    }

    public async Task HashSetBulkAsync(string key, IEnumerable<HashEntry> entries)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in HashSetBulkAsync for key: {Key}", key);
                return;
            }

            var entryArray = entries.ToArray();
            await db.HashSetAsync(key, entryArray);
            logger.LogDebug("HashSetBulk: Key={Key}, Count={Count}", key, entryArray.Length);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in HashSetBulkAsync for key: {Key}", key);
        }
    }

    public async Task<RedisValue> HashGetAsync(string key, string field)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in HashGetAsync for key: {Key}", key);
                return RedisValue.Null;
            }

            var value = await db.HashGetAsync(key, field);
            logger.LogDebug("HashGet: Key={Key}, Field={Field}, Found={Found}", key, field, !value.IsNullOrEmpty);
            return value;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in HashGetAsync for key: {Key}, field: {Field}", key, field);
            return RedisValue.Null;
        }
    }

    public async Task<RedisValue[]> HashGetBulkAsync(string key, IEnumerable<string> fields)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in HashGetBulkAsync for key: {Key}", key);
                return Array.Empty<RedisValue>();
            }

            var fieldArray = fields.Select(f => (RedisValue)f).ToArray();
            var values = await db.HashGetAsync(key, fieldArray);
            return values;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in HashGetBulkAsync for key: {Key}", key);
            return Array.Empty<RedisValue>();
        }
    }

    public async Task<HashEntry[]> HashGetAllAsync(string key)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in HashGetAllAsync for key: {Key}", key);
                return Array.Empty<HashEntry>();
            }

            var entries = await db.HashGetAllAsync(key);
            logger.LogDebug("HashGetAll: Key={Key}, Count={Count}", key, entries.Length);
            return entries;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in HashGetAllAsync for key: {Key}", key);
            return Array.Empty<HashEntry>();
        }
    }

    public async Task<long> HashDeleteAsync(string key, IEnumerable<string> fields)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in HashDeleteAsync for key: {Key}", key);
                return 0;
            }

            var fieldArray = fields.Select(f => (RedisValue)f).ToArray();
            var result = await db.HashDeleteAsync(key, fieldArray);
            logger.LogDebug("HashDelete: Key={Key}, Count={Count}", key, result);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in HashDeleteAsync for key: {Key}", key);
            return 0;
        }
    }

    public async Task<bool> HashExistsAsync(string key, string field)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in HashExistsAsync for key: {Key}", key);
                return false;
            }

            var exists = await db.HashExistsAsync(key, field);
            logger.LogDebug("HashExists: Key={Key}, Field={Field}, Exists={Exists}", key, field, exists);
            return exists;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in HashExistsAsync for key: {Key}, field: {Field}", key, field);
            return false;
        }
    }

    public async Task<double> HashIncrementAsync(string key, string field, double increment)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in HashIncrementAsync for key: {Key}", key);
                return 0;
            }

            var result = await db.HashIncrementAsync(key, field, increment);
            logger.LogDebug("HashIncrement (double): Key={Key}, Field={Field}, Increment={Increment}, Result={Result}",
                key, field, increment, result);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in HashIncrementAsync for key: {Key}, field: {Field}", key, field);
            return 0;
        }
    }

    public async Task<long> HashIncrementAsync(string key, string field, long increment)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in HashIncrementAsync for key: {Key}", key);
                return 0;
            }

            var result = await db.HashIncrementAsync(key, field, increment);
            logger.LogDebug("HashIncrement (long): Key={Key}, Field={Field}, Increment={Increment}, Result={Result}",
                key, field, increment, result);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in HashIncrementAsync for key: {Key}, field: {Field}", key, field);
            return 0;
        }
    }

    public async Task<double> HashDecrementAsync(string key, string field, double decrement)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in HashDecrementAsync for key: {Key}", key);
                return 0;
            }

            var result = await db.HashDecrementAsync(key, field, decrement);
            logger.LogDebug("HashDecrement (double): Key={Key}, Field={Field}, Decrement={Decrement}, Result={Result}",
                key, field, decrement, result);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in HashDecrementAsync for key: {Key}, field: {Field}", key, field);
            return 0;
        }
    }

    public async Task<long> HashDecrementAsync(string key, string field, long decrement)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in HashDecrementAsync for key: {Key}", key);
                return 0;
            }

            var result = await db.HashDecrementAsync(key, field, decrement);
            logger.LogDebug("HashDecrement (long): Key={Key}, Field={Field}, Decrement={Decrement}, Result={Result}",
                key, field, decrement, result);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in HashDecrementAsync for key: {Key}, field: {Field}", key, field);
            return 0;
        }
    }

    public async Task<RedisValue[]> HashKeysAsync(string key)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in HashKeysAsync for key: {Key}", key);
                return Array.Empty<RedisValue>();
            }

            var entries = await db.HashGetAllAsync(key);
            return entries.Select(e => e.Name).ToArray();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in HashKeysAsync for key: {Key}", key);
            return Array.Empty<RedisValue>();
        }
    }

    public async Task<RedisValue[]> HashValuesAsync(string key)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in HashValuesAsync for key: {Key}", key);
                return Array.Empty<RedisValue>();
            }

            var entries = await db.HashGetAllAsync(key);
            return entries.Select(e => e.Value).ToArray();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in HashValuesAsync for key: {Key}", key);
            return Array.Empty<RedisValue>();
        }
    }

    public async Task<long> HashLengthAsync(string key)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in HashLengthAsync for key: {Key}", key);
                return 0;
            }

            var entries = await db.HashGetAllAsync(key);
            var length = entries.Length;
            logger.LogDebug("HashLength: Key={Key}, Length={Length}", key, length);
            return length;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in HashLengthAsync for key: {Key}", key);
            return 0;
        }
    }
}
