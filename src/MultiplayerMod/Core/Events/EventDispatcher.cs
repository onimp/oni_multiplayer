using System;
using System.Collections.Generic;
using System.Reflection;
using MultiplayerMod.Core.Collections;
using MultiplayerMod.Core.Extensions;
using MultiplayerMod.Core.Logging;

namespace MultiplayerMod.Core.Events;

public class EventDispatcher {

    private static readonly Core.Logging.Logger log = LoggerFactory.GetLogger(typeof(EventDispatcher));

    private readonly Dictionary<Type, LinkedHashSet<SubscribedAction>> handlers = new();

    public event Action<object>? EventDispatching;

    public EventSubscription Subscribe<T>(Action<T> action) where T : IDispatchableEvent {
        return Subscribe<T>((@event, _) => action(@event));
    }

    public EventSubscription Subscribe<T>(Action<T, EventSubscription> action) where T : IDispatchableEvent {
        var type = typeof(T);
        if (!handlers.TryGetValue(type, out var delegates)) {
            delegates = new LinkedHashSet<SubscribedAction>();
            handlers[type] = delegates;
        }
        var subscription = new EventSubscription(this, action, type);
        delegates.Add(new SubscribedAction(action, subscription));
        return subscription;
    }

    public void Unsubscribe<T>(Action<T> action) where T : IDispatchableEvent {
        Unsubscribe(typeof(T), action);
    }

    public void Unsubscribe(Type type, Delegate action) {
        if (!handlers.TryGetValue(type, out var delegates))
            return;

        delegates.Remove(new SubscribedAction(action, null));
    }

    public void Dispatch<T>(T @event) where T : IDispatchableEvent {
        EventDispatching?.Invoke(@event);
        if (!handlers.TryGetValue(typeof(T), out var delegates))
            return;

        var actions = new LinkedList<SubscribedAction>(delegates);
        actions.ForEach(it => Invoke(it, @event));
    }

    // Isolate each handler: one throwing subscriber must not abort the remaining handlers or unwind into the
    // caller. This matters most during teardown - GameQuit / StopMultiplayer are dispatched from ONI's
    // PauseScreen quit postfix, and an escaping exception there corrupts the menu/scene transition (the
    // "can't rejoin without restarting the game" class of bug). Log and continue instead.
    private static void Invoke<T>(SubscribedAction action, T @event) where T : IDispatchableEvent {
        try {
            action.Action.Method.Invoke(action.Action.Target, new object?[] { @event, action.Subscription });
        } catch (Exception exception) {
            // Method.Invoke wraps the real failure in a TargetInvocationException - unwrap it for the log.
            var actual = (exception as TargetInvocationException)?.InnerException ?? exception;
            log.Error($"Event handler for {typeof(T).Name} threw and was isolated: {actual}");
        }
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

public class EventDispatcher<T> where T : IDispatchableEvent {

    private static readonly Core.Logging.Logger log = LoggerFactory.GetLogger(typeof(EventDispatcher<T>));

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
        delegates.ForEach(
            it => {
                // Same handler isolation as the untyped EventDispatcher above - a throwing subscriber must
                // not abort the rest or unwind into the caller.
                try {
                    it.Method.Invoke(it.Target, arguments);
                } catch (Exception exception) {
                    var actual = (exception as TargetInvocationException)?.InnerException ?? exception;
                    log.Error($"Event handler for {typeof(T).Name} threw and was isolated: {actual}");
                }
            }
        );
    }

}
