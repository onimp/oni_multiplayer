using System;
using System.Threading;

namespace MultiplayerMod.Core.Logging;

public class Logger {

    private readonly string group;

    public LogLevel Level { get; set; } = LogLevel.Info;

    public Logger(Type type) {
        group = type.Name;

        var level = Environment.GetEnvironmentVariable("MULTIPLAYERMOD.LOGGING.LEVEL");
        if (level != null && Enum.TryParse<LogLevel>(level, true, out var globalLevel))
            Level = globalLevel;
        level = Environment.GetEnvironmentVariable($"MULTIPLAYERMOD.LOGGING.{type.Name.ToUpper()}");
        if (level != null && Enum.TryParse<LogLevel>(level, true, out var specificLevel))
            Level = specificLevel;
    }

    public void Info(string message) => Log(LogLevel.Info, () => message);
    public void Info(Func<string> message) => Log(LogLevel.Info, message);

    public void Warning(string message) => Log(LogLevel.Warning, () => message);
    public void Warning(Func<string> message) => Log(LogLevel.Warning, message);

    public void Error(string message) => Log(LogLevel.Error, () => message);
    public void Error(Func<string> message) => Log(LogLevel.Error, message);

    public void Debug(string message) => Log(LogLevel.Debug, () => message);
    public void Debug(Func<string> message) => Log(LogLevel.Debug, message);

    public void Trace(string message) => Log(LogLevel.Trace, () => message);
    public void Trace(Func<string> message) => Log(LogLevel.Trace, message);

    private void Log(LogLevel level, Func<string> message) {
        if (level < Level)
            return;

        var timestamp = System.DateTime.UtcNow.ToString("HH:mm:ss.fff");
        var threadId = Thread.CurrentThread.ManagedThreadId;
        var levelId = level.ToString().ToUpper();
        Console.WriteLine($"[{timestamp}] [{threadId}] [{levelId}] [{group}] {message()}");
    }

}
