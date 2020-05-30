// using System;
// using Serilog.Events;
//
// namespace Core.Logging
// {
//     public interface ILogger
//     {
//         void Write(LogEventLevel level, string message, Exception ex = null);
//     }
//
//     public class SimpleLogger : ILogger
//     {
//         public void Write(LogEventLevel level, string message, Exception ex = null) =>
//             Console.WriteLine($"[{level}]: {message}");
//     }
//
//     public class SerilogLoggerWrapper : ILogger
//     {
//         private readonly Serilog.ILogger _log;
//
//         public SerilogLoggerWrapper(Serilog.ILogger log) => _log = log;
//
//         public void Write(LogEventLevel level, string message, Exception ex = null)
//         {
//             switch (level)
//             {
//                 case LogEventLevel.Debug:
//                     _log.Debug(message);
//                     break;
//                 case LogEventLevel.Information:
//                     _log.Information(message);
//                     break;
//                 case LogEventLevel.Warning:
//                     _log.Warning(message);
//                     break;
//                 case LogEventLevel.Error:
//                     _log.Error(ex, message);
//                     break;
//                 case LogEventLevel.Fatal:
//                     _log.Fatal(ex, message);
//                     break;
//                 default:
//                     throw new ArgumentOutOfRangeException(nameof(level), level, null);
//             }
//         }
//     }
// }