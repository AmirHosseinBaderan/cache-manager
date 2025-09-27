namespace CacheManager.Abstraction;

public interface IRedisConsumer<T>
{
    Task ExecuteAsync(T model, CancellationToken token = default);
}

