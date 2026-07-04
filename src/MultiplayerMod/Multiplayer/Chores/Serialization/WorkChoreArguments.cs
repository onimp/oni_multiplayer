using System;
using System.Linq;
using System.Reflection;

namespace MultiplayerMod.Multiplayer.Chores.Serialization;

// Builds a constructor-argument array for a WorkChore<WorkableType> from an already-constructed
// instance, so the client can rebuild an equivalent chore through the normal CreateChore path
// (reflection Invoke on the closed generic ctor — which is safe; only *patching* the generic ctor
// corrupts Mono generic code sharing, so the host never does that).
//
// Only the identity-defining arguments are recovered from the instance: the chore type, the workable
// target, the provider, and only_when_operational. Everything else falls back to the constructor's
// own default values. That is sufficient because the client's WorkChore is a host-driven puppet — its
// preconditions are overridden and its state machine is driven by commands — so precondition-shaping
// arguments (schedule block, red-alert, anim overrides) don't need to round-trip faithfully.
public static class WorkChoreArguments {

    public static object?[] Build(Chore chore) {
        var parameters = chore.GetType().GetConstructors()[0].GetParameters();
        var arguments = parameters
            .Select(parameter => parameter.HasDefaultValue ? parameter.DefaultValue : null)
            .ToArray();

        Set(parameters, arguments, "chore_type", chore.choreType);
        Set(parameters, arguments, "target", chore.target);
        Set(parameters, arguments, "chore_provider", chore.provider);

        // onlyWhenOperational is a public getter on the closed WorkChore<T>; read it reflectively to
        // avoid binding to the open generic. Best-effort — defaults to the ctor default if absent.
        var onlyWhenOperational = chore.GetType().GetProperty("onlyWhenOperational");
        if (onlyWhenOperational != null)
            Set(parameters, arguments, "only_when_operational", onlyWhenOperational.GetValue(chore));

        return arguments;
    }

    private static void Set(ParameterInfo[] parameters, object?[] arguments, string name, object? value) {
        var index = Array.FindIndex(parameters, parameter => parameter.Name == name);
        if (index >= 0)
            arguments[index] = value;
    }

}
