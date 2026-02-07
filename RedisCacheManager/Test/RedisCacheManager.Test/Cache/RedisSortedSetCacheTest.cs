using Xunit.Abstractions;

namespace RedisCacheManager.Test.Cache;

public class RedisSortedSetCacheTest(RedisCacheFixture fixture, ITestOutputHelper outputHelper) : IClassFixture<RedisCacheFixture>
{
    private readonly IRedisSortedSetCache _sortedSetCache = fixture.ServiceProvider.GetRequiredService<IRedisSortedSetCache>();
    private readonly TestLogging _logger = new(outputHelper);
    private readonly string _sortedSetKey = "TestSortedSet";

    [Fact(DisplayName = "Should add item to sorted set")]
    public async Task AddItemToSortedSet()
    {
        _logger.Log("🏆 Adding item to sorted set...");

        await _sortedSetCache.SortedSetAddAsync(_sortedSetKey, "Member1", 1);
        var length = await _sortedSetCache.GetLengthAsync(_sortedSetKey);

        Assert.Equal(1, length);
        _logger.Log("✅ Item added to sorted set successfully.");
    }

    [Fact(DisplayName = "Should add multiple items to sorted set")]
    public async Task AddMultipleItemsToSortedSet()
    {
        _logger.Log("🏆 Adding multiple items to sorted set...");

        await _sortedSetCache.AddAsync(_sortedSetKey, new Dictionary<string, double>
        {
            { "Member1", 1 },
            { "Member2", 2 },
            { "Member3", 3 }
        });
        var length = await _sortedSetCache.GetLengthAsync(_sortedSetKey);

        Assert.Equal(3, length);
        _logger.Log("✅ Multiple items added to sorted set successfully.");
    }

    [Fact(DisplayName = "Should get item score")]
    public async Task GetItemScore()
    {
        _logger.Log("🏆 Getting item score...");

        await _sortedSetCache.AddAsync(_sortedSetKey, "Member1", 1.5);
        var score = await _sortedSetCache.GetScoreAsync(_sortedSetKey, "Member1");

        Assert.Equal(1.5, score);
        _logger.Log("✅ Item score retrieved successfully.");
    }

    [Fact(DisplayName = "Should check if item exists in sorted set")]
    public async Task CheckIfItemExists()
    {
        _logger.Log("🏆 Checking if item exists in sorted set...");

        await _sortedSetCache.AddAsync(_sortedSetKey, "Member1", 1);
        var exists = await _sortedSetCache.ContainsAsync(_sortedSetKey, "Member1");
        var notExists = await _sortedSetCache.ContainsAsync(_sortedSetKey, "Member2");

        Assert.True(exists);
        Assert.False(notExists);
        _logger.Log("✅ Item existence checked successfully.");
    }

    [Fact(DisplayName = "Should get length of sorted set")]
    public async Task GetSortedSetLength()
    {
        _logger.Log("🏆 Getting sorted set length...");

        await _sortedSetCache.AddAsync(_sortedSetKey, new Dictionary<string, double>
        {
            { "Member1", 1 },
            { "Member2", 2 },
            { "Member3", 3 }
        });
        var length = await _sortedSetCache.GetLengthAsync(_sortedSetKey);

        Assert.Equal(3, length);
        _logger.Log("✅ Sorted set length retrieved successfully.");
    }

    [Fact(DisplayName = "Should get range by rank")]
    public async Task GetRangeByRank()
    {
        _logger.Log("🏆 Getting range by rank...");

        await _sortedSetCache.AddAsync(_sortedSetKey, new Dictionary<string, double>
        {
            { "Member1", 1 },
            { "Member2", 2 },
            { "Member3", 3 }
        });
        var range = await _sortedSetCache.GetRangeByRankAsync(_sortedSetKey, 0, 1);

        Assert.Equal(2, range.Count);
        Assert.Equal("Member1", range[0]);
        Assert.Equal("Member2", range[1]);
        _logger.Log("✅ Range by rank retrieved successfully.");
    }

