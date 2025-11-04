namespace RedisCacheManager.Test.Core;

public class CoreTest(RedisCacheFixture fixture) : IClassFixture<RedisCacheFixture>
{
    private readonly IServiceProvider _provider = fixture.ServiceProvider;
    private readonly ILogger<CoreTest> _logger = fixture.ServiceProvider.GetRequiredService<ILogger<CoreTest>>();

    [Fact(DisplayName = "Should connect with default Redis configuration")]
    public async Task ConnectionWithDefaultConfig()
    {
        _logger.LogInformation("🔍 Starting connection test with default Redis configuration...");

        var core = _provider.GetService<ICacheCore>();
        Assert.NotNull(core);

        var connection = await core!.ConnectAsync();

        Assert.NotNull(connection);
        Assert.True(connection.IsConnected, "Failed to connect to Redis with default config");

        _logger.LogInformation("✅ Successfully connected to Redis with default configuration.");
    }

    [Fact(DisplayName = "Should connect with custom Redis configuration")]
    public async Task ConnectionWithCustomConfig()
    {
        _logger.LogInformation("🔍 Starting connection test with custom Redis configuration...");

        var core = _provider.GetService<ICacheCore>();
        Assert.NotNull(core);

        var connection = await core!.ConnectAsync("127.0.0.1:6379");

        Assert.NotNull(connection);
        Assert.True(connection.IsConnected, "Failed to connect to Redis with custom config");

        _logger.LogInformation("✅ Successfully connected to Redis using custom configuration.");
    }
}