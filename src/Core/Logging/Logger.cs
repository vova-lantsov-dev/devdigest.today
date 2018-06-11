using System;

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

    public class Logger : ILogger
    {
        public void Write(LogLevel level, string message, Exception ex = null)
        {
            
        }
    }
}