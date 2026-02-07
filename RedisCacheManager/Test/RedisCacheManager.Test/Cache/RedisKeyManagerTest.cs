using Xunit.Abstractions;

namespace RedisCacheManager.Test.Cache;

public class RedisKeyManagerTest(RedisCacheFixture fixture, ITestOutputHelper outputHelper) : IClassFixture<RedisCacheFixture>
{
    private readonly IRedisKeyManager _keyManager = fixture.ServiceProvider.GetRequiredService<IRedisKeyManager>();
    private readonly TestLogging _logger = new(outputHelper);
    private readonly string _testKey = "TestKey";

    [Fact(DisplayName = "Should check if key exists")]
    public async Task KeyExists()
    {
        _logger.Log("🧩 Checking if key exists...");

        await fixture.ServiceProvider.GetRequiredService<ICacheBase>().SetAsync(_testKey, "Value");
        var exists = await _keyManager.ExistsAsync(_testKey);

        Assert.True(exists);
        _logger.Log("✅ Key existence checked successfully.");
    }

    [Fact(DisplayName = "Should delete key successfully")]
    public async Task DeleteKey()
    {
        _logger.Log("🧩 Deleting key...");

        await fixture.ServiceProvider.GetRequiredService<ICacheBase>().SetAsync(_testKey, "Value");
        var result = await _keyManager.DeleteAsync(_testKey);

        Assert.True(result);

        var exists = await _keyManager.ExistsAsync(_testKey);
        Assert.False(exists);
        _logger.Log("✅ Key deleted successfully.");
    }

    [Fact(DisplayName = "Should delete multiple keys successfully")]
    public async Task DeleteMultipleKeys()
    {
        _logger.Log("🧩 Deleting multiple keys...");

        await fixture.ServiceProvider.GetRequiredService<ICacheBase>().SetAsync("Key1", "Value1");
        await fixture.ServiceProvider.GetRequiredService<ICacheBase>().SetAsync("Key2", "Value2");
        await fixture.ServiceProvider.GetRequiredService<ICacheBase>().SetAsync("Key3", "Value3");

        var result = await _keyManager.DeleteAsync(["Key1", "Key2", "Key3"]);

        Assert.Equal(3, result);
        _logger.Log("✅ Multiple keys deleted successfully.");
    }

    [Fact(DisplayName = "Should get key TTL successfully")]
    public async Task GetKeyTTL()
    {
        _logger.Log("🧩 Getting key TTL...");

        await fixture.ServiceProvider.GetRequiredService<ICacheBase>().SetAsync(_testKey, "Value", TimeSpan.FromSeconds(60));

        var ttl = await _keyManager.GetTimeToLiveAsync(_testKey);

        Assert.True(ttl.HasValue);
        Assert.True(ttl.Value.TotalSeconds > 0 && ttl.Value.TotalSeconds <= 60);
        _logger.Log("✅ Key TTL retrieved successfully.");
    }

    [Fact(DisplayName = "Should set key expiration successfully")]
    public async Task SetKeyExpiration()
    {
        _logger.Log("🧩 Setting key expiration...");

        await fixture.ServiceProvider.GetRequiredService<ICacheBase>().SetAsync(_testKey, "Value");
        var result = await _keyManager.SetTimeToLiveAsync(_testKey, TimeSpan.FromSeconds(30));

        Assert.True(result);

        var ttl = await _keyManager.GetTimeToLiveAsync(_testKey);
        Assert.True(ttl.HasValue && ttl.Value.TotalSeconds <= 30);
        _logger.Log("✅ Key expiration set successfully.");
    }

    [Fact(DisplayName = "Should persist key successfully")]
    public async Task PersistKey()
    {
        _logger.Log("🧩 Persisting key...");

        await fixture.ServiceProvider.GetRequiredService<ICacheBase>().SetAsync(_testKey, "Value", TimeSpan.FromSeconds(60));
        await _keyManager.SetTimeToLiveAsync(_testKey, TimeSpan.FromSeconds(60));

        var result = await _keyManager.PersistAsync(_testKey);

        Assert.True(result);

        var ttl = await _keyManager.GetTimeToLiveAsync(_testKey);
        Assert.Null(ttl); // No expiration
        _logger.Log("✅ Key persisted successfully.");
    }

    [Fact(DisplayName = "Should rename key successfully")]
    public async Task RenameKey()
    {
        _logger.Log("🧩 Renaming key...");

        await fixture.ServiceProvider.GetRequiredService<ICacheBase>().SetAsync(_testKey, "Value");
        var result = await _keyManager.RenameAsync(_testKey, "NewKeyName");

        Assert.True(result);

        var existsOld = await _keyManager.ExistsAsync(_testKey);
        var existsNew = await _keyManager.ExistsAsync("NewKeyName");

        Assert.False(existsOld);
        Assert.True(existsNew);
        _logger.Log("✅ Key renamed successfully.");
    }

