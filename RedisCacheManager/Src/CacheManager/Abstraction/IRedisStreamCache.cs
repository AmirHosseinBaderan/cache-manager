using CacheManager.Core;

namespace CacheManager.Abstraction;

public interface IRedisStreamCache
{
    /// <summary>
    /// Add a message to a stream
    /// </summary>
    Task<string> StreamAddAsync(string key, IDictionary<string, string> fieldValues, string? messageId = null);

    /// <summary>
    /// Read messages from a stream
    /// </summary>
    Task<Dictionary<string, Dictionary<string, string>>[]> StreamReadAsync(string key, string startId = "0", long count = 10);

    /// <summary>
    /// Read messages from multiple streams
    /// </summary>
    Task<Dictionary<string, Dictionary<string, string>>[]> StreamReadAsync(string[] keys, (string key, string id)[] streamIds, long count = 10);

    /// <summary>
    /// Delete messages from a stream
    /// </summary>
    Task<long> StreamDeleteAsync(string key, string[] messageIds);

    /// <summary>
    /// Get the length of a stream
    /// </summary>
    Task<long> StreamLengthAsync(string key);

    /// <summary>
    /// Create a consumer group
    /// </summary>
    Task StreamConsumerGroupCreateAsync(string key, string groupName, string? startId = null);

    /// <summary>
    /// Read from a consumer group
    /// </summary>
    Task<Dictionary<string, Dictionary<string, string>>[]> StreamReadGroupAsync(string key, string groupName, string consumerName, string startId = ">", long count = 10);

    /// <summary>
    /// Acknowledge messages as processed
    /// </summary>
    Task<long> StreamAcknowledgeAsync(string key, string groupName, string[] messageIds);

    /// <summary>
    /// Get pending messages for a consumer group
    /// </summary>
    Task<StreamPendingInfo> StreamPendingAsync(string key, string groupName, string startId = "+", string endId = "-", int count = 10);

    /// <summary>
    /// Claim messages for a consumer group
    /// </summary>
    Task<string[]> StreamClaimAsync(string key, string groupName, string consumerName, long minIdleTimeMs, string[] messageIds);
}
