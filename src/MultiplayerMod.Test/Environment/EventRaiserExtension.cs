using System;
using System.Reflection;
using MultiplayerMod.ModRuntime;

namespace MultiplayerMod.Test.Environment;

public static class EventRaiserExtension {

    private const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static;

    public static void RaiseEvent(this Type type, string eventName, Runtime runtime, params object[] args) {
        var eventField = type.GetField(eventName, flags);
        if (eventField == null)
            throw new Exception($"Event with name {eventName} could not be found.");

        var multicastDelegate = eventField.GetValue(null) as MulticastDelegate;
        if (multicastDelegate == null)
            return;

        var invocationList = multicastDelegate.GetInvocationList();

        foreach (var invocationMethod in invocationList) {
            var target = invocationMethod.Target;
            if (ReferenceEquals(runtime.Dependencies.Get(target.GetType()), target))
                invocationMethod.Method.Invoke(invocationMethod.Target, args);
        }
    }

}
