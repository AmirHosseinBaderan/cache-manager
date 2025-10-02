namespace CacheManager.Abstraction;

public interface IProducer
{
    Task PushAsync<T>(T model, TimeSpan delay, string? key = null, bool repeat = false,
        CancellationToken token = default);

    void Cancel(string key);
}

public class MessageEnvelope
{
    public string Type { get; set; } = null!;
    public string Data { get; set; } = null!;
}