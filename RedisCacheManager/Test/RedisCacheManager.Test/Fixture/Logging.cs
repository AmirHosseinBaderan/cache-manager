using Xunit.Abstractions;

namespace CacheManager.Tests.Fixtures;

public class TestLogging(ITestOutputHelper outputHelper)
{
    public void Log(string message = "", string type = "Info", params object[] args)
    {
        var formattedMessage = args is { Length: > 0 }
            ? string.Format(message, args)
            : message;

        outputHelper.WriteLine($"[{DateTime.Now:HH:mm:ss} {type}] {formattedMessage}");
    }
}