namespace MultiplayerMod.Multiplayer.StateMachines.Configuration.Dsl;

public class StateMachinePostConfigurerDsl<TStateMachine, TStateMachineInstance, TMaster, TDef>(
    StateMachineRootConfigurerDsl<TStateMachine, TStateMachineInstance, TMaster, TDef> root,
    StateMachineConfigurationContext context
) : StateMachineBaseConfigurerDsl<TStateMachine, TStateMachineInstance, TMaster, TDef>(root, context)
    where TStateMachine : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>
    where TStateMachineInstance : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>.GameInstance
    where TMaster : IStateMachineTarget;
