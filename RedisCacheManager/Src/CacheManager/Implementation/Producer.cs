using Microsoft.Extensions.Logging;

namespace CacheManager.Implementation;

internal class Producer(ICacheDb db, ILogger<Producer> logger) : IProducer
{
    public async Task PushAsync<T>(T model, TimeSpan? delay = null, CancellationToken token = default)
    {
        try
        {
            var database = await db.GetDataBaseAsync(token);
            if (database is null)
            {
                logger.LogWarning("Can't connect to redis data base :(");
                return;
            }

            logger.LogInformation("Push producer in redis");

            MessageEnvelope message = new()
            {
                Type = typeof(T).AssemblyQualifiedName!,
                Data = JsonConvert.SerializeObject(model),
            };

            var payload = JsonConvert.SerializeObject(message);

            if (delay.HasValue)
            {
                var deliverAt = DateTimeOffset.UtcNow.Add(delay.Value).ToUnixTimeSeconds();
                await database.SortedSetAddAsync(Configs.CacheConfigs.QueueName, payload, deliverAt);
            }
            else
                await database.ListLeftPushAsync(Configs.CacheConfigs.QueueName, payload);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception in PushAsync");
        }
    }
}