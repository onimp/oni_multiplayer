namespace MultiplayerMod.Multiplayer.StateMachines.Configuration.Configurers;

public delegate void StateMachineConfigurerDelegate<TStateMachine, TStateMachineInstance, TMaster, TDef>(
    IStateMachineRootConfigurer<TStateMachine, TStateMachineInstance, TMaster, TDef> root
)
    where TStateMachine : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>
    where TStateMachineInstance : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>.GameInstance
    where TMaster : IStateMachineTarget;
