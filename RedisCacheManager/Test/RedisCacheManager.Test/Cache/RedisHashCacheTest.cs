using Xunit.Abstractions;

namespace RedisCacheManager.Test.Cache;

public class RedisHashCacheTest(RedisCacheFixture fixture, ITestOutputHelper outputHelper) : IClassFixture<RedisCacheFixture>
{
    private readonly IRedisHashCache _hashCache = fixture.ServiceProvider.GetRequiredService<IRedisHashCache>();
    private readonly TestLogging _logger = new(outputHelper);
    private readonly string _hashKey = "TestHash";

    [Fact(DisplayName = "Should set field in hash")]
    public async Task SetFieldInHash()
    {
        _logger.Log("🧩 Setting field in hash...");

        await _hashCache.SetAsync(_hashKey, "Field1", "Value1");
        var value = await _hashCache.GetAsync(_hashKey, "Field1");

        Assert.Equal("Value1", value);
        _logger.Log("✅ Field set in hash successfully.");
    }

    [Fact(DisplayName = "Should set multiple fields in hash")]
    public async Task SetMultipleFieldsInHash()
    {
        _logger.Log("🧩 Setting multiple fields in hash...");

        var fields = new Dictionary<string, string>
        {
            { "Field1", "Value1" },
            { "Field2", "Value2" },
            { "Field3", "Value3" }
        };

        await _hashCache.SetAsync(_hashKey, fields);
        var value1 = await _hashCache.GetAsync(_hashKey, "Field1");
        var value2 = await _hashCache.GetAsync(_hashKey, "Field2");
        var value3 = await _hashCache.GetAsync(_hashKey, "Field3");

        Assert.Equal("Value1", value1);
        Assert.Equal("Value2", value2);
        Assert.Equal("Value3", value3);
        _logger.Log("✅ Multiple fields set in hash successfully.");
    }

    [Fact(DisplayName = "Should get field from hash")]
    public async Task GetFieldFromHash()
    {
        _logger.Log("🧩 Getting field from hash...");

        await _hashCache.SetAsync(_hashKey, "Field1", "Value1");
        var value = await _hashCache.GetAsync(_hashKey, "Field1");

        Assert.Equal("Value1", value);
        _logger.Log("✅ Field from hash retrieved successfully.");
    }

    [Fact(DisplayName = "Should get all fields from hash")]
    public async Task GetAllFieldsFromHash()
    {
        _logger.Log("🧩 Getting all fields from hash...");

        var fields = new Dictionary<string, string>
        {
            { "Field1", "Value1" },
            { "Field2", "Value2" }
        };

        await _hashCache.SetAsync(_hashKey, fields);
        var allFields = await _hashCache.GetAllAsync(_hashKey);

        Assert.Equal(2, allFields.Count);
        Assert.Equal("Value1", allFields["Field1"]);
        Assert.Equal("Value2", allFields["Field2"]);
        _logger.Log("✅ All fields from hash retrieved successfully.");
    }

    [Fact(DisplayName = "Should check if field exists in hash")]
    public async Task CheckFieldExistsInHash()
    {
        _logger.Log("🧩 Checking if field exists in hash...");

        await _hashCache.SetAsync(_hashKey, "Field1", "Value1");
        var exists = await _hashCache.ExistsAsync(_hashKey, "Field1");
        var notExists = await _hashCache.ExistsAsync(_hashKey, "NonExistent");

        Assert.True(exists);
        Assert.False(notExists);
        _logger.Log("✅ Field existence in hash checked successfully.");
    }

    [Fact(DisplayName = "Should delete field from hash")]
    public async Task DeleteFieldFromHash()
    {
        _logger.Log("🧩 Deleting field from hash...");

        await _hashCache.SetAsync(_hashKey, new Dictionary<string, string> { { "Field1", "Value1" }, { "Field2", "Value2" } });
        var deleted = await _hashCache.DeleteAsync(_hashKey, ["Field1"]);

        Assert.Equal(1, deleted);
        var allFields = await _hashCache.GetAllAsync(_hashKey);
        Assert.False(allFields.ContainsKey("Field1"));
        Assert.True(allFields.ContainsKey("Field2"));
        _logger.Log("✅ Field from hash deleted successfully.");
    }

    [Fact(DisplayName = "Should get hash length")]
    public async Task GetHashLength()
    {
        _logger.Log("🧩 Getting hash length...");

        await _hashCache.SetAsync(_hashKey, new Dictionary<string, string>
        {
            { "Field1", "Value1" },
            { "Field2", "Value2" },
            { "Field3", "Value3" }
        });

        var length = await _hashCache.GetLengthAsync(_hashKey);
        Assert.Equal(3, length);
        _logger.Log("✅ Hash length retrieved successfully.");
    }

