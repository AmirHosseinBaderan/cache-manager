using Xunit.Abstractions;

namespace RedisCacheManager.Test.Cache;

public class RedisListCacheTest(RedisCacheFixture fixture, ITestOutputHelper outputHelper) : IClassFixture<RedisCacheFixture>
{
    private readonly IRedisListCache _listCache = fixture.ServiceProvider.GetRequiredService<IRedisListCache>();
    private readonly TestLogging _logger = new(outputHelper);
    private readonly string _listKey = "TestList";

    [Fact(DisplayName = "Should push item to list")]
    public async Task PushItemToList()
    {
        _logger.Log("📚 Pushing item to list...");

        await _listCache.PushAsync(_listKey, "Item1");
        var length = await _listCache.GetLengthAsync(_listKey);

        Assert.Equal(1, length);
        _logger.Log("✅ Item pushed to list successfully.");
    }

    [Fact(DisplayName = "Should push multiple items to list")]
    public async Task PushMultipleItemsToList()
    {
        _logger.Log("📚 Pushing multiple items to list...");

        await _listCache.PushAsync(_listKey, ["Item1", "Item2", "Item3"]);
        var length = await _listCache.GetLengthAsync(_listKey);

        Assert.Equal(3, length);
        _logger.Log("✅ Multiple items pushed to list successfully.");
    }

    [Fact(DisplayName = "Should push item to left of list")]
    public async Task PushItemToLeftOfList()
    {
        _logger.Log("📚 Pushing item to left of list...");

        await _listCache.PushLeftAsync(_listKey, "Item0");
        var firstItem = await _listCache.GetByIndexAsync(_listKey, 0);

        Assert.Equal("Item0", firstItem);
        _logger.Log("✅ Item pushed to left of list successfully.");
    }

    [Fact(DisplayName = "Should pop item from list")]
    public async Task PopItemFromList()
    {
        _logger.Log("📚 Popping item from list...");

        await _listCache.PushAsync(_listKey, "Item1");
        var poppedItem = await _listCache.PopAsync(_listKey);

        Assert.Equal("Item1", poppedItem);
        var length = await _listCache.GetLengthAsync(_listKey);
        Assert.Equal(0, length);
        _logger.Log("✅ Item popped from list successfully.");
    }

    [Fact(DisplayName = "Should pop item from left of list")]
    public async Task PopItemFromLeftOfList()
    {
        _logger.Log("📚 Popping item from left of list...");

        await _listCache.PushAsync(_listKey, ["Item1", "Item2"]);
        var poppedItem = await _listCache.PopLeftAsync(_listKey);

        Assert.Equal("Item2", poppedItem);
        var firstItem = await _listCache.GetByIndexAsync(_listKey, 0);
        Assert.Equal("Item1", firstItem);
        _logger.Log("✅ Item popped from left of list successfully.");
    }

    [Fact(DisplayName = "Should get item by index")]
    public async Task GetItemByIndex()
    {
        _logger.Log("📚 Getting item by index...");

        await _listCache.PushAsync(_listKey, ["Item1", "Item2", "Item3"]);
        var item = await _listCache.GetByIndexAsync(_listKey, 1);

        Assert.Equal("Item2", item);
        _logger.Log("✅ Item by index retrieved successfully.");
    }

    [Fact(DisplayName = "Should get list range")]
    public async Task GetListRange()
    {
        _logger.Log("📚 Getting list range...");

        await _listCache.PushAsync(_listKey, ["Item1", "Item2", "Item3", "Item4", "Item5"]);
        var range = await _listCache.GetRangeAsync(_listKey, 1, 3);

        Assert.Equal(3, range.Count);
        Assert.Equal("Item2", range[0]);
        Assert.Equal("Item3", range[1]);
        Assert.Equal("Item4", range[2]);
        _logger.Log("✅ List range retrieved successfully.");
    }

    [Fact(DisplayName = "Should get all items from list")]
    public async Task GetAllItemsFromList()
    {
        _logger.Log("📚 Getting all items from list...");

        await _listCache.PushAsync(_listKey, ["Item1", "Item2", "Item3"]);
        var allItems = await _listCache.GetAllAsync(_listKey);

        Assert.Equal(3, allItems.Count);
        Assert.Equal("Item1", allItems[0]);
        Assert.Equal("Item2", allItems[1]);
        Assert.Equal("Item3", allItems[2]);
        _logger.Log("✅ All items from list retrieved successfully.");
    }

    [Fact(DisplayName = "Should get list length")]
    public async Task GetListLength()
    {
        _logger.Log("📚 Getting list length...");

        await _listCache.PushAsync(_listKey, ["Item1", "Item2", "Item3"]);
        var length = await _listCache.GetLengthAsync(_listKey);

        Assert.Equal(3, length);
        _logger.Log("✅ List length retrieved successfully.");
    }

