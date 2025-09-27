namespace CacheManager.Abstraction;

public interface IRedisConsumer<T>
{
    Task ExecuteAsync(T mode, CancellationToken token = default);
}

