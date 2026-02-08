using StackExchange.Redis;
using CacheManager.Abstraction;
using CacheManager.Core;

namespace CacheManager.Implementation;

public class RedisLuaScript : IRedisLuaScript
{
    private readonly ICacheCore _cacheCore;

    public RedisLuaScript(ICacheCore cacheCore)
    {
        _cacheCore = cacheCore;
    }

    public async Task<T> EvalAsync<T>(string script, IEnumerable<string> keys, IEnumerable<object> values)
    {
        var db = _cacheCore.GetDatabase();
        var redisKeys = keys.Select(k => (RedisKey)k).ToArray();
        var redisValues = values.Select(v => (RedisValue)v).ToArray();

        var result = await db.ScriptEvaluateAsync(script, redisKeys, redisValues);

        return ConvertResult<T>(result);
    }

    public async Task<T> EvalShaAsync<T>(string sha1, IEnumerable<string> keys, IEnumerable<object> values)
    {
        var db = _cacheCore.GetDatabase();
        var redisKeys = keys.Select(k => (RedisKey)k).ToArray();
        var redisValues = values.Select(v => (RedisValue)v).ToArray();

        var result = await db.ScriptEvaluateAsync(sha1, redisKeys, redisValues);

        return ConvertResult<T>(result);
    }

    public async Task<string> ScriptLoadAsync(string script)
    {
        var db = _cacheCore.GetDatabase();
        var result = await db.ScriptLoadAsync(script);
        return result;
    }

    public async Task<bool> ScriptExistsAsync(string sha1)
    {
        var db = _cacheCore.GetDatabase();
        var result = await db.ScriptExistsAsync(sha1);
        return result;
    }

    public async Task ScriptFlushAsync()
    {
        var db = _cacheCore.GetDatabase();
        await db.ScriptFlushAsync();
    }

    private static T ConvertResult<T>(RedisResult result)
    {
        if (result.IsNull)
        {
            return default!;
        }

        var type = typeof(T);

        if (type == typeof(long))
        {
            return (T)(object)(long)result;
        }

        if (type == typeof(double))
        {
            return (T)(object)(double)result;
        }

        if (type == typeof(string))
        {
            return (T)(object)result.ToString()!;
        }

        if (type == typeof(bool))
        {
            return (T)(object)(bool)result;
        }

        if (type == typeof(RedisResult[]))
        {
            return (T)(object)result.ToArray()!;
        }

        if (type == typeof(byte[]))
        {
            return (T)(object)(byte[])result!;
        }

        if (type == typeof(string[]))
        {
            return (T)(object)result.ToStringArray()!;
        }

        if (type == typeof(long[]))
        {
            return (T)(object)result.ToLongArray()!;
        }

        return (T)result!;
    }
}
