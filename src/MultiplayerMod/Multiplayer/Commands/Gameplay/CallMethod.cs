using System;
using System.Reflection;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Game.Mechanics.Objects;
using MultiplayerMod.Multiplayer.Compatibility;
using MultiplayerMod.Multiplayer.Objects.Extensions;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Multiplayer.Commands.Gameplay;

[Serializable]
public class CallMethod : MultiplayerCommand {

    private static readonly Logger log = LoggerFactory.GetLogger(typeof(CallMethod));

    private readonly Reference target;
    private readonly Type declaringType;
    private readonly string methodName;
    private readonly object?[] args;

    public CallMethod(ComponentEventsArgs eventArgs) {
        target = eventArgs.Component.GetReference();
        declaringType = eventArgs.Method.DeclaringType!;
        methodName = eventArgs.Method.Name;
        args = ArgumentUtils.WrapObjects(eventArgs.Args);
    }

    public CallMethod(StateMachineEventsArgs eventArgs) {
        target = eventArgs.StateMachineInstance.GetReference();
        declaringType = eventArgs.Method.DeclaringType!;
        methodName = eventArgs.Method.Name;
        args = ArgumentUtils.WrapObjects(eventArgs.Args);
    }

    public override void Execute(MultiplayerCommandContext context) {
        var method = declaringType.GetMethod(
            methodName,
            BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance |
            BindingFlags.DeclaredOnly
        );
        try {
            var obj = target.Resolve();
            if (obj != null)
                method?.Invoke(obj, ArgumentUtils.UnWrapObjects(args));
        } catch (Exception e) when (IsRecoverableExecutionFailure(e)) {
            var message = $"Skipped received multiplayer method {declaringType.Name}.{methodName}: {e.GetBaseException().Message}";
            log.Warning(message);
            DlcMultiplayerSafety.NotifyUnsupported(message);
        }
    }

    public override string ToString() => $"{base.ToString()} (Type = {declaringType.FullName}, Method = {methodName})";

    private static bool IsRecoverableExecutionFailure(Exception e) =>
        e is ObjectNotFoundException
            or TargetInvocationException
            or TargetParameterCountException
            or ArgumentException
            or InvalidOperationException;

}