    [Fact(DisplayName = "Should set item by index")]
    public async Task SetItemByIndex()
    {
        _logger.Log("📚 Setting item by index...");

        await _listCache.PushAsync(_listKey, ["Item1", "Item2", "Item3"]);
        await _listCache.SetByIndexAsync(_listKey, 1, "NewItem2");
        var item = await _listCache.GetByIndexAsync(_listKey, 1);

        Assert.Equal("NewItem2", item);
        _logger.Log("✅ Item by index set successfully.");
    }

    [Fact(DisplayName = "Should remove item from list")]
    public async Task RemoveItemFromList()
    {
        _logger.Log("📚 Removing item from list...");

        await _listCache.PushAsync(_listKey, ["Item1", "Item2", "Item3"]);
        var removed = await _listCache.RemoveAsync(_listKey, "Item2");

        Assert.Equal(1, removed);
        var allItems = await _listCache.GetAllAsync(_listKey);
        Assert.Equal(2, allItems.Count);
        Assert.DoesNotContain("Item2", allItems);
        _logger.Log("✅ Item from list removed successfully.");
    }

    [Fact(DisplayName = "Should remove items by count from list")]
    public async Task RemoveItemsByCountFromList()
    {
        _logger.Log("📚 Removing items by count from list...");

        await _listCache.PushAsync(_listKey, ["Item1", "Item2", "Item2", "Item3"]);
        var removed = await _listCache.RemoveAsync(_listKey, "Item2", 2);

        Assert.Equal(2, removed);
        var allItems = await _listCache.GetAllAsync(_listKey);
        Assert.Equal(2, allItems.Count);
        _logger.Log("✅ Items by count from list removed successfully.");
    }

    [Fact(DisplayName = "Should trim list")]
    public async Task TrimList()
    {
        _logger.Log("📚 Trimming list...");

        await _listCache.PushAsync(_listKey, ["Item1", "Item2", "Item3", "Item4", "Item5"]);
        await _listCache.TrimAsync(_listKey, 1, 3);
        var length = await _listCache.GetLengthAsync(_listKey);
        var allItems = await _listCache.GetAllAsync(_listKey);

        Assert.Equal(3, length);
        Assert.Equal("Item2", allItems[0]);
        Assert.Equal("Item4", allItems[2]);
        _logger.Log("✅ List trimmed successfully.");
    }

    [Fact(DisplayName = "Should block pop from list")]
    public async Task BlockPopFromList()
    {
        _logger.Log("📚 Blocking pop from list...");

        // First, add an item
        await _listCache.PushAsync(_listKey, "Item1");

        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var poppedItem = await _listCache.BlockPopAsync(_listKey, TimeSpan.FromMilliseconds(100), cancellationToken: cts.Token);

        Assert.Equal("Item1", poppedItem);
        _logger.Log("✅ Block pop from list completed successfully.");
    }

    [Fact(DisplayName = "Should get index of item")]
    public async Task GetIndexOfItem()
    {
        _logger.Log("📚 Getting index of item...");

        await _listCache.PushAsync(_listKey, ["Item1", "Item2", "Item3"]);
        var index = await _listCache.IndexOfAsync(_listKey, "Item2");

        Assert.Equal(1, index);
        _logger.Log("✅ Index of item retrieved successfully.");
    }

    [Fact(DisplayName = "Should insert item after another item")]
    public async Task InsertItemAfterAnotherItem()
    {
        _logger.Log("📚 Inserting item after another item...");

        await _listCache.PushAsync(_listKey, ["Item1", "Item3"]);
        var length = await _listCache.InsertAfterAsync(_listKey, "Item1", "Item2");

        Assert.Equal(3, length);
        var allItems = await _listCache.GetAllAsync(_listKey);
        Assert.Equal("Item2", allItems[1]);
        _logger.Log("✅ Item inserted after another item successfully.");
    }

    [Fact(DisplayName = "Should insert item before another item")]
    public async Task InsertItemBeforeAnotherItem()
    {
        _logger.Log("📚 Inserting item before another item...");

        await _listCache.PushAsync(_listKey, ["Item1", "Item3"]);
        var length = await _listCache.InsertBeforeAsync(_listKey, "Item3", "Item2");

        Assert.Equal(3, length);
        var allItems = await _listCache.GetAllAsync(_listKey);
        Assert.Equal("Item2", allItems[1]);
        _logger.Log("✅ Item inserted before another item successfully.");
    }

    [Fact(DisplayName = "Should clear list")]
    public async Task ClearList()
    {
        _logger.Log("📚 Clearing list...");

        await _listCache.PushAsync(_listKey, ["Item1", "Item2", "Item3"]);
        await _listCache.ClearAsync(_listKey);
        var length = await _listCache.GetLengthAsync(_listKey);

        Assert.Equal(0, length);
        _logger.Log("✅ List cleared successfully.");
    }
}
