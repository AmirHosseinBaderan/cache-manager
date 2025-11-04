namespace CacheManager.Configuration;

/// <summary>
/// Defines configuration options for the cache system, including
/// Redis connection details, serialization preferences, and optional Sentinel support.
/// </summary>
public class CacheConfigs
{
    /// <summary>
    /// The standard Redis connection string (e.g. "localhost:6379").
    /// Used when <see cref="UseSentinel"/> is set to <c>false</c>.
    /// </summary>
    public string ConnectionString { get; set; } = "localhost:6379";

    /// <summary>
    /// The Redis database index (0–15 by default) to connect to.
    /// </summary>
    public int Instance { get; set; } = 0;

    /// <summary>
    /// Optional custom JSON serializer settings for cache serialization and deserialization.
    /// </summary>
    public JsonSerializerSettings? JsonSerializerSettings { get; set; }

    /// <summary>
    /// Controls the JSON formatting style for serialization (None or Indented).
    /// </summary>
    public Formatting Formatting { get; set; } = Formatting.None;

    /// <summary>
    /// Optional queue name used for cache-based messaging or queue processing.
    /// </summary>
    public string? QueueName { get; set; }

    /// <summary>
    /// Indicates whether Redis Sentinel should be used for high availability and automatic failover.
    /// When true, <see cref="Sentinels"/> and <see cref="ServiceName"/> must be configured.
    /// </summary>
    public bool UseSentinel { get; set; } = false;

    /// <summary>
    /// A list of Redis Sentinel endpoints (e.g. ["sentinel1:26379", "sentinel2:26379"]).
    /// Required if <see cref="UseSentinel"/> is <c>true</c>.
    /// </summary>
    public string[]? Sentinels { get; set; }

    /// <summary>
    /// The logical name of the Redis master monitored by Sentinel (e.g. "mymaster").
    /// Required if <see cref="UseSentinel"/> is <c>true</c>.
    /// </summary>
    public string? ServiceName { get; set; }
}

public class Configs
{
    public static CacheConfigs CacheConfigs { get; set; } = null!;
}