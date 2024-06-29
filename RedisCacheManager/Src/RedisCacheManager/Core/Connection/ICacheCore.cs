﻿using StackExchange.Redis;

namespace RedisCacheManager.Core;

internal interface ICacheCore : IAsyncDisposable
{
    /// <summary>
    /// Create connection with redis server use default configuration
    /// </summary>
    /// <returns>Instance of <see cref="ConnectionMultiplexer"/></returns>
    Task<ConnectionMultiplexer?> ConnectAsync();

    /// <summary>
    /// Create connection with redis server use connection string
    /// </summary>
    /// <param name="connectionString">Connection string for connect to redis</param>
    /// <returns>Instance of <see cref="ConnectionMultiplexer"/> </returns>
    Task<ConnectionMultiplexer?> ConnectAsync(string connectionString);
}
