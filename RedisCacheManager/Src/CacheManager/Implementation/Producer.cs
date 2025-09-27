using Microsoft.Extensions.Logging;

namespace CacheManager.Implementation;

internal class Producer(IJsonCache cache, ILogger<Producer> logger) : IProducer
{
    public async Task PushAsync<T>(T model, TimeSpan? delay = null, CancellationToken token = default)
    {
        try
        {
            logger.LogInformation("Push producer in redis");

            MessageEnvelope message = new()
            {
                Type = typeof(T).AssemblyQualifiedName!,
                Data = JsonConvert.SerializeObject(model),
                TimeSpan = delay ?? TimeSpan.Zero,
            };

            var queueKey = $"{Configs.CacheConfigs.QueueName}-{Guid.NewGuid():N}";
            await cache.SetItemAsync(queueKey, message);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception in PushAsync");
        }
    }
}