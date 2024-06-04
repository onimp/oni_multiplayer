using System;
using MultiplayerMod.Multiplayer.StateMachines.Configuration.Dsl;

namespace MultiplayerMod.Multiplayer.StateMachines.Configuration;

public abstract class StateMachineConfigurer(Type masterType, Type stateMachineType) {
    public Type MasterType { get; } = masterType;
    public Type StateMachineType { get; } = stateMachineType;

    public abstract StateMachineConfiguration Configure(StateMachineConfigurationContext context);
}

public class StateMachineConfigurer<TStateMachine, TStateMachineInstance, TMaster>(
    StateMachineConfigurerDelegate<TStateMachine, TStateMachineInstance, TMaster, object> scopedAction
)
    : StateMachineConfigurer<TStateMachine, TStateMachineInstance, TMaster, object>(scopedAction)
    where TStateMachine : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, object>
    where TStateMachineInstance : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, object>.GameInstance
    where TMaster : IStateMachineTarget;

public class StateMachineConfigurer<TStateMachine, TStateMachineInstance, TMaster, TDef>(
    StateMachineConfigurerDelegate<TStateMachine, TStateMachineInstance, TMaster, TDef> scopedAction
)
    : StateMachineConfigurer(typeof(TMaster), typeof(TStateMachine))
    where TStateMachine : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>
    where TStateMachineInstance : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>.GameInstance
    where TMaster : IStateMachineTarget
{

    public override StateMachineConfiguration Configure(StateMachineConfigurationContext context) {
        var configuration = context.CreateConfiguration<TStateMachine, TMaster>();
        var root = new StateMachineRootConfigurerDsl<TStateMachine, TStateMachineInstance, TMaster, TDef>(context);
        scopedAction(root);
        return configuration;
    }

}
