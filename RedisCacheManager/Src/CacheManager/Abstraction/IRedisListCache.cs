namespace CacheManager.Abstraction;

/// <summary>
/// Interface for Redis List operations.
/// Lists are ordered collections of Redis strings.
/// </summary>
public interface IRedisListCache
{
    /// <summary>
    /// Inserts an element at the beginning of a list.
    /// </summary>
    Task<long> ListLeftPushAsync(string key, RedisValue value);

    /// <summary>
    /// Inserts multiple elements at the beginning of a list.
    /// </summary>
    Task<long> ListLeftPushBulkAsync(string key, IEnumerable<RedisValue> values);

    /// <summary>
    /// Inserts an element at the end of a list.
    /// </summary>
    Task<long> ListRightPushAsync(string key, RedisValue value);

    /// <summary>
    /// Inserts multiple elements at the end of a list.
    /// </summary>
    Task<long> ListRightPushBulkAsync(string key, IEnumerable<RedisValue> values);

    /// <summary>
    /// Removes and returns the first element of a list.
    /// </summary>
    Task<RedisValue> ListLeftPopAsync(string key);

    /// <summary>
    /// Removes and returns the last element of a list.
    /// </summary>
    Task<RedisValue> ListRightPopAsync(string key);

    /// <summary>
    /// Gets elements from a list by index range.
    /// </summary>
    Task<RedisValue[]> ListRangeAsync(string key, long start, long stop);

    /// <summary>
    /// Gets the length of a list.
    /// </summary>
    Task<long> ListLengthAsync(string key);

    /// <summary>
    /// Gets the element at an index in a list.
    /// </summary>
    Task<RedisValue> ListGetByIndexAsync(string key, long index);

    /// <summary>
    /// Sets the value of an element in a list by its index.
    /// </summary>
    Task<bool> ListSetByIndexAsync(string key, long index, RedisValue value);

    /// <summary>
    /// Inserts an element before or after a pivot element in a list.
    /// </summary>
    Task<long> ListInsertAsync(string key, RedisValue pivot, RedisValue value, bool after = false);

    /// <summary>
    /// Removes elements from a list.
    /// </summary>
    Task<long> ListRemoveAsync(string key, RedisValue value, long count = 0);

    /// <summary>
    /// Blocks until an element is available to pop from the left.
    /// </summary>
    Task<RedisValue?> ListLeftPopAsync(string key, TimeSpan timeout);

    /// <summary>
    /// Blocks until elements are available to pop from the left of multiple lists.
    /// </summary>
    Task<KeyValuePair<RedisChannel, RedisValue>?> ListLeftPopAsync(string[] keys, TimeSpan timeout);
}
