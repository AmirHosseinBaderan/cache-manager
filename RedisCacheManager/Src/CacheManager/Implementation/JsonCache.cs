using CacheManager.Abstraction;
using CacheManager.Configuration;

namespace CacheManager.Implementation;

internal class JsonCache(ICacheBase cacheBase) : IJsonCache
{
    public async Task<TModel?> GetItemAsync<TModel>(string key)
    {
        try
        {
            RedisValue value = await cacheBase.GetItemAsync(key);
            if (value.IsNullOrEmpty)
                return default;
            return JsonConvert.DeserializeObject<TModel>(value.ToString());
        }
        catch
        {
            return default;
        }
    }

    public async Task<TModel?> GetOrSetItemAsync<TModel>(string key, Func<Task<TModel>> func,
        Func<RedisValue, bool>? setIf = null)
    {
        try
        {
            RedisValue value = await cacheBase.GetOrSetItemAsync(key, async () =>
            {
                TModel? res = await func();
                if (res is null)
                    return RedisValue.Null;
                string json = JsonConvert.SerializeObject(res, Configs.CacheConfigs.Formatting,
                    Configs.CacheConfigs.JsonSerializerSettings);
                return new(json);
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

    public async Task<TModel?> GetOrSetItemAsync<TModel>(string key, CacheDuration cacheDuration,
        Func<Task<TModel>> func, Func<RedisValue, bool>? setIf = null)
    {
        try
        {
            RedisValue value = await cacheBase.GetOrSetItemAsync(key, cacheDuration, async () =>
            {
                TModel? res = await func();
                if (res is null)
                    return RedisValue.Null;
                string json = JsonConvert.SerializeObject(res, Configs.CacheConfigs.Formatting,
                    Configs.CacheConfigs.JsonSerializerSettings);
                return new(json);
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
        Func<RedisValue, bool>? setIf = null)
    {
        try
        {
            RedisValue value = await cacheBase.GetOrSetItemAsync(key, () =>
            {
                TModel? res = action();
                if (res is null)
                    return RedisValue.Null;
                string json = JsonConvert.SerializeObject(res, Configs.CacheConfigs.Formatting,
                    Configs.CacheConfigs.JsonSerializerSettings);
                return new(json);
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

    public async Task<TModel?> GetOrSetItemAsync<TModel>(string key, CacheDuration cacheDuration, Func<TModel> action,
        Func<RedisValue, bool>? setIf = null)
    {
        try
        {
            RedisValue value = await cacheBase.GetOrSetItemAsync(key, cacheDuration, () =>
            {
                TModel? res = action();
                if (res is null)
                    return RedisValue.Null;
                string json = JsonConvert.SerializeObject(res, Configs.CacheConfigs.Formatting,
                    Configs.CacheConfigs.JsonSerializerSettings);
                return new(json);
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

    public async Task<TModel?> SetItemAsync<TModel>(string key, TModel? obj)
        => await SetItemAsync(key, obj, cacheTime: null);

    public async Task<TModel?> SetItemAsync<TModel>(string key, TModel? obj, CacheDuration duration)
        => await SetItemAsync(key, obj, duration.ToTimeSpan());

    public async Task<TModel?> SetItemAsync<TModel>(string key, TModel? obj, TimeSpan? cacheTime)
    {
        try
        {
            string json = JsonConvert.SerializeObject(obj, Configs.CacheConfigs.Formatting,
                Configs.CacheConfigs.JsonSerializerSettings);
            await cacheBase.SetItemAsync(key, new(json), cacheTime);
            return obj;
        }
        catch
        {
            return obj;
        }
    }

    public async Task<TModel?> SetItemIfAsync<TModel>(bool condition, string key, TModel? obj)
        => condition
            ? await SetItemAsync(key, obj, cacheTime: null)
            : obj;

    public async Task<TModel?> SetItemIfAsync<TModel>(bool condition, string key, TModel? obj, CacheDuration duration)
        => condition
            ? await SetItemAsync(key, obj, cacheTime: duration.ToTimeSpan())
            : obj;
}