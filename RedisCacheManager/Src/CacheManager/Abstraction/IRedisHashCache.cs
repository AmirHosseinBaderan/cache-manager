namespace CacheManager.Abstraction;

/// <summary>
/// Interface for Redis Hash operations.
/// Hashes are ideal for storing objects with multiple fields.
/// </summary>
public interface IRedisHashCache
{
    /// <summary>
    /// Sets the value of a field in a hash.
    /// </summary>
    Task<bool> HashSetAsync(string key, string field, RedisValue value);

    /// <summary>
    /// Sets multiple field-value pairs in a hash.
    /// </summary>
    Task HashSetBulkAsync(string key, IEnumerable<HashEntry> entries);

    /// <summary>
    /// Gets the value of a field in a hash.
    /// </summary>
    Task<RedisValue> HashGetAsync(string key, string field);

    /// <summary>
    /// Gets the values of multiple fields from a hash.
    /// </summary>
    Task<RedisValue[]> HashGetBulkAsync(string key, IEnumerable<string> fields);

    /// <summary>
    /// Gets all fields and values from a hash.
    /// </summary>
    Task<HashEntry[]> HashGetAllAsync(string key);

    /// <summary>
    /// Deletes one or more fields from a hash.
    /// </summary>
    Task<long> HashDeleteAsync(string key, IEnumerable<string> fields);

    /// <summary>
    /// Checks if a field exists in a hash.
    /// </summary>
    Task<bool> HashExistsAsync(string key, string field);

    /// <summary>
    /// Increments the value of a field in a hash by the given amount.
    /// </summary>
    Task<double> HashIncrementAsync(string key, string field, double increment);

    /// <summary>
    /// Increments the value of a field in a hash by 1.
    /// </summary>
    Task<long> HashIncrementAsync(string key, string field, long increment = 1);

    /// <summary>
    /// Decrements the value of a field in a hash by the given amount.
    /// </summary>
    Task<double> HashDecrementAsync(string key, string field, double decrement);

    /// <summary>
    /// Decrements the value of a field in a hash by 1.
    /// </summary>
    Task<long> HashDecrementAsync(string key, string field, long decrement = 1);

    /// <summary>
    /// Gets all fields in a hash.
    /// </summary>
    Task<RedisValue[]> HashKeysAsync(string key);

    /// <summary>
    /// Gets all values in a hash.
    /// </summary>
    Task<RedisValue[]> HashValuesAsync(string key);

    /// <summary>
    /// Gets the number of fields in a hash.
    /// </summary>
    Task<long> HashLengthAsync(string key);
}
