namespace MultiplayerMod.Multiplayer.StateMachines.Configuration.Configurers;

public class StateMachinePostConfigurer<TStateMachine, TStateMachineInstance, TMaster, TDef>(
    StateMachineRootConfigurer<TStateMachine, TStateMachineInstance, TMaster, TDef> root,
    StateMachineConfigurationContext context
) : StateMachineBaseConfigurer<TStateMachine, TStateMachineInstance, TMaster, TDef>(root, context)
    where TStateMachine : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>
    where TStateMachineInstance : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>.GameInstance
    where TMaster : IStateMachineTarget;
