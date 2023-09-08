using System;
using System.Collections.Generic;
using MultiplayerMod.Core.Collections;
using MultiplayerMod.Core.Extensions;

namespace MultiplayerMod.Core.Events;

public class EventDispatcher {

    private readonly Dictionary<Type, LinkedHashSet<SubscribedAction>> handlers = new();

    public event Action<object>? EventDispatching;

    public EventSubscription Subscribe<T>(Action<T> action) => Subscribe<T>((@event, _) => action(@event));

    public EventSubscription Subscribe<T>(Action<T, EventSubscription> action) {
        var type = typeof(T);
        if (!handlers.TryGetValue(type, out var delegates)) {
            delegates = new LinkedHashSet<SubscribedAction>();
            handlers[type] = delegates;
        }
        var subscription = new EventSubscription(this, action, type);
        delegates.Add(new SubscribedAction(action, subscription));
        return subscription;
    }

    public void Unsubscribe<T>(Action<T> action) => Unsubscribe(typeof(T), action);

    public void Unsubscribe(Type type, Delegate action) {
        if (!handlers.TryGetValue(type, out var delegates))
            return;

        delegates.Remove(new SubscribedAction(action, null));
    }

    public void Dispatch<T>(T @event) where T : notnull {
        EventDispatching?.Invoke(@event);
        if (!handlers.TryGetValue(typeof(T), out var delegates))
            return;

        var actions = new LinkedList<SubscribedAction>(delegates);
        actions.ForEach(it => it.Action.Method.Invoke(it.Action.Target, new object?[] { @event, it.Subscription }));
    }

    private class SubscribedAction {

        public readonly Delegate Action;
        public readonly EventSubscription? Subscription;

        public SubscribedAction(Delegate action, EventSubscription? subscription) {
            Action = action;
            Subscription = subscription;
        }

        private bool Equals(SubscribedAction other) => Action.Equals(other.Action);

        public override bool Equals(object? obj) {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;

            return obj.GetType() == GetType() && Equals((SubscribedAction) obj);
        }

        public override int GetHashCode() => Action.GetHashCode();

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
