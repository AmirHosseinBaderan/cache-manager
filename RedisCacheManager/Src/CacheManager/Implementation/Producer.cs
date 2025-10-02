using Microsoft.Extensions.Logging;

namespace CacheManager.Implementation;

internal class Producer(
    IJsonCache cache,
    ITimerDispatcher timerDispatcher,
    RedisDispatcher dispatcher,
    ILogger<Producer> logger) : IProducer
{
    public async Task PushAsync<T>(T model, TimeSpan delay, string? key = null, bool repeat = false,
        CancellationToken token = default)
    {
        try
        {
            logger.LogInformation("Push producer in redis");

            MessageEnvelope message = new()
            {
                Type = typeof(T).AssemblyQualifiedName!,
                Data = JsonConvert.SerializeObject(model),
            };

            var queueKey = $"{Configs.CacheConfigs.QueueName}-{key ?? Guid.NewGuid().ToString("N")}";
            await cache.SetItemAsync(queueKey, message, repeat ? null : delay.Add(TimeSpan.FromMinutes(1)));

            timerDispatcher.Run(queueKey, delay,
                async (key) => await dispatcher.DispatchAsync(key, token),
                repeat,
                async (key) => await cache.RemoveItemAsync(key));
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception in PushAsync");
        }
    }

    public void Cancel(string key)
    {
        var queueKey = $"{Configs.CacheConfigs.QueueName}-{key}";
        timerDispatcher.Cancel(queueKey);
    }
}