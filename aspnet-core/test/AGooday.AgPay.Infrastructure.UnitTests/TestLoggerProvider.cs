using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace AGooday.AgPay.Infrastructure.UnitTests
{
    public class TestLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentQueue<string> _logs;

        public TestLoggerProvider()
        {
            _logs = new ConcurrentQueue<string>();
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new TestLogger(_logs);
        }

        public void Dispose()
        {
            // 可选的清理操作
        }

        public string[] GetLogs()
        {
            return _logs.ToArray();
        }
    }

    public class TestLogger : ILogger
    {
        private readonly ConcurrentQueue<string> _logs;

        public TestLogger(ConcurrentQueue<string> logs)
        {
            _logs = logs;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return NullScope.Instance;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _logs.Enqueue(formatter(state, exception));
        }
    }

    internal class NullScope : IDisposable
    {
        public static NullScope Instance { get; } = new NullScope();

        public void Dispose()
        {
            // 不执行任何操作
        }
    }
}
