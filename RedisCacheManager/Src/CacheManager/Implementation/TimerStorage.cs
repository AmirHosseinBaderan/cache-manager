namespace CacheManager.Implementation;

internal class TimerStorage(ILogger<TimerStorage> logger) : ITimerStorage
{
    private const string FilePath = "timers.json";

    public async Task SaveAsync(IEnumerable<TimerInfo> timers, CancellationToken token = default)
    {
        try
        {
            var json = JsonConvert.SerializeObject(timers);
            await File.WriteAllTextAsync(FilePath, json, token);
            logger.LogInformation("Timers Saved");
        }
        catch (Exception e)
        {
            logger.LogError("Exception in Save timers {@Exception}", e);
        }
    }

    public async Task<IEnumerable<TimerInfo>> LoadAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (!File.Exists(FilePath))
                return [];
            var json = await File.ReadAllTextAsync(FilePath, cancellationToken);
            return JsonConvert.DeserializeObject<IEnumerable<TimerInfo>>(json) ?? [];
        }
        catch (Exception e)
        {
            logger.LogError("Failed to restore timers {@Exception}", e);
            return [];
        }
    }
}