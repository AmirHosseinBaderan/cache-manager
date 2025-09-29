using Timer = System.Timers.Timer;

namespace CacheManager.Implementation;

internal class TimerDispatcher : ITimerDispatcher
{
    private readonly Dictionary<string, Timer> _timers = new();

    public void Run(string key, TimeSpan timeSpan, Action<string> onElapsed)
    {
        Cancel(key); // ensure old timer is cleared if exists

        var timer = new Timer(timeSpan.TotalMilliseconds)
        {
            AutoReset = false,
            Enabled = true
        };

        timer.Elapsed += (sender, args) =>
        {
            onElapsed(key);
            Cancel(key); // cleanup after elapsed
        };

        _timers[key] = timer;
    }

    public void Cancel(string key)
    {
        if (!_timers.TryGetValue(key, out var timer)) return;
        timer.Stop();
        timer.Dispose();
        _timers.Remove(key);
    }
}