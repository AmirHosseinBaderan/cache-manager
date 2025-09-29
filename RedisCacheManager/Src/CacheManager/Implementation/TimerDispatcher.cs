using Timer = System.Timers.Timer;

namespace CacheManager.Implementation;

internal class TimerDispatcher : ITimerDispatcher
{
    private readonly Dictionary<string, Timer> _timers = new();
    private readonly Dictionary<string, Action<string>> _cancelCallbacks = new();

    public void Run(string key, TimeSpan timeSpan, Action<string> onElapsed, Action<string>? onCanceled = null)
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
            Cancel(key, triggerCallback: false); // cleanup, don’t call cancel callback here
        };

        _timers[key] = timer;

        if (onCanceled != null)
        {
            _cancelCallbacks[key] = onCanceled;
        }
    }

    public void Cancel(string key, bool triggerCallback = true)
    {
        if (!_timers.TryGetValue(key, out var timer)) return;
        timer.Stop();
        timer.Dispose();
        _timers.Remove(key);

        if (!triggerCallback || !_cancelCallbacks.TryGetValue(key, out var callback)) return;
        callback(key);
        _cancelCallbacks.Remove(key);
    }
}