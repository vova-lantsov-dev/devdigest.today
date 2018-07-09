using System;
using Serilog.Core;

namespace Core.Logging
{
    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error,
        FatalError
    }

    public interface ILogger
    {
        void Write(LogLevel level, string message, Exception ex = null);
    }

    public class SimpleLogger : ILogger
    {
        public void Write(LogLevel level, string message, Exception ex = null)
        {
            Console.WriteLine($"[{level}]: {message}");
        }
    }

    public class SerilogLoggerWrapper : ILogger
    {
        private readonly Logger _log;

        public SerilogLoggerWrapper(Logger log) => _log = log;

        public void Write(LogLevel level, string message, Exception ex = null)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    _log.Debug(message);
                    break;
                case LogLevel.Info:
                    _log.Information(message);
                    break;
                case LogLevel.Warning:
                    _log.Warning(message);
                    break;
                case LogLevel.Error:
                    _log.Error(ex, message);
                    break;
                case LogLevel.FatalError:
                    _log.Fatal(ex, message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }
    }
}