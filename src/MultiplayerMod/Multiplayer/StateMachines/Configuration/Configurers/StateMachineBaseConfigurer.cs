namespace MultiplayerMod.Multiplayer.StateMachines.Configuration.Configurers;

public abstract class StateMachineBaseConfigurer<TStateMachine, TStateMachineInstance, TMaster, TDef>(
    StateMachineRootConfigurer<TStateMachine, TStateMachineInstance, TMaster, TDef> root,
    StateMachineConfigurationContext context
)
    where TStateMachine : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>
    where TStateMachineInstance : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>.GameInstance
    where TMaster : IStateMachineTarget
{

    public readonly MultiplayerGame Game = context.Dependencies.Get<MultiplayerGame>();
    public MultiplayerMode MultiplayerMode => Game.Mode;

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
