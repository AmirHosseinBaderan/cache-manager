namespace CacheManager.Implementation;

/// <summary>
/// Implementation of Redis Key Management operations.
/// </summary>
internal class RedisKeyManager(ICacheDb cacheDb, ILogger<RedisKeyManager> logger) : IRedisKeyManager
{
    public async Task<bool> ExistsAsync(string key)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in ExistsAsync for key: {Key}", key);
                return false;
            }

            var exists = await db.KeyExistsAsync(key);
            logger.LogDebug("Exists: Key={Key}, Exists={Exists}", key, exists);
            return exists;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in ExistsAsync for key: {Key}", key);
            return false;
        }
    }

    public async Task<bool> ExistsAsync(IEnumerable<string> keys)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in ExistsAsync (multiple keys)");
                return false;
            }

            var keyArray = keys.Select(k => (RedisKey)k).ToArray();
            var result = await db.KeyExistsAsync(keyArray);
            logger.LogDebug("Exists (multiple): Keys={Keys}, Count={Count}", string.Join(", ", keyArray), keyArray.Length);
            return result > 0;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in ExistsAsync (multiple keys)");
            return false;
        }
    }

    public async Task<RedisType> KeyTypeAsync(string key)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in KeyTypeAsync for key: {Key}", key);
                return RedisType.None;
            }

            var type = await db.KeyTypeAsync(key);
            logger.LogDebug("KeyType: Key={Key}, Type={Type}", key, type);
            return type;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in KeyTypeAsync for key: {Key}", key);
            return RedisType.None;
        }
    }

    public async Task<bool> ExpireAsync(string key, TimeSpan expiration)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in ExpireAsync for key: {Key}", key);
                return false;
            }

            var result = await db.KeyExpireAsync(key, expiration);
            logger.LogDebug("Expire: Key={Key}, Expiration={Expiration}, Result={Result}", key, expiration, result);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in ExpireAsync for key: {Key}", key);
            return false;
        }
    }

    public async Task<bool> ExpireAtAsync(string key, DateTimeOffset expiration)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in ExpireAtAsync for key: {Key}", key);
                return false;
            }

            var result = await db.KeyExpireAsync(key, expiration.DateTime);
            logger.LogDebug("ExpireAt: Key={Key}, Expiration={Expiration}, Result={Result}", key, expiration, result);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in ExpireAtAsync for key: {Key}", key);
            return false;
        }
    }

    public async Task<bool> PersistAsync(string key)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in PersistAsync for key: {Key}", key);
                return false;
            }

            var result = await db.KeyPersistAsync(key);
            logger.LogDebug("Persist: Key={Key}, Result={Result}", key, result);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in PersistAsync for key: {Key}", key);
            return false;
        }
    }

    public async Task<TimeSpan?> TimeToLiveAsync(string key)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in TimeToLiveAsync for key: {Key}", key);
                return null;
            }

            var ttl = await db.KeyTimeToLiveAsync(key);
            logger.LogDebug("TimeToLive: Key={Key}, TTL={TTL}", key, ttl);
            return ttl;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in TimeToLiveAsync for key: {Key}", key);
            return null;
        }
    }

    public async Task<bool> RenameAsync(string key, string newKey)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in RenameAsync");
                return false;
            }

            await db.KeyRenameAsync(key, newKey);
            logger.LogDebug("Rename: Key={Key}, NewKey={NewKey}", key, newKey);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in RenameAsync");
            return false;
        }
    }

    public async Task<bool> RenameNxAsync(string key, string newKey)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in RenameNxAsync");
                return false;
            }

            var result = await db.KeyRenameAsync(key, newKey, When.NotExists);
            logger.LogDebug("RenameNx: Key={Key}, NewKey={NewKey}, Result={Result}", key, newKey, result);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in RenameNxAsync");
            return false;
        }
    }

    public async Task<long> DeleteAsync(IEnumerable<string> keys)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in DeleteAsync (multiple keys)");
                return 0;
            }

            var keyArray = keys.Select(k => (RedisKey)k).ToArray();
            var result = await db.KeyDeleteAsync(keyArray);
            logger.LogDebug("Delete (multiple): Keys={Keys}, Count={Count}", string.Join(", ", keyArray), result);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in DeleteAsync (multiple keys)");
            return 0;
        }
    }

    public async Task<bool> DeleteAsync(string key)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in DeleteAsync for key: {Key}", key);
                return false;
            }

            var result = await db.KeyDeleteAsync(key);
            logger.LogDebug("Delete: Key={Key}, Result={Result}", key, result);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in DeleteAsync for key: {Key}", key);
            return false;
        }
    }

    public async Task<RedisValue[]> KeysAsync(string pattern)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in KeysAsync");
                return Array.Empty<RedisValue>();
            }

            var server = db.Multiplexer.GetServer(db.Multiplexer.GetEndPoints()[0]);
            var keys = new List<RedisKey>();
            await foreach (var key in server.KeysAsync(pattern: pattern))
            {
                keys.Add(key);
            }
            logger.LogDebug("Keys: Pattern={Pattern}, Count={Count}", pattern, keys.Count);
            return keys.Select(k => (RedisValue)k.ToString()).ToArray();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in KeysAsync");
            return Array.Empty<RedisValue>();
        }
    }

    public async Task<RedisValue[]> KeysAsync(string pattern, int pageSize, int pageOffset)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in KeysAsync (paginated)");
                return Array.Empty<RedisValue>();
            }

            var server = db.Multiplexer.GetServer(db.Multiplexer.GetEndPoints()[0]);
            var keys = new List<RedisKey>();
            await foreach (var key in server.KeysAsync(pattern: pattern, pageSize: pageSize, pageOffset: pageOffset))
            {
                keys.Add(key);
            }
            logger.LogDebug("Keys (paginated): Pattern={Pattern}, PageSize={PageSize}, PageOffset={PageOffset}, Count={Count}",
                pattern, pageSize, pageOffset, keys.Count);
            return keys.Select(k => (RedisValue)k.ToString()).ToArray();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in KeysAsync (paginated)");
            return Array.Empty<RedisValue>();
        }
    }

    public async Task<RedisValue> RandomKeyAsync()
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in RandomKeyAsync");
                return RedisValue.Null;
            }

            var key = await db.StringGetRandomAsync();
            logger.LogDebug("RandomKey: Key={Key}", key);
            return key;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in RandomKeyAsync");
            return RedisValue.Null;
        }
    }

    public async Task<long> AppendAsync(string key, RedisValue value)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in AppendAsync for key: {Key}", key);
                return 0;
            }

            var result = await db.StringAppendAsync(key, value);
            logger.LogDebug("Append: Key={Key}, Value={Value}, Result={Result}", key, value, result);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in AppendAsync for key: {Key}", key);
            return 0;
        }
    }

    public async Task<long> StringLengthAsync(string key)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in StringLengthAsync for key: {Key}", key);
                return 0;
            }

            var length = await db.StringLengthAsync(key);
            logger.LogDebug("StringLength: Key={Key}, Length={Length}", key, length);
            return length;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in StringLengthAsync for key: {Key}", key);
            return 0;
        }
    }
}
