using Xunit.Abstractions;

namespace RedisCacheManager.Test.Cache;

public class PortoCacheTest(RedisCacheFixture fixture,ITestOutputHelper outputHelper) : IClassFixture<RedisCacheFixture>
{
    private readonly IProtoCache _cache = fixture.ServiceProvider.GetRequiredService<IProtoCache>();

    private readonly TestLogging _logger = new(outputHelper);

    private readonly Person _model = new()
    {
        Id = 1,
        Email = "amirhossein@gmail.com",
        Name = "Amir hossein baderan",
    };

    private readonly string _key = "Cached-Proto-Item";

    [Fact(DisplayName = "Should set proto cache item successfully")]
    public async Task SetCache()
    {
        _logger.Log("🧩 Setting proto cache item...");

        var result = await _cache.SetItemAsync(_key, _model);

        Assert.NotNull(result);
        Assert.Equal(_model.Id, result.Id);
        Assert.Equal(_model.Name, result.Name);
        Assert.Equal(_model.Email, result.Email);

        _logger.Log("✅ Proto cache item set successfully.");
    }

    [Fact(DisplayName = "Should get or set proto cache item successfully")]
    public async Task GetOrSetCache()
    {
        _logger.Log("🧩 Running GetOrSetCache test for proto cache...");

        var result = await _cache.GetOrSetItemAsync<Person>(_key + ":GetOrSet", async () =>
            new Person
            {
                Id = 2,
                Name = "Amir2",
                Email = "Baderan@gmail2.com",
            }
        );

        Assert.NotNull(result);
        Assert.Equal(2, result.Id);
        Assert.Equal("Amir2", result.Name);
        Assert.Equal("Baderan@gmail2.com", result.Email);

        _logger.Log("✅ Proto cache item retrieved or set successfully.");
    }

    [Fact(DisplayName = "Should get proto cache item successfully")]
    public async Task GetCache()
    {
        _logger.Log("🧩 Retrieving proto cache item...");

        var result = await _cache.GetItemAsync<Person>(_key);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Amir hossein baderan", result.Name);
        Assert.Equal("amirhossein@gmail.com", result.Email);

        _logger.Log("✅ Proto cache item retrieved successfully.");
    }
}