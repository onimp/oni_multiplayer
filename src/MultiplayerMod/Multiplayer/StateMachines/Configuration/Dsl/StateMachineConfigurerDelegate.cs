namespace MultiplayerMod.Multiplayer.StateMachines.Configuration.Dsl;

public delegate void StateMachineConfigurerDelegate<TStateMachine, TStateMachineInstance, TMaster, TDef>(
    IStateMachineRootConfigurerDsl<TStateMachine, TStateMachineInstance, TMaster, TDef> root
)
    where TStateMachine : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>
    where TStateMachineInstance : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>.GameInstance
    where TMaster : IStateMachineTarget;
