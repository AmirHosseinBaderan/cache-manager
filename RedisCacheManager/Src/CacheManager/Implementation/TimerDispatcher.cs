using Timer = System.Timers.Timer;

namespace CacheManager.Implementation;

internal class TimerDispatcher : ITimerDispatcher
{
    public void Run(string key, TimeSpan timeSpan, Action<string> onElapsed)
    {
        Timer timer = new(timeSpan)
        {
            AutoReset = false,
            Enabled = true,
        };
        timer.Elapsed += (sender, args) => onElapsed(key);
    }
}