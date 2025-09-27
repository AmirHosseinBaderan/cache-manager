using CacheManager.Configuration;
using StackExchange.Redis;

namespace CacheManager.Core;

public class CacheDb(ICacheCore core) : ICacheDb
{
    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await core.DisposeAsync();
    }

    public async Task<IDatabase?> GetDataBaseAsync(CancellationToken cancellationToken = default)
        => await GetDataBaseAsync(Configs.CacheConfigs.ConnectionString, Configs.CacheConfigs.Instance,
            cancellationToken);

    public async Task<IDatabase?> GetDataBaseAsync(string connectionString, int instance,
        CancellationToken cancellationToken = default)
    {
        var connection = await core.ConnectAsync(connectionString, cancellationToken);
        if (connection is null)
            return null;

        return connection.GetDatabase(instance);
    }
}