    [Fact(DisplayName = "Should rename key only if destination exists")]
    public async Task RenameKeyIfExists()
    {
        _logger.Log("🧩 Renaming key if destination exists...");

        await fixture.ServiceProvider.GetRequiredService<ICacheBase>().SetAsync(_testKey, "Value");
        await fixture.ServiceProvider.GetRequiredService<ICacheBase>().SetAsync("DestKey", "DestValue");

        var result = await _keyManager.RenameIfExistsAsync(_testKey, "DestKey");

        Assert.True(result);

        var value = await fixture.ServiceProvider.GetRequiredService<ICacheBase>().GetAsync<string>("DestKey");
        Assert.Equal("Value", value);
        _logger.Log("✅ Key renamed (if exists) successfully.");
    }

    [Fact(DisplayName = "Should get key type successfully")]
    public async Task GetKeyType()
    {
        _logger.Log("🧩 Getting key type...");

        await fixture.ServiceProvider.GetRequiredService<ICacheBase>().SetAsync(_testKey, "Value");

        var type = await _keyManager.GetTypeAsync(_testKey);

        Assert.NotNull(type);
        Assert.Equal("string", type);
        _logger.Log("✅ Key type retrieved successfully.");
    }

    [Fact(DisplayName = "Should scan keys successfully")]
    public async Task ScanKeys()
    {
        _logger.Log("🧩 Scanning keys...");

        // Set up test keys
        await fixture.ServiceProvider.GetRequiredService<ICacheBase>().SetAsync("ScanKey1", "Value1");
        await fixture.ServiceProvider.GetRequiredService<ICacheBase>().SetAsync("ScanKey2", "Value2");
        await fixture.ServiceProvider.GetRequiredService<ICacheBase>().SetAsync("ScanKey3", "Value3");

        var keys = await _keyManager.ScanAsync("ScanKey*", 10);

        Assert.NotNull(keys);
        Assert.Equal(3, keys.Count);
        _logger.Log("✅ Keys scanned successfully.");
    }

    [Fact(DisplayName = "Should get random key successfully")]
    public async Task GetRandomKey()
    {
        _logger.Log("🧩 Getting random key...");

        await fixture.ServiceProvider.GetRequiredService<ICacheBase>().SetAsync("RandomKey1", "Value1");
        await fixture.ServiceProvider.GetRequiredService<ICacheBase>().SetAsync("RandomKey2", "Value2");

        var key = await _keyManager.GetRandomKeyAsync();

        Assert.NotNull(key);
        Assert.Contains("RandomKey", key);
        _logger.Log("✅ Random key retrieved successfully.");
    }

    [Fact(DisplayName = "Should get multiple keys with type info")]
    public async Task GetKeysWithTypeInfo()
    {
        _logger.Log("🧩 Getting keys with type info...");

        await fixture.ServiceProvider.GetRequiredService<ICacheBase>().SetAsync("TypeKey1", "Value1");
        await fixture.ServiceProvider.GetRequiredService<IRedisListCache>().PushAsync("TypeKey2", "Item1");

        var result = await _keyManager.GetKeysWithTypeInfoAsync(["TypeKey1", "TypeKey2"]);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("string", result["TypeKey1"]);
        Assert.Equal("list", result["TypeKey2"]);
        _logger.Log("✅ Keys with type info retrieved successfully.");
    }

    [Fact(DisplayName = "Should get key memory usage")]
    public async Task GetKeyMemoryUsage()
    {
        _logger.Log("🧩 Getting key memory usage...");

        await fixture.ServiceProvider.GetRequiredService<ICacheBase>().SetAsync(_testKey, "This is a test value for memory usage");

        var memory = await _keyManager.GetMemoryUsageAsync(_testKey);

        Assert.True(memory > 0);
        _logger.Log("✅ Key memory usage retrieved successfully.");
    }

    [Fact(DisplayName = "Should dump key successfully")]
    public async Task DumpKey()
    {
        _logger.Log("🧩 Dumping key...");

        await fixture.ServiceProvider.GetRequiredService<ICacheBase>().SetAsync(_testKey, "Value");

        var dump = await _keyManager.DumpAsync(_testKey);

        Assert.NotNull(dump);
        Assert.True(dump.Length > 0);
        _logger.Log("✅ Key dumped successfully.");
    }

    [Fact(DisplayName = "Should restore key from dump")]
    public async Task RestoreKeyFromDump()
    {
        _logger.Log("🧩 Restoring key from dump...");

        await fixture.ServiceProvider.GetRequiredService<ICacheBase>().SetAsync(_testKey, "Value");
        var dump = await _keyManager.DumpAsync(_testKey);

        // Delete original
        await _keyManager.DeleteAsync(_testKey);

        // Restore
        var result = await _keyManager.RestoreAsync("RestoredKey", dump);

        Assert.True(result);

        var value = await fixture.ServiceProvider.GetRequiredService<ICacheBase>().GetAsync<string>("RestoredKey");
        Assert.Equal("Value", value);
        _logger.Log("✅ Key restored from dump successfully.");
    }
}
