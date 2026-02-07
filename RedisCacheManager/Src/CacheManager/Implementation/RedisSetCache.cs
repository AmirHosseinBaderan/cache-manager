namespace CacheManager.Implementation;

/// <summary>
/// Implementation of Redis Set operations.
/// </summary>
internal class RedisSetCache(ICacheDb cacheDb, ILogger<RedisSetCache> logger) : IRedisSetCache
{
    public async Task<long> SetAddAsync(string key, RedisValue value)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in SetAddAsync for key: {Key}", key);
                return 0;
            }

            var result = await db.SetAddAsync(key, value);
            logger.LogDebug("SetAdd: Key={Key}, Value={Value}, Result={Result}", key, value, result);
            return result ? 1L : 0L;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in SetAddAsync for key: {Key}", key);
            return 0;
        }
    }

    public async Task<long> SetAddBulkAsync(string key, IEnumerable<RedisValue> values)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in SetAddBulkAsync for key: {Key}", key);
                return 0;
            }

            var valueArray = values.ToArray();
            var result = await db.SetAddAsync(key, valueArray);
            logger.LogDebug("SetAddBulk: Key={Key}, Count={Count}, Result={Result}", key, valueArray.Length, result);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in SetAddBulkAsync for key: {Key}", key);
            return 0;
        }
    }

    public async Task<long> SetRemoveAsync(string key, IEnumerable<RedisValue> values)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in SetRemoveAsync for key: {Key}", key);
                return 0;
            }

            var valueArray = values.ToArray();
            var result = await db.SetRemoveAsync(key, valueArray);
            logger.LogDebug("SetRemove: Key={Key}, Count={Count}, Result={Result}", key, valueArray.Length, result);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in SetRemoveAsync for key: {Key}", key);
            return 0;
        }
    }

    public async Task<RedisValue[]> SetMembersAsync(string key)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in SetMembersAsync for key: {Key}", key);
                return Array.Empty<RedisValue>();
            }

            var members = await db.SetMembersAsync(key);
            logger.LogDebug("SetMembers: Key={Key}, Count={Count}", key, members.Length);
            return members;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in SetMembersAsync for key: {Key}", key);
            return Array.Empty<RedisValue>();
        }
    }

    public async Task<bool> SetContainsAsync(string key, RedisValue value)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in SetContainsAsync for key: {Key}", key);
                return false;
            }

            var exists = await db.SetContainsAsync(key, value);
            logger.LogDebug("SetContains: Key={Key}, Value={Value}, Exists={Exists}", key, value, exists);
            return exists;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in SetContainsAsync for key: {Key}", key);
            return false;
        }
    }

    public async Task<long> SetLengthAsync(string key)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in SetLengthAsync for key: {Key}", key);
                return 0;
            }

            var length = await db.SetLengthAsync(key);
            logger.LogDebug("SetLength: Key={Key}, Length={Length}", key, length);
            return length;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in SetLengthAsync for key: {Key}", key);
            return 0;
        }
    }

    public async Task<RedisValue> SetRandomMemberAsync(string key)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in SetRandomMemberAsync for key: {Key}", key);
                return RedisValue.Null;
            }

            var member = await db.SetRandomMemberAsync(key);
            logger.LogDebug("SetRandomMember: Key={Key}, Member={Member}", key, member);
            return member;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in SetRandomMemberAsync for key: {Key}", key);
            return RedisValue.Null;
        }
    }

    public async Task<RedisValue[]> SetRandomMembersAsync(string key, long count)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in SetRandomMembersAsync for key: {Key}", key);
                return Array.Empty<RedisValue>();
            }

            var members = await db.SetRandomMembersAsync(key, count);
            logger.LogDebug("SetRandomMembers: Key={Key}, Count={Count}, Got={Got}", key, count, members.Length);
            return members;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in SetRandomMembersAsync for key: {Key}", key);
            return Array.Empty<RedisValue>();
        }
    }

    public async Task<bool> SetMoveAsync(string source, string destination, RedisValue value)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in SetMoveAsync");
                return false;
            }

            var result = await db.SetMoveAsync(source, destination, value);
            logger.LogDebug("SetMove: Source={Source}, Destination={Destination}, Value={Value}, Result={Result}",
                source, destination, value, result);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in SetMoveAsync");
            return false;
        }
    }

    public async Task<RedisValue> SetPopAsync(string key)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in SetPopAsync for key: {Key}", key);
                return RedisValue.Null;
            }

            var member = await db.SetPopAsync(key);
            logger.LogDebug("SetPop: Key={Key}, Member={Member}", key, member);
            return member;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in SetPopAsync for key: {Key}", key);
            return RedisValue.Null;
        }
    }

    public async Task<RedisValue[]> SetPopAsync(string key, long count)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in SetPopAsync (bulk) for key: {Key}", key);
                return Array.Empty<RedisValue>();
            }

            var members = await db.SetPopAsync(key, count);
            logger.LogDebug("SetPop (bulk): Key={Key}, Count={Count}, Got={Got}", key, count, members.Length);
            return members;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in SetPopAsync (bulk) for key: {Key}", key);
            return Array.Empty<RedisValue>();
        }
    }

    public async Task<RedisValue[]> SetIntersectAsync(string[] keys)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null || keys.Length == 0)
            {
                logger.LogWarning("Redis connection unavailable in SetIntersectAsync");
                return Array.Empty<RedisValue>();
            }

            // Use SetCombineAndStore with a temporary key, then retrieve members
            var tempKey = "temp:intersect:" + Guid.NewGuid().ToString("N");
            var keyArray = keys.Select(k => (RedisKey)k).ToArray();
            await db.SetCombineAndStoreAsync(SetOperation.Intersect, tempKey, keyArray);
            var members = await db.SetMembersAsync(tempKey);
            await db.KeyDeleteAsync(tempKey);
            
            logger.LogDebug("SetIntersect: Keys={Keys}, Count={Count}", string.Join(", ", keys), members.Length);
            return members;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in SetIntersectAsync");
            return Array.Empty<RedisValue>();
        }
    }

    public async Task<RedisValue[]> SetUnionAsync(string[] keys)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null || keys.Length == 0)
            {
                logger.LogWarning("Redis connection unavailable in SetUnionAsync");
                return Array.Empty<RedisValue>();
            }

            // Use SetCombineAndStore with a temporary key, then retrieve members
            var tempKey = "temp:union:" + Guid.NewGuid().ToString("N");
            var keyArray = keys.Select(k => (RedisKey)k).ToArray();
            await db.SetCombineAndStoreAsync(SetOperation.Union, tempKey, keyArray);
            var members = await db.SetMembersAsync(tempKey);
            await db.KeyDeleteAsync(tempKey);
            
            logger.LogDebug("SetUnion: Keys={Keys}, Count={Count}", string.Join(", ", keys), members.Length);
            return members;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in SetUnionAsync");
            return Array.Empty<RedisValue>();
        }
    }

    public async Task<RedisValue[]> SetDifferenceAsync(string[] keys)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null || keys.Length == 0)
            {
                logger.LogWarning("Redis connection unavailable in SetDifferenceAsync");
                return Array.Empty<RedisValue>();
            }

            // Use SetCombineAndStore with a temporary key, then retrieve members
            var tempKey = "temp:diff:" + Guid.NewGuid().ToString("N");
            var keyArray = keys.Select(k => (RedisKey)k).ToArray();
            await db.SetCombineAndStoreAsync(SetOperation.Difference, tempKey, keyArray);
            var members = await db.SetMembersAsync(tempKey);
            await db.KeyDeleteAsync(tempKey);
            
            logger.LogDebug("SetDifference: Keys={Keys}, Count={Count}", string.Join(", ", keys), members.Length);
            return members;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in SetDifferenceAsync");
            return Array.Empty<RedisValue>();
        }
    }

    public async Task<long> SetIntersectAndStoreAsync(string destination, string[] keys, Aggregate aggregate = Aggregate.Sum)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null || keys.Length == 0)
            {
                logger.LogWarning("Redis connection unavailable in SetIntersectAndStoreAsync");
                return 0;
            }

            var keyArray = keys.Select(k => (RedisKey)k).ToArray();
            var result = await db.SetCombineAndStoreAsync(SetOperation.Intersect, destination, keyArray);
            logger.LogDebug("SetIntersectAndStore: Destination={Destination}, Keys={Keys}, Count={Count}",
                destination, string.Join(", ", keys), result);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in SetIntersectAndStoreAsync");
            return 0;
        }
    }

    public async Task<long> SetUnionAndStoreAsync(string destination, string[] keys, Aggregate aggregate = Aggregate.Sum)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null || keys.Length == 0)
            {
                logger.LogWarning("Redis connection unavailable in SetUnionAndStoreAsync");
                return 0;
            }

            var keyArray = keys.Select(k => (RedisKey)k).ToArray();
            var result = await db.SetCombineAndStoreAsync(SetOperation.Union, destination, keyArray);
            logger.LogDebug("SetUnionAndStore: Destination={Destination}, Keys={Keys}, Count={Count}",
                destination, string.Join(", ", keys), result);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in SetUnionAndStoreAsync");
            return 0;
        }
    }

    public async Task<long> SetDifferenceAndStoreAsync(string destination, string[] keys, Aggregate aggregate = Aggregate.Sum)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null || keys.Length == 0)
            {
                logger.LogWarning("Redis connection unavailable in SetDifferenceAndStoreAsync");
                return 0;
            }

            var keyArray = keys.Select(k => (RedisKey)k).ToArray();
            var result = await db.SetCombineAndStoreAsync(SetOperation.Difference, destination, keyArray);
            logger.LogDebug("SetDifferenceAndStore: Destination={Destination}, Keys={Keys}, Count={Count}",
                destination, string.Join(", ", keys), result);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in SetDifferenceAndStoreAsync");
            return 0;
        }
    }
}
