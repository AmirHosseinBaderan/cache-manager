namespace CacheManager.Abstraction;

public interface IRedisLuaScript
{
    /// <summary>
    /// Execute a Lua script
    /// </summary>
    /// <typeparam name="T">Return type</typeparam>
    /// <param name="script">Lua script content</param>
    /// <param name="keys">Keys to pass to script</param>
    /// <param name="values">Values to pass to script</param>
    /// <returns>Script result</returns>
    Task<T> EvalAsync<T>(string script, IEnumerable<string> keys, IEnumerable<object> values);

    /// <summary>
    /// Execute a Lua script with SHA1 hash (cached version)
    /// </summary>
    /// <typeparam name="T">Return type</typeparam>
    /// <param name="sha1">SHA1 hash of the script</param>
    /// <param name="keys">Keys to pass to script</param>
    /// <param name="values">Values to pass to script</param>
    /// <returns>Script result</returns>
    Task<T> EvalShaAsync<T>(string sha1, IEnumerable<string> keys, IEnumerable<object> values);

    /// <summary>
    /// Load a Lua script and return its SHA1 hash
    /// </summary>
    /// <param name="script">Lua script content</param>
    /// <returns>SHA1 hash of the script</returns>
    Task<string> ScriptLoadAsync(string script);

    /// <summary>
    /// Check if a script exists by its SHA1 hash
    /// </summary>
    /// <param name="sha1">SHA1 hash to check</param>
    /// <returns>True if script exists</returns>
    Task<bool> ScriptExistsAsync(string sha1);

    /// <summary>
    /// Remove all scripts from cache
    /// </summary>
    Task ScriptFlushAsync();
}