    [Fact(DisplayName = "Should get all keys from hash")]
    public async Task GetAllKeysFromHash()
    {
        _logger.Log("🧩 Getting all keys from hash...");

        await _hashCache.SetAsync(_hashKey, new Dictionary<string, string>
        {
            { "Field1", "Value1" },
            { "Field2", "Value2" }
        });

        var keys = await _hashCache.GetKeysAsync(_hashKey);
        Assert.Equal(2, keys.Count);
        Assert.Contains("Field1", keys);
        Assert.Contains("Field2", keys);
        _logger.Log("✅ All keys from hash retrieved successfully.");
    }

    [Fact(DisplayName = "Should get all values from hash")]
    public async Task GetAllValuesFromHash()
    {
        _logger.Log("🧩 Getting all values from hash...");

        await _hashCache.SetAsync(_hashKey, new Dictionary<string, string>
        {
            { "Field1", "Value1" },
            { "Field2", "Value2" }
        });

        var values = await _hashCache.GetValuesAsync(_hashKey);
        Assert.Equal(2, values.Count);
        Assert.Contains("Value1", values);
        Assert.Contains("Value2", values);
        _logger.Log("✅ All values from hash retrieved successfully.");
    }

    [Fact(DisplayName = "Should increment field in hash")]
    public async Task IncrementFieldInHash()
    {
        _logger.Log("🧩 Incrementing field in hash...");

        await _hashCache.SetAsync(_hashKey, "Counter", "0");
        var newValue = await _hashCache.IncrementAsync(_hashKey, "Counter");

        Assert.Equal(1, newValue);
        var value = await _hashCache.GetAsync(_hashKey, "Counter");
        Assert.Equal("1", value);
        _logger.Log("✅ Field in hash incremented successfully.");
    }

    [Fact(DisplayName = "Should increment field by amount in hash")]
    public async Task IncrementFieldByAmountInHash()
    {
        _logger.Log("🧩 Incrementing field by amount in hash...");

        await _hashCache.SetAsync(_hashKey, "Counter", "5");
        var newValue = await _hashCache.IncrementByAsync(_hashKey, "Counter", 10);

        Assert.Equal(15, newValue);
        var value = await _hashCache.GetAsync(_hashKey, "Counter");
        Assert.Equal("15", value);
        _logger.Log("✅ Field in hash incremented by amount successfully.");
    }

    [Fact(DisplayName = "Should decrement field in hash")]
    public async Task DecrementFieldInHash()
    {
        _logger.Log("🧩 Decrementing field in hash...");

        await _hashCache.SetAsync(_hashKey, "Counter", "10");
        var newValue = await _hashCache.DecrementAsync(_hashKey, "Counter");

        Assert.Equal(9, newValue);
        var value = await _hashCache.GetAsync(_hashKey, "Counter");
        Assert.Equal("9", value);
        _logger.Log("✅ Field in hash decremented successfully.");
    }

    [Fact(DisplayName = "Should decrement field by amount in hash")]
    public async Task DecrementFieldByAmountInHash()
    {
        _logger.Log("🧩 Decrementing field by amount in hash...");

        await _hashCache.SetAsync(_hashKey, "Counter", "20");
        var newValue = await _hashCache.DecrementByAsync(_hashKey, "Counter", 5);

        Assert.Equal(15, newValue);
        var value = await _hashCache.GetAsync(_hashKey, "Counter");
        Assert.Equal("15", value);
        _logger.Log("✅ Field in hash decremented by amount successfully.");
    }

    [Fact(DisplayName = "Should get multiple fields from hash")]
    public async Task GetMultipleFieldsFromHash()
    {
        _logger.Log("🧩 Getting multiple fields from hash...");

        await _hashCache.SetAsync(_hashKey, new Dictionary<string, string>
        {
            { "Field1", "Value1" },
            { "Field2", "Value2" },
            { "Field3", "Value3" }
        });

        var values = await _hashCache.GetAsync(_hashKey, ["Field1", "Field3"]);
        Assert.Equal(2, values.Count);
        Assert.Equal("Value1", values["Field1"]);
        Assert.Equal("Value3", values["Field3"]);
        Assert.False(values.ContainsKey("Field2"));
        _logger.Log("✅ Multiple fields from hash retrieved successfully.");
    }

    [Fact(DisplayName = "Should clear hash")]
    public async Task ClearHash()
    {
        _logger.Log("🧩 Clearing hash...");

        await _hashCache.SetAsync(_hashKey, new Dictionary<string, string>
        {
            { "Field1", "Value1" },
            { "Field2", "Value2" }
        });

        await _hashCache.ClearAsync(_hashKey);
        var length = await _hashCache.GetLengthAsync(_hashKey);

        Assert.Equal(0, length);
        _logger.Log("✅ Hash cleared successfully.");
    }

    [Fact(DisplayName = "Should use scan to get fields")]
    public async Task ScanToGetFields()
    {
        _logger.Log("🧩 Using scan to get fields...");

        // Add many fields
        for (int i = 0; i < 50; i++)
        {
            await _hashCache.SetAsync(_hashKey, $"Field{i}", $"Value{i}");
        }

        var fields = await _hashCache.ScanAsync(_hashKey, "Field*", 10);
        Assert.True(fields.Count > 0);
        _logger.Log("✅ Scan to get fields completed successfully.");
    }
}
