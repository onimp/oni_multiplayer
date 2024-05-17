namespace MultiplayerMod.Multiplayer.StateMachines.Configuration.Dsl;

public delegate void StateMachineConfigurerDelegate<TStateMachine, TStateMachineInstance, TMaster, TDef>(
    IStateMachineConfigurerDsl<TStateMachine, TStateMachineInstance, TMaster, TDef> dsl
)
    where TStateMachine : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>
    where TStateMachineInstance : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>.GameInstance
    where TMaster : IStateMachineTarget;
