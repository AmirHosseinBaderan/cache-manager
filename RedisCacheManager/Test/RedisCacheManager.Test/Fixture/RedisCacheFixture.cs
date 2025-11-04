using CacheManager.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace CacheManager.Tests.Fixtures;

public class RedisCacheFixture : IAsyncLifetime
{
    public IServiceProvider ServiceProvider { get; private set; } = default!;
    public ServiceCollection Services { get; private set; } = default!;
    public TestModel Model { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        // Setup model
        Model = new TestModel("1", "Amir", "Baderan");

        // Configure services
        Services = new ServiceCollection();
        Services.AddRedisCacheManager(() => new CacheConfigs
        {
            ConnectionString = "127.0.0.1:6379",
            QueueName = "Test-Queue",
            Instance = 0,
            Formatting = Formatting.None,
        });

        ServiceProvider = Services.BuildServiceProvider();

        // If Redis connection test or initialization is needed, add here:
        // var cache = ServiceProvider.GetRequiredService<ICacheBase>();
        // await cache.PingAsync();  // example
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        if (ServiceProvider is IAsyncDisposable asyncDisposable)
            await asyncDisposable.DisposeAsync();
    }
}

public record TestModel(string Id, string FirstName, string LastName);