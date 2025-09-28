namespace CacheManager.Abstraction;

public interface IProducer
{
    Task PushAsync<T>(T model,TimeSpan delay, CancellationToken token = default);
}

public class MessageEnvelope
{
    public string Type { get; set; } = null!;
    public string Data { get; set; } = null!;
}