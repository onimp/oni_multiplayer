#if TCP_LOGGING

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Events;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.ModRuntime.StaticCompatibility;
using MultiplayerMod.Multiplayer;
using MultiplayerMod.Multiplayer.CoreOperations.Events;
using MultiplayerMod.Multiplayer.Players;
using Newtonsoft.Json;

namespace MultiplayerMod.Core.Logging;

[Dependency, UsedImplicitly]
public class TcpLogger {

    private class ExtendedMessageData : MessageData {
        public bool SetProperties { get; set; }
    }

    private class MessageData {
        public long Sequence { get; set; }
        public System.DateTime Time { get; set; }
        public int Level { get; set; }
        public string LevelName { get; set; } = null!;
        public string PlayerName { get; set; } = null!;
        public string Mode { get; set; } = null!;
        public string[]? StackTrace { get; set; }
        public string Message { get; set; } = null!;
    }

    private static readonly TcpClient client = new();
    private static StreamWriter writer = null!;
    private static readonly BlockingCollection<MessageData> messages = new();
    private static readonly Thread worker = new(Worker);
    private static bool started;
    private static long sequence;

    public TcpLogger(EventDispatcher events, MultiplayerGame multiplayer) {
        events.Subscribe<RuntimeReadyEvent>(_ => {
            worker.Start();
            started = true;
        });
        events.Subscribe<GameStartedEvent>(_ => {
            if (multiplayer.Mode == MultiplayerMode.Host)
                SetSession(SaveGame.Instance.BaseName);
        });
    }

    public static void Log(string message, string[]? stackTrace = null)
        => Log(System.DateTime.Now, LogLevel.Trace, message, stackTrace);

    public static void Log(System.DateTime time, LogLevel level, string message, string[]? stackTrace = null) {
        if (!started)
            return;

        var data = new MessageData {
            Sequence = sequence++,
            Time = time,
            Level = (int) level,
            LevelName = level.ToString(),
            PlayerName = Dependencies.Get<IPlayerProfileProvider>().GetPlayerProfile().PlayerName,
            Mode = Dependencies.Get<ExecutionLevelManager>().LevelIsActive(ExecutionLevel.Multiplayer)
                ? Dependencies.Get<MultiplayerGame>().Mode.ToString()
                : "Single",
            Message = message,
            StackTrace = stackTrace
        };
        messages.Add(data);
    }

    private static void SetSession(string name) => messages.Add(
        new ExtendedMessageData {
            SetProperties = true,
            Message = $"session:{name}"
        }
    );

    private static void Worker() {
        while (true) {
            EnsureConnected();
            var message = messages.Take();
            try {
                writer.WriteLine(JsonConvert.SerializeObject(message));
            } catch (Exception e) {
                //
            }
        }
        // ReSharper disable once FunctionNeverReturns
    }

    private static void EnsureConnected() {
        if (client.Connected)
            return;

        lock (client) {
            if (client.Connected)
                return;

            client.Connect("oni.logs", 15380);
            writer = new StreamWriter(client.GetStream()) { AutoFlush = true };
        }
    }

}

#endif
