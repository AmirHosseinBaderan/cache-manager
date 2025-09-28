using Microsoft.Extensions.Logging;

namespace CacheManager.Implementation;

internal class Producer(
    IJsonCache cache,
    ITimerDispatcher timerDispatcher,
    RedisDispatcher dispatcher,
    ILogger<Producer> logger) : IProducer
{
    public async Task PushAsync<T>(T model, TimeSpan delay, CancellationToken token = default)
    {
        try
        {
            logger.LogInformation("Push producer in redis");

            MessageEnvelope message = new()
            {
                Type = typeof(T).AssemblyQualifiedName!,
                Data = JsonConvert.SerializeObject(model),
            };

            var queueKey = $"{Configs.CacheConfigs.QueueName}-{Guid.NewGuid():N}";
            await cache.SetItemAsync(queueKey, message);

            timerDispatcher.Run(queueKey, delay,
                async (key) => await dispatcher.DispatchAsync(key, token));
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception in PushAsync");
        }
    }
}