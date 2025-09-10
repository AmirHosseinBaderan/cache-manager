using CacheManager.Configuration;

namespace CacheManager.Abstraction;

public interface IProtoCache
{
    Task<TModel?> GetItemAsync<TModel>(string key) where TModel : IMessage<TModel>, new();

    Task<TModel?> SetItemAsync<TModel>(string key, TModel? obj) where TModel : IMessage<TModel>, new();

    Task<TModel?> SetItemAsync<TModel>(string key, TModel? obj, CacheDuration duration) where TModel : IMessage<TModel>, new();

    Task<TModel?> SetItemIfAsync<TModel>(bool condition, string key, TModel? obj) where TModel : IMessage<TModel>, new();

    Task<TModel?> SetItemIfAsync<TModel>(bool condition, string key, TModel? obj, CacheDuration duration) where TModel : IMessage<TModel>, new();

    Task RemoveItemAsync(string key);

    Task<TModel?> GetOrSetItemAsync<TModel>(string key, Func<Task<TModel>> action,Func<RedisValue,bool>? setIf = null) where TModel : IMessage<TModel>, new();

    Task<TModel?> GetOrSetItemAsync<TModel>(string key, Func<TModel> action,Func<RedisValue,bool>? setIf = null) where TModel : IMessage<TModel>, new();

    Task<TModel?> GetOrSetItemAsync<TModel>(string key, CacheDuration cacheDuration, Func<Task<TModel>> action,Func<RedisValue,bool>? setIf = null) where TModel : IMessage<TModel>, new();

    Task<TModel?> GetOrSetItemAsync<TModel>(string key, CacheDuration cacheDuration, Func<TModel> action,Func<RedisValue,bool>? setIf = null) where TModel : IMessage<TModel>, new();

    Task<TModel?> SetItemAsync<TModel>(string key, TModel obj, TimeSpan? cacheTime) where TModel : IMessage<TModel>, new();
}
