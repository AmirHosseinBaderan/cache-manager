using CacheManager.Configuration;

namespace CacheManager.Abstraction;

public interface IJsonCache
{
    Task<TModel?> GetItemAsync<TModel>(string key);

    Task<TModel?> SetItemAsync<TModel>(string key, TModel? obj);

    Task<TModel?> SetItemAsync<TModel>(string key, TModel? obj, CacheDuration duration);

    Task<TModel?> SetItemIfAsync<TModel>(bool condition, string key, TModel? obj);

    Task<TModel?> SetItemIfAsync<TModel>(bool condition, string key, TModel? obj, CacheDuration duration);

    Task RemoveItemAsync(string key);

    Task<TModel?> GetOrSetItemAsync<TModel>(string key, Func<Task<TModel>> action,
        Func<RedisValue, bool>? setIf = null);

    Task<TModel?> GetOrSetItemAsync<TModel>(string key, Func<TModel> action, Func<RedisValue, bool>? setIf = null);

    Task<TModel?> GetOrSetItemAsync<TModel>(string key, CacheDuration cacheDuration, Func<Task<TModel>> action,
        Func<RedisValue, bool>? setIf = null);

    Task<TModel?> GetOrSetItemAsync<TModel>(string key, CacheDuration cacheDuration, Func<TModel> action,
        Func<RedisValue, bool>? setIf = null);

    Task<TModel?> SetItemAsync<TModel>(string key, TModel obj, TimeSpan? cacheTime);
}