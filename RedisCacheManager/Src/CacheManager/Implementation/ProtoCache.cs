using CacheManager.Abstraction;
using CacheManager.Configuration;

namespace CacheManager.Implementation;

public class ProtoCache(ICacheBase cacheBase) : IProtoCache
{
    public async Task<TModel?> GetItemAsync<TModel>(string key) where TModel : IMessage<TModel>, new()
    {
        try
        {
            RedisValue value = await cacheBase.GetItemAsync(key);
            return value.IsNullOrEmpty
                ? default
                : ((byte[])value!).Deserialize<TModel>();
        }
        catch
        {
            return default;
        }
    }

    public async Task<TModel?> GetOrSetItemAsync<TModel>(string key, Func<Task<TModel>> func,
        Func<RedisValue, bool>? setIf = null) where TModel : IMessage<TModel>, new()
    {
        try
        {
            RedisValue value = await cacheBase.GetOrSetItemAsync(key, async () =>
            {
                TModel? res = await func();
                return res is null
                    ? RedisValue.Null
                    : (RedisValue)res.Serialize();
            }, setIf);
            return value.IsNullOrEmpty
                ? await func()
                : ((byte[])value!).Deserialize<TModel>();
        }
        catch
        {
            return await func();
        }
    }

    public async Task<TModel?> GetOrSetItemAsync<TModel>(string key, CacheDuration cacheDuration,
        Func<Task<TModel>> func, Func<RedisValue, bool>? setIf = null) where TModel : IMessage<TModel>, new()
    {
        try
        {
            RedisValue value = await cacheBase.GetOrSetItemAsync(key, cacheDuration, async () =>
            {
                TModel? res = await func();
                return res is null
                    ? RedisValue.Null
                    : (RedisValue)res.Serialize();
            }, setIf);
            return value.IsNullOrEmpty
                ? await func()
                : JsonConvert.DeserializeObject<TModel>(value.ToString());
        }
        catch
        {
            return await func();
        }
    }

    public async Task<TModel?> GetOrSetItemAsync<TModel>(string key, Func<TModel> action,
        Func<RedisValue, bool>? setIf = null) where TModel : IMessage<TModel>, new()
    {
        try
        {
            RedisValue value = await cacheBase.GetOrSetItemAsync(key, () =>
            {
                TModel? res = action();
                return res is null
                    ? RedisValue.Null
                    : (RedisValue)res.Serialize();
            }, setIf);
            return value.IsNullOrEmpty
                ? action()
                : ((byte[])value!).Deserialize<TModel>();
        }
        catch
        {
            return action();
        }
    }

    public async Task<TModel?> GetOrSetItemAsync<TModel>(string key, CacheDuration cacheDuration, Func<TModel> action,
        Func<RedisValue, bool>? setIf = null) where TModel : IMessage<TModel>, new()
    {
        try
        {
            RedisValue value = await cacheBase.GetOrSetItemAsync(key, cacheDuration, () =>
            {
                TModel? res = action();
                return res is null
                    ? RedisValue.Null
                    : (RedisValue)res.Serialize();
            }, setIf);
            return value.IsNullOrEmpty
                ? action()
                : JsonConvert.DeserializeObject<TModel>(value.ToString());
        }
        catch
        {
            return action();
        }
    }

    public async Task RemoveItemAsync(string key)
        => await cacheBase.RemoveItemAsync(key);

    public async Task<TModel?> SetItemAsync<TModel>(string key, TModel? obj) where TModel : IMessage<TModel>, new()
        => await SetItemAsync(key, obj, cacheTime: null);

    public async Task<TModel?> SetItemAsync<TModel>(string key, TModel? obj, CacheDuration duration)
        where TModel : IMessage<TModel>, new()
        => await SetItemAsync(key, obj, duration.ToTimeSpan());

    public async Task<TModel?> SetItemAsync<TModel>(string key, TModel? obj, TimeSpan? cacheTime)
        where TModel : IMessage<TModel>, new()
    {
        try
        {
            if (obj is null)
                return obj;

            await cacheBase.SetItemAsync(key, obj.Serialize(), cacheTime);
            return obj;
        }
        catch
        {
            return obj;
        }
    }

    public async Task<TModel?> SetItemIfAsync<TModel>(bool condition, string key, TModel? obj)
        where TModel : IMessage<TModel>, new()
        => condition
            ? await SetItemAsync(key, obj, cacheTime: null)
            : obj;

    public async Task<TModel?> SetItemIfAsync<TModel>(bool condition, string key, TModel? obj, CacheDuration duration)
        where TModel : IMessage<TModel>, new()
        => condition
            ? await SetItemAsync(key, obj, cacheTime: duration.ToTimeSpan())
            : obj;
}