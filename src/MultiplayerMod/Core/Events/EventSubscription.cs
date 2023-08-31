using System;

namespace MultiplayerMod.Core.Events;

public interface IEventSubscription {
    void Cancel();
}

public class EventSubscription : IEventSubscription {

    private readonly EventDispatcher dispatcher;
    private readonly Delegate action;
    private readonly Type type;

    public EventSubscription(EventDispatcher dispatcher, Delegate action, Type type) {
        this.dispatcher = dispatcher;
        this.action = action;
        this.type = type;
    }

    public void Cancel() => dispatcher.Unsubscribe(type, action);

}


public class EventSubscription<T> : IEventSubscription where T : notnull {

    private readonly EventDispatcher<T> dispatcher;
    private readonly Delegate action;

    public EventSubscription(EventDispatcher<T> dispatcher, Delegate action) {
        this.dispatcher = dispatcher;
        this.action = action;
    }

    public void Cancel() => dispatcher.Unsubscribe(action);

}
