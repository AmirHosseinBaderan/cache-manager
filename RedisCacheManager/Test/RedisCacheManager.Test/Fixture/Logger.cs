using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace RedisCacheManager.Test.Fixtures;

public class XUnitLoggerProvider(ITestOutputHelper output) : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
        => new XUnitLogger(output, categoryName);

    public void Dispose() { }

    private class XUnitLogger(ITestOutputHelper output, string categoryName) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId,
            TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            output.WriteLine($"[{DateTime.Now:HH:mm:ss}] [{logLevel}] {categoryName}: {formatter(state, exception)}");

            if (exception != null)
                output.WriteLine(exception.ToString());
        }
    }
}