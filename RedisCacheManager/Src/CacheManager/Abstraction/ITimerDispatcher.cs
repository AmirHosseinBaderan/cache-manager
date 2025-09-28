namespace CacheManager.Abstraction;

internal interface ITimerDispatcher
{
    void Run(string key, TimeSpan timeSpan, Action<string> onElapsed);
}