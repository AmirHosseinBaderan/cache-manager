using CacheManager.Abstraction;
using CacheManager.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Timer = System.Timers.Timer;

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

Order order = new(Guid.NewGuid(), "Order with repeat", 1);

await producer.PushAsync(order, TimeSpan.FromSeconds(5), repeat: true);
// Task.Run(async () =>
// {
//     for (int i = 0; i < 20; i++)
//     {
//         Order order = new(Guid.NewGuid(), "amir hossein", i);
//         await producer.PushAsync(order, TimeSpan.FromSeconds(10));
//     }
// });
// // 3️⃣ Run the host to start background services (consumers)

// for (int i = 1; i < 2000; i++)
// {
//     int copy = i; // capture the current value
//     Timer timer = new(copy * 100);
//     timer.AutoReset = false;
//     timer.Elapsed += (sender, e) =>
//     {
//         Console.WriteLine($"Timer Elapsed {copy}");
//         timer.Dispose(); // clean up
//     };
//     timer.Start();
// }

await host.RunAsync();

public class Consumed
{
    public static int Count = 0;
}

// ----------------- Consumer -----------------
class OrderConsumer : IRedisConsumer<Order>
{
    public Task ExecuteAsync(Order model, CancellationToken token = default)
    {
        Consumed.Count++;
        Console.WriteLine("OrderConsumer.ExecuteAsync() Give model {0} Consumed Count : {1}",
            JsonConvert.SerializeObject(model), Consumed.Count);
        return Task.CompletedTask;
    }
}

public record Order(Guid Id, string Name, int Index);