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
            if (_connection is { IsConnected: true })
                return _connection;

            if (Configs.CacheConfigs.UseSentinel)
            {
                _connection = await ConnectViaSentinelAsync(
                    Configs.CacheConfigs.Sentinels!,
                    Configs.CacheConfigs.ServiceName!);
            }
            else
            {
                _connection = await ConnectionMultiplexer.ConnectAsync(connectionString);
            }

            logger.LogInformation("✅ Redis connection established ({Mode})",
                Configs.CacheConfigs.UseSentinel ? "Sentinel" : "Direct");

            return _connection;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ Exception during Redis connection");
            throw;
        }
    }

    private async Task<ConnectionMultiplexer> ConnectViaSentinelAsync(string[] sentinels, string serviceName)
    {
        if (sentinels == null || sentinels.Length == 0)
            throw new ArgumentException("No Sentinel endpoints configured.");

        EndPoint? masterEndpoint = null;
        ConnectionMultiplexer? sentinelConnection = null;

        // Try each sentinel until we find the master
        foreach (var sentinel in sentinels)
        {
            try
            {
                sentinelConnection = await ConnectionMultiplexer.ConnectAsync(sentinel);
                var server = sentinelConnection.GetServer(sentinel);

                var masterInfo = await server.SentinelGetMasterAddressByNameAsync(serviceName);
                if (masterInfo != null)
                {
                    masterEndpoint = masterInfo;
                    break;
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "⚠️ Failed to query sentinel at {Sentinel}", sentinel);
            }
        }

        if (masterEndpoint == null)
            throw new Exception($"❌ Sentinel could not find master for service '{serviceName}'");

        logger.LogInformation("✅ Redis master resolved by Sentinel: {Master}", masterEndpoint);

        var redisOptions = new ConfigurationOptions
        {
            AbortOnConnectFail = false
        };
        redisOptions.EndPoints.Add(masterEndpoint);

        var connection = await ConnectionMultiplexer.ConnectAsync(redisOptions);

        // Monitor Sentinel failover
        _ = Task.Run(async () =>
        {
            while (true)
            {
                foreach (var sentinel in sentinels)
                {
                    try
                    {
                        var server = sentinelConnection?.GetServer(sentinel);
                        var currentMaster = await server?.SentinelGetMasterAddressByNameAsync(serviceName)!;

                        if (currentMaster != null && !currentMaster.Equals(masterEndpoint))
                        {
                            logger.LogWarning("⚠️ Redis master switched: {Old} -> {New}", masterEndpoint,
                                currentMaster);
                            masterEndpoint = currentMaster;
                            await connection.CloseAsync();
                            _connection = await ConnectionMultiplexer.ConnectAsync(currentMaster.ToString() ?? "");
                        }
                    }
                    catch
                    {
                        /* ignore transient errors */
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        });

        return connection;
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        if (_connection is not null)
            await _connection.DisposeAsync();
    }
}