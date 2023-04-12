using System;
using System.Threading;

namespace MultiplayerMod.Core.Logging;

public class Logger {

    private readonly string group;

    public LogLevel LogLevel { get; set; } = LogLevel.Info;

    public Logger(Type type) {
        group = type.Name;
    }

    public void Info(string message) => Log(LogLevel.Info, message);
    public void Warning(string message) => Log(LogLevel.Warning, message);
    public void Error(string message) => Log(LogLevel.Error, message);
    public void Debug(string message) => Log(LogLevel.Debug, message);
    public void Trace(string message) => Log(LogLevel.Trace, message);
    public void Trace(Func<string> messageProvider) => Log(LogLevel.Trace, messageProvider());

    private void Log(LogLevel level, string message) {
        if (level < LogLevel) return;
        var timestamp = System.DateTime.UtcNow.ToString("HH:mm:ss.fff");
        var threadId = Thread.CurrentThread.ManagedThreadId;
        var levelId = level.ToString().ToUpper();
        Console.WriteLine($"[{timestamp}] [{threadId}] [{levelId}] [{group}] {message}");
    }

}
