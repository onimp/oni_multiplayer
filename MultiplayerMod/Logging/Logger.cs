using System;
using System.Threading;

namespace MultiplayerMod.Logging {

    public class Logger {

        private readonly string group;

        public Logger(string group) {
            this.group = group;
        }

        public void Info(string message) => Log(LogLevel.Info, message);
        public void Warning(string message) => Log(LogLevel.Warning, message);
        public void Error(string message) => Log(LogLevel.Error, message);
        public void Debug(string message) => Log(LogLevel.Debug, message);
        public void Trace(string message) => Log(LogLevel.Trace, message);

        private void Log(LogLevel level, string message) {
            var timestamp = System.DateTime.UtcNow.ToString("HH:mm:ss.fff");
            var threadId = Thread.CurrentThread.ManagedThreadId;
            var levelId = level.ToString().ToUpper();
            Console.WriteLine($"[{timestamp}] [{threadId}] [{levelId}] [{group}] {message}");
        }

    }

}
