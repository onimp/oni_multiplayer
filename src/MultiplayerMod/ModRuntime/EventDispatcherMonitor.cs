using JetBrains.Annotations;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Core.Logging;

namespace MultiplayerMod.ModRuntime;

[UsedImplicitly]
public class EventDispatcherMonitor {

    private readonly Core.Logging.Logger log = LoggerFactory.GetLogger<EventDispatcherMonitor>();

    public EventDispatcherMonitor(EventDispatcher eventDispatcher) {
        if (log.Level == LogLevel.Trace)
            eventDispatcher.EventDispatching += OnEventDispatching;
    }

    private void OnEventDispatching(object @event) => log.Trace($"Event dispatched: {@event}");

}
