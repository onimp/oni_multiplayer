using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;

namespace MultiplayerMod.Multiplayer.States;

[Dependency, UsedImplicitly]
public class StatesManager {

    public const string StateName = "WaitHostState";

    public virtual void AllowTransition(Chore chore, string? targetState, Dictionary<int, object?> args) {
        var smi = GetSmi(chore);

        var waitHostState = GetWaitHostState(smi);
        waitHostState.GetType().GetMethod("AllowTransition").Invoke(
            waitHostState,
            new object?[] { smi, targetState, args }
        );
    }

    public virtual void DisableChoreStateTransition(StateMachine.BaseState stateToBeSynced) {
        var sm = (StateMachine) stateToBeSynced.GetType().GetField("sm").GetValue(stateToBeSynced);
        InjectWaitHostState(sm);
        var callbackType = typeof(StateMachine<,,,>)
            .GetNestedType("State")
            .GetNestedType("Callback")
            .MakeGenericType(sm.GetType().BaseType.GetGenericArguments().Append(typeof(object)));
        stateToBeSynced.enterActions ??= new List<StateMachine.Action>();
        stateToBeSynced.enterActions.Clear();
        var method = typeof(StatesManager).GetMethod(
            nameof(TransitToWaitState),
            BindingFlags.NonPublic | BindingFlags.Static
        )!;
        var callback = Delegate.CreateDelegate(callbackType, method);
        stateToBeSynced.enterActions.Add(new StateMachine.Action("Transit to waiting state", callback));
    }

    public void InjectWaitHostState(StateMachine sm) {
        var genericType = typeof(WaitHostState<,,,>).MakeGenericType(
            sm.GetType().BaseType.GetGenericArguments().Append(typeof(object))
        );
        Activator.CreateInstance(genericType, sm);
    }

    public object GetWaitHostState(Chore chore) {
        return GetWaitHostState(GetSmi(chore));
    }

    public StateMachine.Instance GetSmi(Chore chore) {
        return (StateMachine.Instance) chore.GetType().GetProperty(nameof(Chore<StateMachine.Instance>.smi))
            .GetValue(chore);
    }

    private object GetWaitHostState(StateMachine.Instance smi) => smi.stateMachine.GetState("root." + StateName);

    private static void TransitToWaitState(StateMachine.Instance smi) {
        smi.GoTo("root." + StateName);
    }

}
