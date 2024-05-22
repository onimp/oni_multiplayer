using MultiplayerMod.ModRuntime.StaticCompatibility;

namespace MultiplayerMod.Multiplayer.StateMachines.Configuration.Dsl;

public abstract class StateMachineBaseConfigurerDsl<TStateMachine, TStateMachineInstance, TMaster, TDef>(
    StateMachineRootConfigurerDsl<TStateMachine, TStateMachineInstance, TMaster, TDef> root,
    StateMachineConfigurationContext context
)
    where TStateMachine : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>
    where TStateMachineInstance : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>.GameInstance
    where TMaster : IStateMachineTarget {

    public readonly MultiplayerGame Game = Dependencies.Get<MultiplayerGame>();
    public MultiplayerMode MultiplayerMode => Game.Mode;

}
