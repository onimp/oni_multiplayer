using System;
using System.Collections.Generic;

namespace MultiplayerMod.Multiplayer.Tools;

public class CommandRateThrottle {

    private readonly TimeSpan period;
    private readonly Dictionary<Type, System.DateTime> lastInvokedByType = new();

    public CommandRateThrottle(int rate) {
        period = new TimeSpan(10000000 / rate);
    }

    public void Run<T>(System.Action action) where T : IMultiplayerCommand {
        lastInvokedByType.TryGetValue(typeof(T), out var lastInvoked);
        if (System.DateTime.Now - lastInvoked < period)
            return;
        action();
        lastInvokedByType[typeof(T)] = System.DateTime.Now;
    }

}
