using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CacheManager.Core;

internal class RedisDispatcher(ICacheDb db, IJsonCache cache, IServiceProvider sp, ILogger<RedisDispatcher> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var database = await db.GetDataBaseAsync(stoppingToken);
            if (database is null)
            {
                logger.LogWarning("Can't connect to redis database :(");
                return;
            }

            var keys = await GetKeysAsync(database);
            var values = await database.StringGetAsync(keys);

            var filters = keys
                .Zip(values, (key, value) => new { Key = key, Value = value })
                .Where(kv => !kv.Value.IsNullOrEmpty)
                .Select(kv =>
                {
                    var json = kv.Value.ToString();
                    TimeSpan ticks = TimeSpan.Zero;

                    // Look for '"TimeSpan":12345678'
                    var match = System.Text.RegularExpressions.Regex.Match(
                        json,
                        @"""TimeSpan""\s*:\s*""([^""]+)"""
                    );
                    if (match.Success)
                    {
                        var timeSpanString = match.Groups[1].Value;

                        // Parse to TimeSpan
                        _ = TimeSpan.TryParse(timeSpanString, out ticks);
                    }

                    // Only ready messages
                    if (ticks == TimeSpan.Zero || ticks <= TimeSpan.FromTicks(DateTimeOffset.Now.Ticks))
                    {
                        var message = JsonConvert.DeserializeObject<MessageEnvelope>(json);
                        return new { kv.Key, Message = message };
                    }

                    return null;
                })
                .Where(x => x != null)
                .ToList();

            foreach (var value in filters)
            {
                await DispatchAsync(value.Message, stoppingToken);
                await cache.RemoveItemAsync(value.Key);
            }

            await Task.Delay(500, stoppingToken);
        }
    }

    async Task<RedisKey[]> GetKeysAsync(IDatabase database)
    {
        // Get all endpoints from the multiplexer
        var endpoints = database.Multiplexer.GetEndPoints();

// Collect all keys from all servers
        var allKeys = new List<RedisKey>();
        foreach (var endpoint in endpoints)
        {
            var server = database.Multiplexer.GetServer(endpoint);

            // Skip if the server is a replica (optional, only scan masters)
            if (server.IsReplica) continue;

            // Enumerate keys with the desired pattern
            allKeys.AddRange(server.Keys(Configs.CacheConfigs.Instance, pattern: $"{Configs.CacheConfigs.QueueName}*"));
        }

// Convert to array if needed
        return allKeys.ToArray();
    }


    private async Task DispatchAsync(MessageEnvelope message, CancellationToken ct)
    {
        var messageType = Type.GetType(message.Type);
        if (messageType == null) return;

        var data = JsonConvert.DeserializeObject(message.Data, messageType);

        using var scope = sp.CreateScope();
        var consumerType = typeof(IRedisConsumer<>).MakeGenericType(messageType);
        var consumers = scope.ServiceProvider.GetServices(consumerType);

        foreach (var consumer in consumers)
        {
            var method = consumerType.GetMethod("ExecuteAsync")!;
            await (Task)method.Invoke(consumer, new[] { data!, ct })!;
        }
    }
}