    [Fact(DisplayName = "Should get range by score")]
    public async Task GetRangeByScore()
    {
        _logger.Log("🏆 Getting range by score...");

        await _sortedSetCache.AddAsync(_sortedSetKey, new Dictionary<string, double>
        {
            { "Member1", 1 },
            { "Member2", 2 },
            { "Member3", 3 }
        });
        var range = await _sortedSetCache.GetRangeByScoreAsync(_sortedSetKey, 1.5, 2.5);

        Assert.Equal(1, range.Count);
        Assert.Equal("Member2", range[0]);
        _logger.Log("✅ Range by score retrieved successfully.");
    }

    [Fact(DisplayName = "Should get range with scores")]
    public async Task GetRangeWithScores()
    {
        _logger.Log("🏆 Getting range with scores...");

        await _sortedSetCache.AddAsync(_sortedSetKey, new Dictionary<string, double>
        {
            { "Member1", 1 },
            { "Member2", 2 },
            { "Member3", 3 }
        });
        var range = await _sortedSetCache.GetRangeByRankWithScoresAsync(_sortedSetKey, 0, 2);

        Assert.Equal(3, range.Count);
        Assert.Equal("Member1", range[0].Member);
        Assert.Equal(1, range[0].Score);
        Assert.Equal("Member3", range[2].Member);
        Assert.Equal(3, range[2].Score);
        _logger.Log("✅ Range with scores retrieved successfully.");
    }

    [Fact(DisplayName = "Should get rank of item")]
    public async Task GetRankOfItem()
    {
        _logger.Log("🏆 Getting rank of item...");

        await _sortedSetCache.AddAsync(_sortedSetKey, new Dictionary<string, double>
        {
            { "Member1", 1 },
            { "Member2", 2 },
            { "Member3", 3 }
        });
        var rank = await _sortedSetCache.GetRankAsync(_sortedSetKey, "Member2");

        Assert.Equal(1, rank);
        _logger.Log("✅ Rank of item retrieved successfully.");
    }

    [Fact(DisplayName = "Should increment score")]
    public async Task IncrementScore()
    {
        _logger.Log("🏆 Incrementing score...");

        await _sortedSetCache.AddAsync(_sortedSetKey, "Member1", 1);
        await _sortedSetCache.IncrementScoreAsync(_sortedSetKey, "Member1", 5);
        var newScore = await _sortedSetCache.GetScoreAsync(_sortedSetKey, "Member1");

        Assert.Equal(6, newScore);
        _logger.Log("✅ Score incremented successfully.");
    }

    [Fact(DisplayName = "Should decrement score")]
    public async Task DecrementScore()
    {
        _logger.Log("🏆 Decrementing score...");

        await _sortedSetCache.AddAsync(_sortedSetKey, "Member1", 10);
        await _sortedSetCache.DecrementScoreAsync(_sortedSetKey, "Member1", 3);
        var newScore = await _sortedSetCache.GetScoreAsync(_sortedSetKey, "Member1");

        Assert.Equal(7, newScore);
        _logger.Log("✅ Score decremented successfully.");
    }

    [Fact(DisplayName = "Should remove item from sorted set")]
    public async Task RemoveItemFromSortedSet()
    {
        _logger.Log("🏆 Removing item from sorted set...");

        await _sortedSetCache.AddAsync(_sortedSetKey, new Dictionary<string, double>
        {
            { "Member1", 1 },
            { "Member2", 2 },
            { "Member3", 3 }
        });
        await _sortedSetCache.RemoveAsync(_sortedSetKey, "Member2");
        var length = await _sortedSetCache.GetLengthAsync(_sortedSetKey);

        Assert.Equal(2, length);
        var exists = await _sortedSetCache.ContainsAsync(_sortedSetKey, "Member2");
        Assert.False(exists);
        _logger.Log("✅ Item removed from sorted set successfully.");
    }

    [Fact(DisplayName = "Should remove range by rank")]
    public async Task RemoveRangeByRank()
    {
        _logger.Log("🏆 Removing range by rank...");

        await _sortedSetCache.AddAsync(_sortedSetKey, new Dictionary<string, double>
        {
            { "Member1", 1 },
            { "Member2", 2 },
            { "Member3", 3 }
        });
        var removed = await _sortedSetCache.RemoveRangeByRankAsync(_sortedSetKey, 0, 1);

        Assert.Equal(2, removed);
        var length = await _sortedSetCache.GetLengthAsync(_sortedSetKey);
        Assert.Equal(1, length);
        _logger.Log("✅ Range by rank removed successfully.");
    }

