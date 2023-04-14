using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using MultiplayerMod.Core.Extensions;

namespace MultiplayerMod.Core.Logging;

public static class LoggerFactory {

    private static readonly Dictionary<string, LogLevel> levels = new();
    private static readonly Regex Pattern = new Regex("(.*?)=(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    static LoggerFactory() {
        Environment.GetCommandLineArgs().ForEach(ParseLogConfiguration);
    }

    private static void ParseLogConfiguration(string arg) {
        var match = Pattern.Match(arg);
        if (!match.Success)
            return;

        var id = match.Groups[1].Value.ToLower();
        if (!id.StartsWith("log."))
            return;

        var levelName = match.Groups[2].Value;
        if (!Enum.TryParse<LogLevel>(levelName, true, out var level))
            return;

        var type = id.Substring(4);
        levels[type] = level;
    }

    public static Logger GetLogger<T>() => GetLogger(typeof(T));

    public static Logger GetLogger(Type type) {
        var logger = new Logger(type);
        if (levels.TryGetValue(type.Name.ToLower(), out var level))
            logger.Level = level;
        else if (levels.TryGetValue("level", out var global))
            logger.Level = global;
        return logger;
    }

}
