using System;
using System.Collections.Generic;
using MultiplayerMod.Core.Collections;
using MultiplayerMod.Core.Extensions;

namespace MultiplayerMod.Core.Events;

public class EventDispatcher {

    private readonly Dictionary<Type, LinkedHashSet<Delegate>> handlers = new();

    public event Action<object>? EventDispatching;

    public EventSubscription Subscribe<T>(Action<T> action) {
        var type = typeof(T);
        if (!handlers.TryGetValue(type, out var delegates)) {
            delegates = new LinkedHashSet<Delegate>();
            handlers[type] = delegates;
        }
        delegates.Add(action);
        return new EventSubscription(this, action, type);
    }

    public void Unsubscribe<T>(Action<T> action) => Unsubscribe(typeof(T), action);

    public void Unsubscribe(Type type, Delegate action) {
        if (!handlers.TryGetValue(type, out var delegates))
            return;

        delegates.Remove(action);
    }

    public void Dispatch<T>(T @event) where T : notnull {
        EventDispatching?.Invoke(@event);
        if (!handlers.TryGetValue(typeof(T), out var delegates))
            return;

        var arguments = new object[] { @event };
        delegates.ForEach(it => it.Method.Invoke(it.Target, arguments));
    }

}

public class EventDispatcher<T> where T : notnull {

    private readonly LinkedHashSet<Delegate> delegates = new();

    public EventSubscription<T> Subscribe(Action<T> action) {
        delegates.Add(action);
        return new EventSubscription<T>(this, action);
    }

    public void Unsubscribe(Delegate action) {
        delegates.Remove(action);
    }

    public void Dispatch(T @event) {
        var arguments = new object[] { @event };
        delegates.ForEach(it => it.Method.Invoke(it.Target, arguments));
    }

}
