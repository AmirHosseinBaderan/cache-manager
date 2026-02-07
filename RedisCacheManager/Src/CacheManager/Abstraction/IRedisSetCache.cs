namespace CacheManager.Abstraction;

/// <summary>
/// Interface for Redis Set operations.
/// Sets are unordered collections of unique Redis strings.
/// </summary>
public interface IRedisSetCache
{
    /// <summary>
    /// Adds one or more members to a set.
    /// </summary>
    Task<long> SetAddAsync(string key, RedisValue value);

    /// <summary>
    /// Adds multiple members to a set.
    /// </summary>
    Task<long> SetAddBulkAsync(string key, IEnumerable<RedisValue> values);

    /// <summary>
    /// Removes one or more members from a set.
    /// </summary>
    Task<long> SetRemoveAsync(string key, IEnumerable<RedisValue> values);

    /// <summary>
    /// Gets all members of a set.
    /// </summary>
    Task<RedisValue[]> SetMembersAsync(string key);

    /// <summary>
    /// Checks if a value is a member of a set.
    /// </summary>
    Task<bool> SetContainsAsync(string key, RedisValue value);

    /// <summary>
    /// Gets the number of members in a set.
    /// </summary>
    Task<long> SetLengthAsync(string key);

    /// <summary>
    /// Gets a random member from a set.
    /// </summary>
    Task<RedisValue> SetRandomMemberAsync(string key);

    /// <summary>
    /// Gets multiple random members from a set.
    /// </summary>
    Task<RedisValue[]> SetRandomMembersAsync(string key, long count);

    /// <summary>
    /// Moves a member from one set to another.
    /// </summary>
    Task<bool> SetMoveAsync(string source, string destination, RedisValue value);

    /// <summary>
    /// Removes and returns a random member from a set.
    /// </summary>
    Task<RedisValue> SetPopAsync(string key);

    /// <summary>
    /// Removes and returns multiple random members from a set.
    /// </summary>
    Task<RedisValue[]> SetPopAsync(string key, long count);

    /// <summary>
    /// Returns the intersection of multiple sets.
    /// </summary>
    Task<RedisValue[]> SetIntersectAsync(string[] keys);

    /// <summary>
    /// Returns the union of multiple sets.
    /// </summary>
    Task<RedisValue[]> SetUnionAsync(string[] keys);

    /// <summary>
    /// Returns the difference between the first set and other sets.
    /// </summary>
    Task<RedisValue[]> SetDifferenceAsync(string[] keys);

    /// <summary>
    /// Stores the intersection of multiple sets in a destination set.
    /// </summary>
    Task<long> SetIntersectAndStoreAsync(string destination, string[] keys, Aggregate aggregate = Aggregate.Sum);

    /// <summary>
    /// Stores the union of multiple sets in a destination set.
    /// </summary>
    Task<long> SetUnionAndStoreAsync(string destination, string[] keys, Aggregate aggregate = Aggregate.Sum);

    /// <summary>
    /// Stores the difference between the first set and other sets in a destination set.
    /// </summary>
    Task<long> SetDifferenceAndStoreAsync(string destination, string[] keys, Aggregate aggregate = Aggregate.Sum);
}
