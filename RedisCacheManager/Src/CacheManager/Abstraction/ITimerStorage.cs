namespace CacheManager.Abstraction;

public class TimerInfo
{
    public string Id { get; set; } = string.Empty;

    public DateTime FireAt { get; set; }

    public MessageEnvelope Message { get; set; } = null!;
}

public interface ITimerStorage
{
    Task SaveAsync(IEnumerable<TimerInfo> timers, CancellationToken token = default);

    Task<IEnumerable<TimerInfo>> LoadAsync(CancellationToken cancellationToken = default);
}