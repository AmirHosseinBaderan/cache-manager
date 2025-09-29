using System.Reflection;
using CacheManager.Abstraction;
using CacheManager.Core;
using CacheManager.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace CacheManager.Configuration;

public static class Configuration
{
    public static IServiceCollection AddRedisCacheManager(this IServiceCollection services, Func<CacheConfigs> config)
    {
        Configs.CacheConfigs = config();

        // Add cache core services
        services.AddScoped<ICacheCore, CacheCore>();

        services.AddScoped<ICacheDb, CacheDb>();
        services.AddScoped<ICacheBase, CacheBase>();

        services.AddScoped<IJsonCache, JsonCache>();
        services.AddScoped<IProtoCache, ProtoCache>();

        return services;
    }

    public static IServiceCollection AddRedisCacheManagerQueue(
        this IServiceCollection services, Assembly? assembly = null)
    {
        assembly ??= Assembly.GetExecutingAssembly();

        // Add Producer & Dispatcher
        services.AddScoped<IProducer, Producer>();
        services.AddScoped<RedisDispatcher>();
        services.AddSingleton<ITimerDispatcher, TimerDispatcher>();
        
        // Find all classes implementing IRedisConsumer<T>
        var consumerTypes = assembly.GetTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false })
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRedisConsumer<>))
                .Select(i => new { Service = i, Implementation = t }))
            .ToList();

        foreach (var c in consumerTypes)
            services.AddScoped(c.Service, c.Implementation);

        return services;
    }
}