using CacheManager.Configuration;
using Microsoft.Extensions.Logging;

namespace CacheManager.Core;

public class CacheCore(ILogger<CacheCore> logger) : ICacheCore
{
    private ConnectionMultiplexer? _connection;

    public async Task<ConnectionMultiplexer?> ConnectAsync(CancellationToken cancellationToken = default)
        => await ConnectAsync(Configs.CacheConfigs.ConnectionString, cancellationToken);

    public async Task<ConnectionMultiplexer?> ConnectAsync(string connectionString,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (_connection is not null and { IsConnected: true })
                return _connection;

            _connection = await ConnectionMultiplexer.ConnectAsync(connectionString);
            logger.LogInformation("Success connection with redis");
            return _connection;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception in redis connection");
            throw;
        }
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        if (_connection is not null)
            await _connection.DisposeAsync();
    }
}