    [Fact(DisplayName = "Should remove range by score")]
    public async Task RemoveRangeByScore()
    {
        _logger.Log("🏆 Removing range by score...");

        await _sortedSetCache.AddAsync(_sortedSetKey, new Dictionary<string, double>
        {
            { "Member1", 1 },
            { "Member2", 2 },
            { "Member3", 3 }
        });
        var removed = await _sortedSetCache.RemoveRangeByScoreAsync(_sortedSetKey, 1, 2);

        Assert.Equal(2, removed);
        var length = await _sortedSetCache.GetLengthAsync(_sortedSetKey);
        Assert.Equal(1, length);
        _logger.Log("✅ Range by score removed successfully.");
    }

    [Fact(DisplayName = "Should get count by score")]
    public async Task GetCountByScore()
    {
        _logger.Log("🏆 Getting count by score...");

        await _sortedSetCache.AddAsync(_sortedSetKey, new Dictionary<string, double>
        {
            { "Member1", 1 },
            { "Member2", 2 },
            { "Member3", 2 }
        });
        var count = await _sortedSetCache.GetCountByScoreAsync(_sortedSetKey, 2, 2);

        Assert.Equal(2, count);
        _logger.Log("✅ Count by score retrieved successfully.");
    }

    [Fact(DisplayName = "Should get all items in ascending order")]
    public async Task GetAllItemsAscending()
    {
        _logger.Log("🏆 Getting all items in ascending order...");

        await _sortedSetCache.AddAsync(_sortedSetKey, new Dictionary<string, double>
        {
            { "Member1", 3 },
            { "Member2", 1 },
            { "Member3", 2 }
        });
        var allItems = await _sortedSetCache.GetAllAsync(_sortedSetKey);

        Assert.Equal(3, allItems.Count);
        Assert.Equal("Member2", allItems[0]);
        Assert.Equal("Member3", allItems[1]);
        Assert.Equal("Member1", allItems[2]);
        _logger.Log("✅ All items in ascending order retrieved successfully.");
    }

    [Fact(DisplayName = "Should get all items in descending order")]
    public async Task GetAllItemsDescending()
    {
        _logger.Log("🏆 Getting all items in descending order...");

        await _sortedSetCache.AddAsync(_sortedSetKey, new Dictionary<string, double>
        {
            { "Member1", 3 },
            { "Member2", 1 },
            { "Member3", 2 }
        });
        var allItems = await _sortedSetCache.GetAllAsync(_sortedSetKey, Order.Descending);

        Assert.Equal(3, allItems.Count);
        Assert.Equal("Member1", allItems[0]);
        Assert.Equal("Member3", allItems[1]);
        Assert.Equal("Member2", allItems[2]);
        _logger.Log("✅ All items in descending order retrieved successfully.");
    }

    [Fact(DisplayName = "Should get all items with scores")]
    public async Task GetAllItemsWithScores()
    {
        _logger.Log("🏆 Getting all items with scores...");

        await _sortedSetCache.AddAsync(_sortedSetKey, new Dictionary<string, double>
        {
            { "Member1", 1 },
            { "Member2", 2 },
            { "Member3", 3 }
        });
        var allItems = await _sortedSetCache.GetAllWithScoresAsync(_sortedSetKey);

        Assert.Equal(3, allItems.Count);
        Assert.Equal("Member1", allItems[0].Member);
        Assert.Equal(1, allItems[0].Score);
        Assert.Equal("Member3", allItems[2].Member);
        Assert.Equal(3, allItems[2].Score);
        _logger.Log("✅ All items with scores retrieved successfully.");
    }

    [Fact(DisplayName = "Should clear sorted set")]
    public async Task ClearSortedSet()
    {
        _logger.Log("🏆 Clearing sorted set...");

        await _sortedSetCache.AddAsync(_sortedSetKey, new Dictionary<string, double>
        {
            { "Member1", 1 },
            { "Member2", 2 },
            { "Member3", 3 }
        });
        await _sortedSetCache.ClearAsync(_sortedSetKey);
        var length = await _sortedSetCache.GetLengthAsync(_sortedSetKey);

        Assert.Equal(0, length);
        _logger.Log("✅ Sorted set cleared successfully.");
    }
}
