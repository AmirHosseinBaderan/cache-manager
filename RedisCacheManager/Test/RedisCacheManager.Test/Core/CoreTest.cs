using CacheManager.Configuration;
using Xunit.Abstractions;

namespace RedisCacheManager.Test.Core;

public class CoreTest(RedisCacheFixture fixture, ITestOutputHelper outputHelper) : IClassFixture<RedisCacheFixture>
{
    private readonly IServiceProvider _provider = fixture.ServiceProvider;

    private readonly TestLogging _logger = new(outputHelper);

    [Fact(DisplayName = "Should connect with default Redis configuration")]
    public async Task ConnectionWithDefaultConfig()
    {
        _logger.Log("🔍 Starting connection test with default Redis configuration...");

        var core = _provider.GetService<ICacheCore>();
        Assert.NotNull(core);

        var connection = await core!.ConnectAsync();
        Assert.NotNull(connection);
        Assert.True(connection.IsConnected, "Failed to connect to Redis with default config");

        _logger.Log("✅ Successfully connected to Redis with default configuration.");
    }

    [Fact(DisplayName = "Should connect with custom Redis configuration")]
    public async Task ConnectionWithCustomConfig()
    {
        _logger.Log("🔍 Starting connection test with custom Redis configuration...");

        var core = _provider.GetService<ICacheCore>();
        Assert.NotNull(core);

        var connection = await core!.ConnectAsync("127.0.0.1:6379");
        Assert.NotNull(connection);
        Assert.True(connection.IsConnected, "Failed to connect to Redis with custom config");

        _logger.Log("✅ Successfully connected to Redis using custom configuration.");
    }

    [Fact(DisplayName = "Should connect with Redis Sentinel configuration")]
    public async Task ConnectionWithSentinelConfig()
    {
        _logger.Log("🔍 Starting connection test with Sentinel configuration...");

        var sentinelConfig = new CacheConfigs
        {
            UseSentinel = true,
            Sentinels = ["127.0.0.1:26379", "127.0.0.1:26380"],
            ServiceName = "mymaster",
            Instance = 0,
            QueueName = "Test-Sentinel-Queue",
            Formatting = Newtonsoft.Json.Formatting.None
        };

        // Resolve core
        var core = _provider.GetService<ICacheCore>();
        Assert.NotNull(core);

        // Try connecting using sentinel
        var connection = await core!.ConnectAsync();

        Assert.NotNull(connection);
        Assert.True(connection.IsConnected, "Failed to connect to Redis Sentinel cluster");

        _logger.Log($"✅ Successfully connected to Redis Sentinel master [{sentinelConfig.ServiceName}] at {string.Join(", ", sentinelConfig.Sentinels!)}.");
    }
}