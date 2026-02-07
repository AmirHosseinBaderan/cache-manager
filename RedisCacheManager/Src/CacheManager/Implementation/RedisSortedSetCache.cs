namespace CacheManager.Implementation;

/// <summary>
/// Implementation of Redis Sorted Set operations.
/// </summary>
internal class RedisSortedSetCache(ICacheDb cacheDb, ILogger<RedisSortedSetCache> logger) : IRedisSortedSetCache
{
    public async Task<bool> SortedSetAddAsync(string key, RedisValue member, double score)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in SortedSetAddAsync for key: {Key}", key);
                return false;
            }

            var result = await db.SortedSetAddAsync(key, member, score);
            logger.LogDebug("SortedSetAdd: Key={Key}, Member={Member}, Score={Score}, Result={Result}", key, member, score, result);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in SortedSetAddAsync for key: {Key}", key);
            return false;
        }
    }

    public async Task<long> SortedSetAddBulkAsync(string key, IEnumerable<SortedSetEntry> entries)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in SortedSetAddBulkAsync for key: {Key}", key);
                return 0;
            }

            var entryArray = entries.ToArray();
            var result = await db.SortedSetAddAsync(key, entryArray);
            logger.LogDebug("SortedSetAddBulk: Key={Key}, Count={Count}, Result={Result}", key, entryArray.Length, result);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in SortedSetAddBulkAsync for key: {Key}", key);
            return 0;
        }
    }

    public async Task<double?> SortedSetScoreAsync(string key, RedisValue member)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in SortedSetScoreAsync for key: {Key}", key);
                return null;
            }

            var score = await db.SortedSetScoreAsync(key, member);
            logger.LogDebug("SortedSetScore: Key={Key}, Member={Member}, Score={Score}", key, member, score);
            return score;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in SortedSetScoreAsync for key: {Key}", key);
            return null;
        }
    }

    public async Task<(double? Score, long? Rank)?> SortedSetRankAndScoreAsync(string key, RedisValue member, Order order = Order.Ascending)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in SortedSetRankAndScoreAsync for key: {Key}", key);
                return null;
            }

            var rank = await db.SortedSetRankAsync(key, member, order);
            var score = await db.SortedSetScoreAsync(key, member);
            logger.LogDebug("SortedSetRankAndScore: Key={Key}, Member={Member}, Rank={Rank}, Score={Score}", key, member, rank, score);
            return (score, rank);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in SortedSetRankAndScoreAsync for key: {Key}", key);
            return null;
        }
    }

    public async Task<RedisValue[]> SortedSetRangeByRankAsync(string key, long start = 0, long stop = -1, Order order = Order.Ascending)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in SortedSetRangeByRankAsync for key: {Key}", key);
                return Array.Empty<RedisValue>();
            }

            var members = await db.SortedSetRangeByRankAsync(key, start, stop, order);
            logger.LogDebug("SortedSetRangeByRank: Key={Key}, Start={Start}, Stop={Stop}, Count={Count}", key, start, stop, members.Length);
            return members;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in SortedSetRangeByRankAsync for key: {Key}", key);
            return Array.Empty<RedisValue>();
        }
    }

    public async Task<SortedSetEntry[]> SortedSetRangeByRankWithScoresAsync(string key, long start = 0, long stop = -1, Order order = Order.Ascending)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in SortedSetRangeByRankWithScoresAsync for key: {Key}", key);
                return Array.Empty<SortedSetEntry>();
            }

            var entries = await db.SortedSetRangeByRankWithScoresAsync(key, start, stop, order);
            logger.LogDebug("SortedSetRangeByRankWithScores: Key={Key}, Start={Start}, Stop={Stop}, Count={Count}", key, start, stop, entries.Length);
            return entries;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in SortedSetRangeByRankWithScoresAsync for key: {Key}", key);
            return Array.Empty<SortedSetEntry>();
        }
    }

    public async Task<RedisValue[]> SortedSetRangeByScoreAsync(string key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in SortedSetRangeByScoreAsync for key: {Key}", key);
                return Array.Empty<RedisValue>();
            }

            var members = await db.SortedSetRangeByScoreAsync(key, min, max, exclude, order, skip, take);
            logger.LogDebug("SortedSetRangeByScore: Key={Key}, Min={Min}, Max={Max}, Count={Count}", key, min, max, members.Length);
            return members;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in SortedSetRangeByScoreAsync for key: {Key}", key);
            return Array.Empty<RedisValue>();
        }
    }

    public async Task<SortedSetEntry[]> SortedSetRangeByScoreWithScoresAsync(string key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in SortedSetRangeByScoreWithScoresAsync for key: {Key}", key);
                return Array.Empty<SortedSetEntry>();
            }

            var entries = await db.SortedSetRangeByScoreWithScoresAsync(key, min, max, exclude, order, skip, take);
            logger.LogDebug("SortedSetRangeByScoreWithScores: Key={Key}, Min={Min}, Max={Max}, Count={Count}", key, min, max, entries.Length);
            return entries;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in SortedSetRangeByScoreWithScoresAsync for key: {Key}", key);
            return Array.Empty<SortedSetEntry>();
        }
    }

    public async Task<bool> SortedSetRemoveAsync(string key, RedisValue member)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in SortedSetRemoveAsync for key: {Key}", key);
                return false;
            }

            var result = await db.SortedSetRemoveAsync(key, member);
            logger.LogDebug("SortedSetRemove: Key={Key}, Member={Member}, Result={Result}", key, member, result);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in SortedSetRemoveAsync for key: {Key}", key);
            return false;
        }
    }

    public async Task<long> SortedSetRemoveBulkAsync(string key, IEnumerable<RedisValue> members)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in SortedSetRemoveBulkAsync for key: {Key}", key);
                return 0;
            }

            var memberArray = members.ToArray();
            var result = await db.SortedSetRemoveAsync(key, memberArray);
            logger.LogDebug("SortedSetRemoveBulk: Key={Key}, Count={Count}, Result={Result}", key, memberArray.Length, result);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in SortedSetRemoveBulkAsync for key: {Key}", key);
            return 0;
        }
    }

    public async Task<long> SortedSetRemoveRangeByRankAsync(string key, long start, long stop)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in SortedSetRemoveRangeByRankAsync for key: {Key}", key);
                return 0;
            }

            var result = await db.SortedSetRemoveRangeByRankAsync(key, start, stop);
            logger.LogDebug("SortedSetRemoveRangeByRank: Key={Key}, Start={Start}, Stop={Stop}, Result={Result}", key, start, stop, result);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in SortedSetRemoveRangeByRankAsync for key: {Key}", key);
            return 0;
        }
    }

    public async Task<long> SortedSetRemoveRangeByScoreAsync(string key, double min, double max, Exclude exclude = Exclude.None)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in SortedSetRemoveRangeByScoreAsync for key: {Key}", key);
                return 0;
            }

            var result = await db.SortedSetRemoveRangeByScoreAsync(key, min, max, exclude);
            logger.LogDebug("SortedSetRemoveRangeByScore: Key={Key}, Min={Min}, Max={Max}, Result={Result}", key, min, max, result);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in SortedSetRemoveRangeByScoreAsync for key: {Key}", key);
            return 0;
        }
    }

    public async Task<double> SortedSetIncrementAsync(string key, RedisValue member, double increment)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in SortedSetIncrementAsync for key: {Key}", key);
                return 0;
            }

            var result = await db.SortedSetIncrementAsync(key, member, increment);
            logger.LogDebug("SortedSetIncrement: Key={Key}, Member={Member}, Increment={Increment}, Result={Result}", key, member, increment, result);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in SortedSetIncrementAsync for key: {Key}", key);
            return 0;
        }
    }

    public async Task<double> SortedSetDecrementAsync(string key, RedisValue member, double decrement)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in SortedSetDecrementAsync for key: {Key}", key);
                return 0;
            }

            var result = await db.SortedSetDecrementAsync(key, member, decrement);
            logger.LogDebug("SortedSetDecrement: Key={Key}, Member={Member}, Decrement={Decrement}, Result={Result}", key, member, decrement, result);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in SortedSetDecrementAsync for key: {Key}", key);
            return 0;
        }
    }

    public async Task<long> SortedSetLengthAsync(string key)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in SortedSetLengthAsync for key: {Key}", key);
                return 0;
            }

            var length = await db.SortedSetLengthAsync(key);
            logger.LogDebug("SortedSetLength: Key={Key}, Length={Length}", key, length);
            return length;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in SortedSetLengthAsync for key: {Key}", key);
            return 0;
        }
    }

    public async Task<long> SortedSetCountAsync(string key, double min, double max, Exclude exclude = Exclude.None)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in SortedSetCountAsync for key: {Key}", key);
                return 0;
            }

            var count = await db.SortedSetLengthAsync(key, min, max);
            logger.LogDebug("SortedSetCount: Key={Key}, Min={Min}, Max={Max}, Count={Count}", key, min, max, count);
            return count;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in SortedSetCountAsync for key: {Key}", key);
            return 0;
        }
    }

    public async Task<long?> SortedSetRankAsync(string key, RedisValue member, Order order = Order.Ascending)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in SortedSetRankAsync for key: {Key}", key);
                return null;
            }

            var rank = await db.SortedSetRankAsync(key, member, order);
            logger.LogDebug("SortedSetRank: Key={Key}, Member={Member}, Rank={Rank}", key, member, rank);
            return rank;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in SortedSetRankAsync for key: {Key}", key);
            return null;
        }
    }

    public async Task<long> SortedSetIntersectAndStoreAsync(string destination, string[] keys, Aggregate aggregate = Aggregate.Sum)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in SortedSetIntersectAndStoreAsync");
                return 0;
            }

            var keyArray = keys.Select(k => (RedisKey)k).ToArray();
            var result = await db.SortedSetCombineAndStoreAsync(SetOperation.Intersect, destination, keyArray, aggregate);
            logger.LogDebug("SortedSetIntersectAndStore: Destination={Destination}, Keys={Keys}, Count={Count}",
                destination, string.Join(", ", keys), result);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in SortedSetIntersectAndStoreAsync");
            return 0;
        }
    }

    public async Task<long> SortedSetUnionAndStoreAsync(string destination, string[] keys, Aggregate aggregate = Aggregate.Sum)
    {
        try
        {
            var db = await cacheDb.GetDataBaseAsync();
            if (db is null)
            {
                logger.LogWarning("Redis connection unavailable in SortedSetUnionAndStoreAsync");
                return 0;
            }

            var keyArray = keys.Select(k => (RedisKey)k).ToArray();
            var result = await db.SortedSetCombineAndStoreAsync(SetOperation.Union, destination, keyArray, aggregate);
            logger.LogDebug("SortedSetUnionAndStore: Destination={Destination}, Keys={Keys}, Count={Count}",
                destination, string.Join(", ", keys), result);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in SortedSetUnionAndStoreAsync");
            return 0;
        }
    }

    Task<long> IRedisSortedSetCache.SortedSetLengthByValueAsync(string key, double min, double max, Exclude exclude)
    {
        // Using SortedSetLengthAsync with score range as alternative
        return SortedSetCountAsync(key, min, max, exclude);
    }
}
