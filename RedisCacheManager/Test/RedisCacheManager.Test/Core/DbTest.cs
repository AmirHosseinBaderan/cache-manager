namespace RedisCacheManager.Test.Core;

public class DbTest(RedisCacheFixture fixture) : IClassFixture<RedisCacheFixture>
{
    private readonly IServiceProvider _provider = fixture.ServiceProvider;
    private readonly ILogger<DbTest> _logger = fixture.ServiceProvider.GetRequiredService<ILogger<DbTest>>();

    [Fact(DisplayName = "Should open Redis database with default configuration")]
    public async Task GetDataBaseInstanceWithDefaultConfig()
    {
        _logger.LogInformation("🔍 Starting test: Get Redis database using default config...");

        var dbService = _provider.GetService<ICacheDb>();
        Assert.NotNull(dbService);

        var db = await dbService!.GetDataBaseAsync();
        Assert.NotNull(db);

        _logger.LogInformation("✅ Redis database opened successfully with default configuration.");
    }

    [Fact(DisplayName = "Should open Redis database with custom configuration")]
    public async Task GetDataBaseInstanceWithCustomConfig()
    {
        _logger.LogInformation("🔍 Starting test: Get Redis database using custom config...");

        var dbService = _provider.GetService<ICacheDb>();
        Assert.NotNull(dbService);

        var db = await dbService!.GetDataBaseAsync("127.0.0.1:6379", 1);
        Assert.NotNull(db);

        _logger.LogInformation("✅ Redis database opened successfully with custom configuration.");
    }
}