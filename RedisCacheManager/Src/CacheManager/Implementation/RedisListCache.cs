namespace CacheManager.Implementation;

/// <summary>
/// Implementation of Redis List operations.
/// </summary>
internal class RedisListCache(ICacheDb cacheDb, ILogger<RedisListCache> logger) : IRedisListCache
{
    public async Task<long> ListLeftPushAsync(string key, RedisValue value)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in ListLeftPushAsync for key: {Key}", key);
                return 0;
            }

            var length = await db.ListLeftPushAsync(key, value);
            logger.LogDebug("ListLeftPush: Key={Key}, Value={Value}, Length={Length}", key, value, length);
            return length;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in ListLeftPushAsync for key: {Key}", key);
            return 0;
        }
    }

    public async Task<long> ListLeftPushBulkAsync(string key, IEnumerable<RedisValue> values)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in ListLeftPushBulkAsync for key: {Key}", key);
                return 0;
            }

            var valueArray = values.ToArray();
            var length = await db.ListLeftPushAsync(key, valueArray);
            logger.LogDebug("ListLeftPushBulk: Key={Key}, Count={Count}, Length={Length}", key, valueArray.Length, length);
            return length;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in ListLeftPushBulkAsync for key: {Key}", key);
            return 0;
        }
    }

    public async Task<long> ListRightPushAsync(string key, RedisValue value)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in ListRightPushAsync for key: {Key}", key);
                return 0;
            }

            var length = await db.ListRightPushAsync(key, value);
            logger.LogDebug("ListRightPush: Key={Key}, Value={Value}, Length={Length}", key, value, length);
            return length;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in ListRightPushAsync for key: {Key}", key);
            return 0;
        }
    }

    public async Task<long> ListRightPushBulkAsync(string key, IEnumerable<RedisValue> values)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in ListRightPushBulkAsync for key: {Key}", key);
                return 0;
            }

            var valueArray = values.ToArray();
            var length = await db.ListRightPushAsync(key, valueArray);
            logger.LogDebug("ListRightPushBulk: Key={Key}, Count={Count}, Length={Length}", key, valueArray.Length, length);
            return length;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in ListRightPushBulkAsync for key: {Key}", key);
            return 0;
        }
    }

    public async Task<RedisValue> ListLeftPopAsync(string key)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in ListLeftPopAsync for key: {Key}", key);
                return RedisValue.Null;
            }

            var value = await db.ListLeftPopAsync(key);
            logger.LogDebug("ListLeftPop: Key={Key}, Value={Value}", key, value);
            return value;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in ListLeftPopAsync for key: {Key}", key);
            return RedisValue.Null;
        }
    }

    public async Task<RedisValue> ListRightPopAsync(string key)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in ListRightPopAsync for key: {Key}", key);
                return RedisValue.Null;
            }

            var value = await db.ListRightPopAsync(key);
            logger.LogDebug("ListRightPop: Key={Key}, Value={Value}", key, value);
            return value;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in ListRightPopAsync for key: {Key}", key);
            return RedisValue.Null;
        }
    }

    public async Task<RedisValue[]> ListRangeAsync(string key, long start, long stop)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in ListRangeAsync for key: {Key}", key);
                return Array.Empty<RedisValue>();
            }

            var values = await db.ListRangeAsync(key, start, stop);
            logger.LogDebug("ListRange: Key={Key}, Start={Start}, Stop={Stop}, Count={Count}", key, start, stop, values.Length);
            return values;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in ListRangeAsync for key: {Key}", key);
            return Array.Empty<RedisValue>();
        }
    }

    public async Task<long> ListLengthAsync(string key)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in ListLengthAsync for key: {Key}", key);
                return 0;
            }

            var length = await db.ListLengthAsync(key);
            logger.LogDebug("ListLength: Key={Key}, Length={Length}", key, length);
            return length;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in ListLengthAsync for key: {Key}", key);
            return 0;
        }
    }

    public async Task<RedisValue> ListGetByIndexAsync(string key, long index)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in ListGetByIndexAsync for key: {Key}", key);
                return RedisValue.Null;
            }

            var value = await db.ListGetByIndexAsync(key, index);
            logger.LogDebug("ListGetByIndex: Key={Key}, Index={Index}, Value={Value}", key, index, value);
            return value;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in ListGetByIndexAsync for key: {Key}, index: {Index}", key, index);
            return RedisValue.Null;
        }
    }

    public async Task<bool> ListSetByIndexAsync(string key, long index, RedisValue value)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in ListSetByIndexAsync for key: {Key}", key);
                return false;
            }

            await db.ListSetByIndexAsync(key, index, value);
            logger.LogDebug("ListSetByIndex: Key={Key}, Index={Index}, Value={Value}", key, index, value);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in ListSetByIndexAsync for key: {Key}, index: {Index}", key, index);
            return false;
        }
    }

    public async Task<long> ListInsertAsync(string key, RedisValue pivot, RedisValue value, bool after = false)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in ListInsertAsync for key: {Key}", key);
                return 0;
            }

            long result;
            if (after)
            {
                result = await db.ListInsertAfterAsync(key, pivot, value);
            }
            else
            {
                result = await db.ListInsertBeforeAsync(key, pivot, value);
            }
            
            logger.LogDebug("ListInsert: Key={Key}, Pivot={Pivot}, Value={Value}, After={After}, Result={Result}",
                key, pivot, value, after, result);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in ListInsertAsync for key: {Key}", key);
            return 0;
        }
    }

    public async Task<long> ListRemoveAsync(string key, RedisValue value, long count = 0)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in ListRemoveAsync for key: {Key}", key);
                return 0;
            }

            var removed = await db.ListRemoveAsync(key, value, count);
            logger.LogDebug("ListRemove: Key={Key}, Value={Value}, Count={Count}, Removed={Removed}", key, value, count, removed);
            return removed;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in ListRemoveAsync for key: {Key}", key);
            return 0;
        }
    }

    public Task<RedisValue?> ListLeftPopAsync(string key, TimeSpan timeout)
    {
        // Blocking pop not implemented - use ListLeftPopAsync without timeout
        logger.LogWarning("Blocking ListLeftPopAsync not implemented. Use ListLeftPopAsync instead.");
        return Task.FromResult<RedisValue?>(RedisValue.Null);
    }

    public Task<KeyValuePair<RedisChannel, RedisValue>?> ListLeftPopAsync(string[] keys, TimeSpan timeout)
    {
        // Blocking multi-key pop not implemented
        logger.LogWarning("Blocking ListLeftPopAsync with multiple keys not implemented.");
        return Task.FromResult<KeyValuePair<RedisChannel, RedisValue>?>(null);
    }
}
