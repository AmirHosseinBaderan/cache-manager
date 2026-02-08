using CacheManager.Core.Connection;
using CacheManager.Implementation;
using FluentAssertions;
using Xunit.Abstractions;

namespace RedisCacheManager.Test.Cache;

public class RedisBitmapTest : IClassFixture<RedisCacheFixture>
{
    private readonly RedisBitmapCache _bitmapCache;
    private readonly ITestOutputHelper _output;

    public RedisBitmapTest(RedisCacheFixture fixture, ITestOutputHelper output)
    {
        _output = output;
        var core = new CacheCore(fixture.Configuration);
        _bitmapCache = new RedisBitmapCache(core);
    }

    [Fact]
    public async Task SetBit_ShouldSetBit()
    {
        // Arrange
        var key = "test:bitmap:1";

        // Act
        var oldBit = await _bitmapCache.SetBitAsync(key, 5, 1);

        // Assert
        oldBit.Should().Be(0); // Was not set before
    }

    [Fact]
    public async Task GetBit_ShouldReturnBit()
    {
        // Arrange
        var key = "test:bitmap:2";
        await _bitmapCache.SetBitAsync(key, 10, 1);

        // Act
        var bit = await _bitmapCache.GetBitAsync(key, 10);

        // Assert
        bit.Should().Be(1);
    }

    [Fact]
    public async Task BitCount_ShouldReturnCount()
    {
        // Arrange
        var key = "test:bitmap:3";
        await _bitmapCache.SetBitAsync(key, 0, 1);
        await _bitmapCache.SetBitAsync(key, 2, 1);
        await _bitmapCache.SetBitAsync(key, 4, 1);

        // Act
        var count = await _bitmapCache.BitCountAsync(key);

        // Assert
        count.Should().Be(3);
    }

    [Fact]
    public async Task BitCount_WithRange_ShouldReturnCountInRange()
    {
        // Arrange
        var key = "test:bitmap:4";
        await _bitmapCache.SetBitAsync(key, 0, 1);
        await _bitmapCache.SetBitAsync(key, 1, 1);
        await _bitmapCache.SetBitAsync(key, 2, 1);
        await _bitmapCache.SetBitAsync(key, 3, 0);

        // Act
        var count = await _bitmapCache.BitCountAsync(key, 0, 1);

        // Assert
        count.Should().Be(2);
    }

    [Fact]
    public async Task BitOp_ShouldPerformOperation()
    {
        // Arrange
        var key1 = "test:bitmap:5";
        var key2 = "test:bitmap:6";
        var destKey = "test:bitmap:and";

        await _bitmapCache.SetBitAsync(key1, 0, 1);
        await _bitmapCache.SetBitAsync(key1, 1, 1);
        await _bitmapCache.SetBitAsync(key1, 2, 1);

        await _bitmapCache.SetBitAsync(key2, 0, 1);
        await _bitmapCache.SetBitAsync(key2, 1, 0);
        await _bitmapCache.SetBitAsync(key2, 2, 1);

        // Act
        await _bitmapCache.BitOpAsync(BitOperation.AND, destKey, key1, key2);
        var count = await _bitmapCache.BitCountAsync(destKey);

        // Assert
        count.Should().Be(2); // Bit 0 and 2 are set in both
    }

    [Fact]
    public async Task BitPos_ShouldReturnPosition()
    {
        // Arrange
        var key = "test:bitmap:7";
        await _bitmapCache.SetBitAsync(key, 5, 0);
        await _bitmapCache.SetBitAsync(key, 6, 0);
        await _bitmapCache.SetBitAsync(key, 7, 1);

        // Act
        var pos = await _bitmapCache.BitPosAsync(key, 1);

        // Assert
        pos.Should().Be(7);
    }

    [Fact]
    public async Task GetString_ShouldReturnString()
    {
        // Arrange
        var key = "test:bitmap:8";
        await _bitmapCache.SetBitAsync(key, 0, 1);
        await _bitmapCache.SetBitAsync(key, 7, 1);

        // Act
        var str = await _bitmapCache.GetStringAsync(key);

        // Assert
        str.Should().NotBeNull();
        str.Length.Should().BeGreaterThan(0);
    }
}
