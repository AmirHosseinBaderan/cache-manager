using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CacheManager.Core;

internal class RedisDispatcher(ICacheDb db, IServiceProvider sp, ILogger<RedisDispatcher> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var database = await db.GetDataBaseAsync(stoppingToken);
            if (database is null)
            {
                logger.LogWarning("Can't connect to redis data base :(");
                return;
            }

            var raw = await database.ListRightPopAsync(Configs.CacheConfigs.QueueName);
            if (raw.IsNullOrEmpty)
            {
                var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                var due = await database.SortedSetRangeByScoreAsync(Configs.CacheConfigs.QueueName, stop: now, take: 1);

                if (due.Any())
                {
                    raw = due.First();
                    await database.SortedSetRemoveAsync(Configs.CacheConfigs.QueueName, raw);
                }
            }

            if (!raw.IsNullOrEmpty)
            {
                await DispatchAsync(raw!, stoppingToken);
            }
            else
            {
                await Task.Delay(500, stoppingToken);
            }
        }
    }

    private async Task DispatchAsync(string rawMessage, CancellationToken ct)
    {
        var envelope = JsonConvert.DeserializeObject<MessageEnvelope>(rawMessage);
        if (envelope == null) return;

        var messageType = Type.GetType(envelope.Type);
        if (messageType == null) return;

        var data = JsonConvert.DeserializeObject(envelope.Data, messageType);

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