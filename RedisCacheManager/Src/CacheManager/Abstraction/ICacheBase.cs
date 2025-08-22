﻿using CacheManager.Configuration;

namespace CacheManager.Abstraction;

public interface ICacheBase : IAsyncDisposable
{
    Task<RedisValue> GetItemAsync(string key);

    Task<RedisValue> SetItemAsync(string key, RedisValue? obj);

    Task RemoveItemAsync(string key);

    Task<RedisValue> GetOrSetItemAsync(string key, Func<Task<RedisValue>> action);

    Task<RedisValue> GetOrSetItemAsync(string key, Func<RedisValue> action);

    Task<RedisValue> GetOrSetItemAsync(string key, CacheDuration cacheDuration, Func<Task<RedisValue>> action);

    Task<RedisValue> GetOrSetItemAsync(string key, CacheDuration cacheDuration, Func<RedisValue> action);

    Task<RedisValue> SetItemAsync(string key, RedisValue? obj, TimeSpan? cacheTime);
}
