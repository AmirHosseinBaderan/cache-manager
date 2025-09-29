namespace CacheManager.Abstraction;

internal interface ITimerDispatcher
{
    void Run(string key, TimeSpan timeSpan, Action<string> onElapsed,Action<string>? onCanceled = null);

    void Cancel(string key,bool triggerCallback = true);
}