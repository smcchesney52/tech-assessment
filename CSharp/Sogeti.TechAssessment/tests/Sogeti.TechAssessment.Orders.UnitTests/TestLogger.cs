using Microsoft.Extensions.Logging;

namespace Sogeti.TechAssessment.Orders.UnitTests
{
    public class TestLogger<T> : ILogger<T>
    {
        private readonly List<string> _entries = new();
        
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            _entries.Add(formatter(state, exception));
        }
    }
}