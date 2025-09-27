using StackExchange.Redis;

namespace CacheManager.Core;

public interface ICacheDb : IAsyncDisposable
{
    /// <summary>
    /// Create connection and open data base using default configuration
    /// </summary>
    /// <returns> Instance of <see cref="IDatabase"/></returns>
    Task<IDatabase?> GetDataBaseAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Create connection and open data base using connection string 
    /// </summary>
    /// <param name="connectionString">Connection string for connect to redis</param>
    /// <returns>Instance of <see cref="IDatabaseAsync"/></returns>
    Task<IDatabase?> GetDataBaseAsync(string connectionString, int instance,
        CancellationToken cancellationToken = default);
}