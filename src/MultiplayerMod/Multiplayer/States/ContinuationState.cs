using MultiplayerMod.Game.NameOf;

namespace MultiplayerMod.Multiplayer.States;

public class
    ContinuationState<StateMachineType, StateMachineInstanceType, MasterType, DefType> : StateMachine<StateMachineType,
    StateMachineInstanceType, MasterType, DefType>.State
    where StateMachineInstanceType : StateMachine.Instance
    where MasterType : IStateMachineTarget {

    public ContinuationState(StateMachine sm, StateMachine.BaseState original) {
        name = StatesManager.ContinuationName;

        // enter and update actions must be synced differently.
        exitActions = original.exitActions;
        defaultState = original.defaultState;

        var stateMachineType = sm.GetType();
        var root = stateMachineType.GetField(nameof(StateMachineMemberReference.root)).GetValue(sm);
        stateMachineType.GetMethod(nameof(StateMachineMemberReference.BindState))!
            .Invoke(sm, new[] { root, this, name });
    }
}
