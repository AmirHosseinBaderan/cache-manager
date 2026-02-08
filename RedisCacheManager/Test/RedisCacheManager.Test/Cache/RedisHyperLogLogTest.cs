using CacheManager.Core.Connection;
using CacheManager.Implementation;
using FluentAssertions;
using Xunit.Abstractions;

namespace RedisCacheManager.Test.Cache;

public class RedisHyperLogLogTest : IClassFixture<RedisCacheFixture>
{
    private readonly RedisHyperLogLog _hyperLogLog;
    private readonly ITestOutputHelper _output;

    public RedisHyperLogLogTest(RedisCacheFixture fixture, ITestOutputHelper output)
    {
        _output = output;
        var core = new CacheCore(fixture.Configuration);
        _hyperLogLog = new RedisHyperLogLog(core);
    }

    [Fact]
    public async Task PfAdd_ShouldAddValues()
    {
        // Arrange
        var key = "test:hll:1";
        await _hyperLogLog.PfAddAsync(key, "value1", "value2", "value3");

        // Act
        var count = await _hyperLogLog.PfCountAsync(key);

        // Assert
        count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task PfCount_ShouldReturnCount()
    {
        // Arrange
        var key = "test:hll:2";
        await _hyperLogLog.PfAddAsync(key, "unique1", "unique2", "unique1"); // duplicate

        // Act
        var count = await _hyperLogLog.PfCountAsync(key);

        // Assert
        count.Should().Be(2); // Unique count
    }

    [Fact]
    public async Task PfMerge_ShouldMergeMultipleKeys()
    {
        // Arrange
        var key1 = "test:hll:merge1";
        var key2 = "test:hll:merge2";
        var destKey = "test:hll:merged";

        await _hyperLogLog.PfAddAsync(key1, "a", "b", "c");
        await _hyperLogLog.PfAddAsync(key2, "d", "e", "f");

        // Act
        await _hyperLogLog.PfMergeAsync(destKey, key1, key2);
        var count = await _hyperLogLog.PfCountAsync(destKey);

        // Assert
        count.Should().Be(6);
    }
}
