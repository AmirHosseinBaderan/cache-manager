namespace CacheManager.Abstraction;

/// <summary>
/// Interface for Redis Key Management operations.
/// Provides utilities for managing Redis keys.
/// </summary>
public interface IRedisKeyManager
{
    /// <summary>
    /// Checks if a key exists.
    /// </summary>
    Task<bool> ExistsAsync(string key);

    /// <summary>
    /// Checks if any of the keys exist.
    /// </summary>
    Task<bool> ExistsAsync(IEnumerable<string> keys);

    /// <summary>
    /// Gets the type of a key.
    /// </summary>
    Task<RedisType> KeyTypeAsync(string key);

    /// <summary>
    /// Sets an expiration on a key.
    /// </summary>
    Task<bool> ExpireAsync(string key, TimeSpan expiration);

    /// <summary>
    /// Sets an expiration on a key at a specific time.
    /// </summary>
    Task<bool> ExpireAtAsync(string key, DateTimeOffset expiration);

    /// <summary>
    /// Removes the expiration from a key.
    /// </summary>
    Task<bool> PersistAsync(string key);

    /// <summary>
    /// Gets the time to live for a key.
    /// </summary>
    Task<TimeSpan?> TimeToLiveAsync(string key);

    /// <summary>
    /// Renames a key.
    /// </summary>
    Task<bool> RenameAsync(string key, string newKey);

    /// <summary>
    /// Renames a key if the new key does not exist.
    /// </summary>
    Task<bool> RenameNxAsync(string key, string newKey);

    /// <summary>
    /// Deletes one or more keys.
    /// </summary>
    Task<long> DeleteAsync(IEnumerable<string> keys);

    /// <summary>
    /// Deletes a single key.
    /// </summary>
    Task<bool> DeleteAsync(string key);

    /// <summary>
    /// Returns all keys matching a pattern.
    /// </summary>
    Task<RedisValue[]> KeysAsync(string pattern);

    /// <summary>
    /// Returns all keys matching a pattern with pagination.
    /// </summary>
    Task<RedisValue[]> KeysAsync(string pattern, int pageSize, int pageOffset);

    /// <summary>
    /// Randomly returns a key.
    /// </summary>
    Task<RedisValue> RandomKeyAsync();

    /// <summary>
    /// Appends a value to a string key.
    /// </summary>
    Task<long> AppendAsync(string key, RedisValue value);

    /// <summary>
    /// Gets the length of a string key.
    /// </summary>
    Task<long> StringLengthAsync(string key);
}
