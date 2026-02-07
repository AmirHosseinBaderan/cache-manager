namespace CacheManager.Abstraction;

/// <summary>
/// Interface for Redis Sorted Set operations.
/// Sorted sets are ordered collections of unique strings with associated scores.
/// </summary>
public interface IRedisSortedSetCache
{
    /// <summary>
    /// Adds a member with a score to a sorted set.
    /// </summary>
    Task<bool> SortedSetAddAsync(string key, RedisValue member, double score);

    /// <summary>
    /// Adds multiple members with their scores to a sorted set.
    /// </summary>
    Task<long> SortedSetAddBulkAsync(string key, IEnumerable<SortedSetEntry> entries);

    /// <summary>
    /// Gets the score of a member in a sorted set.
    /// </summary>
    Task<double?> SortedSetScoreAsync(string key, RedisValue member);

    /// <summary>
    /// Gets the score and rank of a member in a sorted set.
    /// </summary>
    Task<(double? Score, long? Rank)?> SortedSetRankAndScoreAsync(string key, RedisValue member, Order order = Order.Ascending);

    /// <summary>
    /// Gets a range of members from a sorted set by index.
    /// </summary>
    Task<RedisValue[]> SortedSetRangeByRankAsync(string key, long start = 0, long stop = -1, Order order = Order.Ascending);

    /// <summary>
    /// Gets a range of members with scores from a sorted set by index.
    /// </summary>
    Task<SortedSetEntry[]> SortedSetRangeByRankWithScoresAsync(string key, long start = 0, long stop = -1, Order order = Order.Ascending);

    /// <summary>
    /// Gets a range of members from a sorted set by score.
    /// </summary>
    Task<RedisValue[]> SortedSetRangeByScoreAsync(string key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1);

    /// <summary>
    /// Gets a range of members with scores from a sorted set by score.
    /// </summary>
    Task<SortedSetEntry[]> SortedSetRangeByScoreWithScoresAsync(string key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1);

    /// <summary>
    /// Removes a member from a sorted set.
    /// </summary>
    Task<bool> SortedSetRemoveAsync(string key, RedisValue member);

    /// <summary>
    /// Removes multiple members from a sorted set.
    /// </summary>
    Task<long> SortedSetRemoveBulkAsync(string key, IEnumerable<RedisValue> members);

    /// <summary>
    /// Removes a range of members from a sorted set by rank.
    /// </summary>
    Task<long> SortedSetRemoveRangeByRankAsync(string key, long start, long stop);

    /// <summary>
    /// Removes a range of members from a sorted set by score.
    /// </summary>
    Task<long> SortedSetRemoveRangeByScoreAsync(string key, double min, double max, Exclude exclude = Exclude.None);

    /// <summary>
    /// Increments the score of a member in a sorted set.
    /// </summary>
    Task<double> SortedSetIncrementAsync(string key, RedisValue member, double increment);

    /// <summary>
    /// Decrements the score of a member in a sorted set.
    /// </summary>
    Task<double> SortedSetDecrementAsync(string key, RedisValue member, double decrement);

    /// <summary>
    /// Gets the number of members in a sorted set.
    /// </summary>
    Task<long> SortedSetLengthAsync(string key);

    /// <summary>
    /// Gets the number of members in a sorted set within a score range.
    /// </summary>
    Task<long> SortedSetLengthByValueAsync(string key, double min, double max, Exclude exclude = Exclude.None);

    /// <summary>
    /// Counts the number of members in a sorted set within a score range.
    /// </summary>
    Task<long> SortedSetCountAsync(string key, double min, double max, Exclude exclude = Exclude.None);

    /// <summary>
    /// Returns the rank of a member in a sorted set.
    /// </summary>
    Task<long?> SortedSetRankAsync(string key, RedisValue member, Order order = Order.Ascending);

    /// <summary>
    /// Intersects multiple sorted sets and stores the result.
    /// </summary>
    Task<long> SortedSetIntersectAndStoreAsync(string destination, string[] keys, Aggregate aggregate = Aggregate.Sum);

    /// <summary>
    /// Unions multiple sorted sets and stores the result.
    /// </summary>
    Task<long> SortedSetUnionAndStoreAsync(string destination, string[] keys, Aggregate aggregate = Aggregate.Sum);
}
