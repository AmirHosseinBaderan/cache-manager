namespace CacheManager.Core;

/// <summary>
/// Provides access to Redis databases through a cache core connection.
/// Supports both direct and Sentinel-based Redis configurations.
/// </summary>
public class CacheDb(ICacheCore core) : ICacheDb
{
    /// <summary>
    /// Disposes the underlying cache core connection.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await core.DisposeAsync();
    }

    /// <summary>
    /// Retrieves a Redis database instance based on the configured cache settings.
    /// Automatically respects Sentinel configuration if enabled.
    /// </summary>
    public async Task<IDatabase?> GetDataBaseAsync(CancellationToken cancellationToken = default)
    {
        // If Sentinel is enabled, CacheCore.ConnectAsync will handle it automatically.
        var connection = await core.ConnectAsync(cancellationToken);
        if (connection is null)
            return null;

        return connection.GetDatabase(Configs.CacheConfigs.Instance);
    }

    /// <summary>
    /// Retrieves a Redis database instance using a custom connection string and instance index.
    /// </summary>
    public async Task<IDatabase?> GetDataBaseAsync(string connectionString, int instance, CancellationToken cancellationToken = default)
    {
        var connection = await core.ConnectAsync(connectionString, cancellationToken);
        if (connection is null)
            return null;

        return connection.GetDatabase(instance);
    }
}