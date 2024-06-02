namespace MultiplayerMod.Multiplayer.StateMachines.Configuration;

public class StateMachineCustomizer<TStateMachine, TStateMachineInstance, TMaster, TDef>
    where TStateMachine : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>
    where TStateMachineInstance : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>.GameInstance
    where TMaster : IStateMachineTarget
{

    public GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>.State AddState(
        GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>.State parent,
        string name
    ) {
        var state = new GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>.State();
        parent.sm.BindState(parent, state, name);
        state.sm = parent.sm;
        return state;
    }

}
