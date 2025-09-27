namespace CacheManager.Configuration;

public record CacheConfigs(
    string ConnectionString,
    int Instance,
    JsonSerializerSettings? JsonSerializerSettings,
    Formatting Formatting = Formatting.None,
    string? QueueName = null);

public class Configs
{
    public static CacheConfigs CacheConfigs { get; set; } = null!;
}