using Xunit.Abstractions;

namespace RedisCacheManager.Test.Cache;

public class RedisSetCacheTest(RedisCacheFixture fixture, ITestOutputHelper outputHelper) : IClassFixture<RedisCacheFixture>
{
    private readonly IRedisSetCache _setCache = fixture.ServiceProvider.GetRequiredService<IRedisSetCache>();
    private readonly TestLogging _logger = new(outputHelper);
    private readonly string _setKey = "TestSet";

    [Fact(DisplayName = "Should add item to set")]
    public async Task AddItemToSet()
    {
        _logger.Log("🧩 Adding item to set...");

        await _setCache.AddAsync(_setKey, "Value1");
        var members = await _setCache.GetAllAsync(_setKey);

        Assert.Contains("Value1", members);
        _logger.Log("✅ Item added to set successfully.");
    }

    [Fact(DisplayName = "Should add multiple items to set")]
    public async Task AddMultipleItemsToSet()
    {
        _logger.Log("🧩 Adding multiple items to set...");

        await _setCache.AddAsync(_setKey, ["Value1", "Value2", "Value3"]);
        var members = await _setCache.GetAllAsync(_setKey);

        Assert.Equal(3, members.Count);
        Assert.Contains("Value1", members);
        Assert.Contains("Value2", members);
        Assert.Contains("Value3", members);
        _logger.Log("✅ Multiple items added to set successfully.");
    }

    [Fact(DisplayName = "Should check if item exists in set")]
    public async Task CheckItemExistsInSet()
    {
        _logger.Log("🧩 Checking if item exists in set...");

        await _setCache.AddAsync(_setKey, "Value1");
        var exists = await _setCache.ExistsAsync(_setKey, "Value1");
        var notExists = await _setCache.ExistsAsync(_setKey, "NonExistent");

        Assert.True(exists);
        Assert.False(notExists);
        _logger.Log("✅ Item existence in set checked successfully.");
    }

    [Fact(DisplayName = "Should get set cardinality")]
    public async Task GetSetCardinality()
    {
        _logger.Log("🧩 Getting set cardinality...");

        await _setCache.AddAsync(_setKey, ["Value1", "Value2", "Value3"]);
        var count = await _setCache.GetCountAsync(_setKey);

        Assert.Equal(3, count);
        _logger.Log("✅ Set cardinality retrieved successfully.");
    }

    [Fact(DisplayName = "Should remove item from set")]
    public async Task RemoveItemFromSet()
    {
        _logger.Log("🧩 Removing item from set...");

        await _setCache.AddAsync(_setKey, ["Value1", "Value2"]);
        var removed = await _setCache.RemoveAsync(_setKey, "Value1");

        Assert.True(removed);
        var members = await _setCache.GetAllAsync(_setKey);
        Assert.DoesNotContain("Value1", members);
        Assert.Contains("Value2", members);
        _logger.Log("✅ Item removed from set successfully.");
    }

    [Fact(DisplayName = "Should remove multiple items from set")]
    public async Task RemoveMultipleItemsFromSet()
    {
        _logger.Log("🧩 Removing multiple items from set...");

        await _setCache.AddAsync(_setKey, ["Value1", "Value2", "Value3"]);
        var removed = await _setCache.RemoveAsync(_setKey, ["Value1", "Value2"]);

        Assert.Equal(2, removed);
        var members = await _setCache.GetAllAsync(_setKey);
        Assert.Single(members);
        Assert.Contains("Value3", members);
        _logger.Log("✅ Multiple items removed from set successfully.");
    }

    [Fact(DisplayName = "Should get random item from set")]
    public async Task GetRandomItemFromSet()
    {
        _logger.Log("🧩 Getting random item from set...");

        await _setCache.AddAsync(_setKey, ["Value1", "Value2", "Value3"]);
        var random = await _setCache.GetRandomAsync(_setKey);

        Assert.NotNull(random);
        Assert.Contains("Value", random);
        _logger.Log("✅ Random item from set retrieved successfully.");
    }

    [Fact(DisplayName = "Should get multiple random items from set")]
    public async Task GetMultipleRandomItemsFromSet()
    {
        _logger.Log("🧩 Getting multiple random items from set...");

        await _setCache.AddAsync(_setKey, ["Value1", "Value2", "Value3", "Value4", "Value5"]);
        var random = await _setCache.GetRandomAsync(_setKey, 3);

        Assert.NotNull(random);
        Assert.Equal(3, random.Count);
        _logger.Log("✅ Multiple random items from set retrieved successfully.");
    }

    [Fact(DisplayName = "Should pop random item from set")]
    public async Task PopRandomItemFromSet()
    {
        _logger.Log("🧩 Popping random item from set...");

        await _setCache.AddAsync(_setKey, ["Value1", "Value2", "Value3"]);
        var popped = await _setCache.PopAsync(_setKey);

        Assert.NotNull(popped);
        var members = await _setCache.GetAllAsync(_setKey);
        Assert.Equal(2, members.Count);
        Assert.DoesNotContain(popped, members);
        _logger.Log("✅ Random item popped from set successfully.");
    }

