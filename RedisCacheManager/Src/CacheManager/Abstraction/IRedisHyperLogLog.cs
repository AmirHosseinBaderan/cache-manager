namespace CacheManager.Abstraction;

/// <summary>
///     Interface for Redis HyperLogLog operations
/// </summary>
public interface IRedisHyperLogLog
{
    /// <summary>
    ///     Adds elements to a HyperLogLog key
    /// </summary>
    /// <param name="key">The key of the HyperLogLog</param>
    /// <param name="values">The values to add</param>
    /// <returns>True if at least one element was added to a new key, False otherwise</returns>
    Task<bool> PfAddAsync(string key, params string[] values);

    /// <summary>
    ///     Returns the approximate cardinality of the HyperLogLog
    /// </summary>
    /// <param name="keys">The keys of the HyperLogLogs</param>
    /// <returns>The approximate count of elements</returns>
    Task<long> PfCountAsync(params string[] keys);

    /// <summary>
    ///     Merges multiple HyperLogLogs into one
    /// </summary>
    /// <param name="destination">The destination key</param>
    /// <param name="sourceKeys">The source keys</param>
    /// <returns>True if successful</returns>
    Task<bool> PfMergeAsync(string destination, params string[] sourceKeys);
}
