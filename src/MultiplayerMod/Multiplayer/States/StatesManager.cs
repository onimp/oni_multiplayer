using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Reflection;

namespace MultiplayerMod.Multiplayer.States;

[Dependency, UsedImplicitly]
public class StatesManager {

    public const string WaitStateName = "WaitHostState";
    public const string ContinuationName = "ContinuationState";

    public virtual void AllowTransition(Chore chore, string? targetState, Dictionary<int, object?> args) {
        var smi = GetSmi(chore);

        var waitHostState = GetWaitHostState(smi);
        waitHostState.GetType().GetMethod("AllowTransition").Invoke(
            waitHostState,
            new object?[] { smi, targetState, args }
        );
    }

    public virtual void AddAndTransitToWaiStateUponEnter(StateMachine.BaseState state) {
        var sm = GetSm(state);
        InjectWaitHostState(sm);
        var callbackType = typeof(StateMachine<,,,>)
            .GetNestedType("State")
            .GetNestedType("Callback")
            .MakeGenericType(sm.GetType().BaseType.GetGenericArguments().Append(typeof(object)));
        var method = typeof(StatesManager).GetMethod(
            nameof(TransitToWaitState),
            BindingFlags.NonPublic | BindingFlags.Static
        )!;
        var callback = Delegate.CreateDelegate(callbackType, method);
        state.enterActions.Add(new StateMachine.Action("Transit to waiting state", callback));
    }

    public virtual StateMachine.BaseState AddContinuationState(StateMachine.BaseState state) {
        var sm = GetSm(state);
        var genericType = typeof(ContinuationState<,,,>).MakeGenericType(
            sm.GetType().BaseType.GetGenericArguments().Append(typeof(object))
        );
        return (StateMachine.BaseState) Activator.CreateInstance(genericType, sm, state);
    }

    public void InjectWaitHostState(StateMachine sm) {
        var genericType = typeof(WaitHostState<,,,>).MakeGenericType(
            sm.GetType().BaseType.GetGenericArguments().Append(typeof(object))
        );
        Activator.CreateInstance(genericType, sm);
    }

    public StateMachine.BaseState GetWaitHostState(Chore chore) {
        return GetWaitHostState(GetSmi(chore));
    }

    public StateMachine.Instance GetSmi(Chore chore) {
        return (StateMachine.Instance) chore.GetType().GetProperty(nameof(Chore<StateMachine.Instance>.smi))
            .GetValue(chore);
    }

    private static StateMachine GetSm(StateMachine.BaseState state) {
        return state.GetFieldValue<StateMachine>("sm");
    }

    private static StateMachine.BaseState GetWaitHostState(StateMachine.Instance smi) =>
        smi.stateMachine.GetState("root." + WaitStateName);

    private static void TransitToWaitState(StateMachine.Instance smi) {
        smi.GoTo(GetWaitHostState(smi));
    }

}
