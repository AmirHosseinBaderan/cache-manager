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

        // Find all classes implementing IRedisConsumer<T>
        var consumerTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRedisConsumer<>))
                .Select(i => new { Service = i, Implementation = t }))
            .ToList();

        foreach (var c in consumerTypes)
            services.AddScoped(c.Service, c.Implementation);

        // Add Producer & Dispatcher
        services.AddSingleton<Producer>();
        services.AddHostedService<RedisDispatcher>();

        return services;
    }
}