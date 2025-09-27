using CacheManager.Abstraction;
using CacheManager.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

// 1️⃣ Create the host
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Logging
        services.AddLogging(config =>
        {
            config.AddConsole();
            config.SetMinimumLevel(LogLevel.Information);
        });

        // Redis producer/consumer
        services.AddRedisCacheManager(() => new("127.0.0.1:6379", 1, null, QueueName: "QueueTest"));
        services.AddRedisCacheManagerQueue(typeof(Program).Assembly);
    })
    .Build();

// 2️⃣ Resolve producer and push messages
using var scope = host.Services.CreateScope();
var provider = scope.ServiceProvider;
var producer = provider.GetRequiredService<IProducer>();

Order order = new(Guid.NewGuid(), "amir hossein");
// await producer.PushAsync(order);
await producer.PushAsync(order, TimeSpan.FromTicks(DateTime.Now.AddSeconds(15).Ticks));

// 3️⃣ Run the host to start background services (consumers)
await host.RunAsync();

// ----------------- Consumer -----------------
class OrderConsumer : IRedisConsumer<Order>
{
    public Task ExecuteAsync(Order model, CancellationToken token = default)
    {
        Console.WriteLine("OrderConsumer.ExecuteAsync() Give model {0}", JsonConvert.SerializeObject(model));
        return Task.CompletedTask;
    }
}

public record Order(Guid Id, string Name);