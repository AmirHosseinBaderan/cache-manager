using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CacheManager.Core;

internal class RedisDispatcher(ICacheDb db, IJsonCache cache, IServiceProvider sp, ILogger<RedisDispatcher> logger)
{
    public async Task DispatchAsync(MessageEnvelope message, CancellationToken ct)
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

    public async Task DispatchAsync(string key, CancellationToken ct)
    {
        var value = await cache.GetItemAsync<MessageEnvelope>(key);
        if (value is null)
            return;
        await DispatchAsync(value, ct);
    }
}