using MultiplayerMod.ModRuntime.StaticCompatibility;

namespace MultiplayerMod.Multiplayer.StateMachines.Configuration.Dsl;

public abstract class StateMachineBaseConfigurerDsl<TStateMachine, TStateMachineInstance, TMaster, TDef>(
    StateMachineRootConfigurerDsl<TStateMachine, TStateMachineInstance, TMaster, TDef> root,
    StateMachineConfigurationContext context
)
    where TStateMachine : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>
    where TStateMachineInstance : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>.GameInstance
    where TMaster : IStateMachineTarget
{

    public StateMachineCustomizer<TStateMachine, TStateMachineInstance, TMaster, TDef> Customizer { get; } = new();

    public readonly MultiplayerGame Game = Dependencies.Get<MultiplayerGame>();
    public MultiplayerMode MultiplayerMode => Game.Mode;

    public GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>.State AddState(
        GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>.State parent,
        string name
    ) => Customizer.AddState(parent, name);

}
