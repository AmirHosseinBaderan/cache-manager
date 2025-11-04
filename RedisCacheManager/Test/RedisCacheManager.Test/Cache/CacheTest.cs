using Xunit.Abstractions;

namespace RedisCacheManager.Test.Cache;

public class CacheTest(RedisCacheFixture fixture,ITestOutputHelper outputHelper) : IClassFixture<RedisCacheFixture>
{
    private readonly IJsonCache _cache = fixture.ServiceProvider.GetRequiredService<IJsonCache>();
    private readonly TestLogging _logger = new(outputHelper);

    private readonly CacheModel _model = new("1", "Amir", "Baderan");
    private readonly string _key = "Cached-Item";

    [Fact(DisplayName = "Should set cache item successfully")]
    public async Task SetCache()
    {
        _logger.Log("🧩 Setting cache item...");

        var result = await _cache.SetItemAsync(_key, _model);

        Assert.NotNull(result);
        Assert.Equal(_model.Id, result.Id);
        Assert.Equal(_model.Name, result.Name);
        Assert.Equal(_model.LastName, result.LastName);

        _logger.Log("✅ Item set successfully in cache.");
    }

    [Fact(DisplayName = "Should get or set cache item successfully")]
    public async Task GetOrSetCache()
    {
        _logger.Log("🧩 Running GetOrSetCache test...");

        var result = await _cache.GetOrSetItemAsync(_key + ":GetOrSet", () =>
            new CacheModel("2", "Amir2", "Baderan2")
        );

        Assert.NotNull(result);
        Assert.Equal("2", result.Id);
        Assert.Equal("Amir2", result.Name);
        Assert.Equal("Baderan2", result.LastName);

        _logger.Log("✅ Item retrieved or set successfully.");
    }

    [Fact(DisplayName = "Should get cache item successfully")]
    public async Task GetCache()
    {
        _logger.Log("🧩 Retrieving cache item...");

        var result = await _cache.GetItemAsync<CacheModel>(_key);

        Assert.NotNull(result);
        Assert.Equal("1", result.Id);
        Assert.Equal("Amir", result.Name);
        Assert.Equal("Baderan", result.LastName);

        _logger.Log("✅ Cache item retrieved successfully.");
    }
}