    [Fact(DisplayName = "Should move item between sets")]
    public async Task MoveItemBetweenSets()
    {
        _logger.Log("🧩 Moving item between sets...");

        await _setCache.AddAsync(_setKey, "Value1");
        await _setCache.AddAsync("DestinationSet", "Value2");

        var moved = await _setCache.MoveAsync(_setKey, "DestinationSet", "Value1");

        Assert.True(moved);
        var sourceMembers = await _setCache.GetAllAsync(_setKey);
        var destMembers = await _setCache.GetAllAsync("DestinationSet");

        Assert.DoesNotContain("Value1", sourceMembers);
        Assert.Contains("Value1", destMembers);
        Assert.Contains("Value2", destMembers);
        _logger.Log("✅ Item moved between sets successfully.");
    }

    [Fact(DisplayName = "Should get intersection of sets")]
    public async Task GetIntersectionOfSets()
    {
        _logger.Log("🧩 Getting intersection of sets...");

        await _setCache.AddAsync(_setKey, ["Value1", "Value2", "Value3"]);
        await _setCache.AddAsync("Set2", ["Value2", "Value3", "Value4"]);

        var intersection = await _setCache.IntersectAsync([_setKey, "Set2"]);

        Assert.Equal(2, intersection.Count);
        Assert.Contains("Value2", intersection);
        Assert.Contains("Value3", intersection);
        Assert.DoesNotContain("Value1", intersection);
        Assert.DoesNotContain("Value4", intersection);
        _logger.Log("✅ Intersection of sets retrieved successfully.");
    }

    [Fact(DisplayName = "Should get union of sets")]
    public async Task GetUnionOfSets()
    {
        _logger.Log("🧩 Getting union of sets...");

        await _setCache.AddAsync(_setKey, ["Value1", "Value2"]);
        await _setCache.AddAsync("Set2", ["Value2", "Value3", "Value4"]);

        var union = await _setCache.UnionAsync([_setKey, "Set2"]);

        Assert.Equal(4, union.Count);
        Assert.Contains("Value1", union);
        Assert.Contains("Value2", union);
        Assert.Contains("Value3", union);
        Assert.Contains("Value4", union);
        _logger.Log("✅ Union of sets retrieved successfully.");
    }

    [Fact(DisplayName = "Should get difference of sets")]
    public async Task GetDifferenceOfSets()
    {
        _logger.Log("🧩 Getting difference of sets...");

        await _setCache.AddAsync(_setKey, ["Value1", "Value2", "Value3"]);
        await _setCache.AddAsync("Set2", ["Value2", "Value3", "Value4"]);

        var difference = await _setCache.DifferenceAsync(_setKey, ["Set2"]);

        Assert.Single(difference);
        Assert.Contains("Value1", difference);
        _logger.Log("✅ Difference of sets retrieved successfully.");
    }

    [Fact(DisplayName = "Should store intersection of sets")]
    public async Task StoreIntersectionOfSets()
    {
        _logger.Log("🧩 Storing intersection of sets...");

        await _setCache.AddAsync(_setKey, ["Value1", "Value2", "Value3"]);
        await _setCache.AddAsync("Set2", ["Value2", "Value3", "Value4"]);

        await _setCache.IntersectStoreAsync("IntersectionResult", [_setKey, "Set2"]);
        var result = await _setCache.GetAllAsync("IntersectionResult");

        Assert.Equal(2, result.Count);
        Assert.Contains("Value2", result);
        Assert.Contains("Value3", result);
        _logger.Log("✅ Intersection of sets stored successfully.");
    }

    [Fact(DisplayName = "Should store union of sets")]
    public async Task StoreUnionOfSets()
    {
        _logger.Log("🧩 Storing union of sets...");

        await _setCache.AddAsync(_setKey, ["Value1", "Value2"]);
        await _setCache.AddAsync("Set2", ["Value2", "Value3", "Value4"]);

        await _setCache.UnionStoreAsync("UnionResult", [_setKey, "Set2"]);
        var result = await _setCache.GetAllAsync("UnionResult");

        Assert.Equal(4, result.Count);
        _logger.Log("✅ Union of sets stored successfully.");
    }

    [Fact(DisplayName = "Should store difference of sets")]
    public async Task StoreDifferenceOfSets()
    {
        _logger.Log("🧩 Storing difference of sets...");

        await _setCache.AddAsync(_setKey, ["Value1", "Value2", "Value3"]);
        await _setCache.AddAsync("Set2", ["Value2", "Value3", "Value4"]);

        await _setCache.DifferenceStoreAsync("DifferenceResult", _setKey, ["Set2"]);
        var result = await _setCache.GetAllAsync("DifferenceResult");

        Assert.Single(result);
        Assert.Contains("Value1", result);
        _logger.Log("✅ Difference of sets stored successfully.");
    }

    [Fact(DisplayName = "Should clear set")]
    public async Task ClearSet()
    {
        _logger.Log("🧩 Clearing set...");

        await _setCache.AddAsync(_setKey, ["Value1", "Value2", "Value3"]);
        await _setCache.ClearAsync(_setKey);
        var count = await _setCache.GetCountAsync(_setKey);

        Assert.Equal(0, count);
        _logger.Log("✅ Set cleared successfully.");
    }
}
