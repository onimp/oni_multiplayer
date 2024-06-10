using MultiplayerMod.Multiplayer.StateMachines.Configuration.Configurers;

namespace MultiplayerMod.Multiplayer.StateMachines.Configuration;

public class StateMachineConfigurerDsl<TStateMachine, TStateMachineInstance>(
    StateMachineConfigurerDelegate<TStateMachine, TStateMachineInstance, IStateMachineTarget, object> boundedAction
)
    : StateMachineConfigurerDsl<TStateMachine, TStateMachineInstance, IStateMachineTarget, object>(boundedAction)
    where TStateMachine : GameStateMachine<TStateMachine, TStateMachineInstance, IStateMachineTarget, object>
    where TStateMachineInstance : GameStateMachine<TStateMachine, TStateMachineInstance, IStateMachineTarget, object>.GameInstance;

public class StateMachineConfigurerDsl<TStateMachine, TStateMachineInstance, TMaster>(
    StateMachineConfigurerDelegate<TStateMachine, TStateMachineInstance, TMaster, object> boundedAction
)
    : StateMachineConfigurerDsl<TStateMachine, TStateMachineInstance, TMaster, object>(boundedAction)
    where TStateMachine : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, object>
    where TStateMachineInstance : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, object>.GameInstance
    where TMaster : IStateMachineTarget;

public class StateMachineConfigurerDsl<TStateMachine, TStateMachineInstance, TMaster, TDef>(
    StateMachineConfigurerDelegate<TStateMachine, TStateMachineInstance, TMaster, TDef> boundedAction
)
    : StateMachineConfigurer(typeof(TMaster), typeof(TStateMachine))
    where TStateMachine : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>
    where TStateMachineInstance : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>.GameInstance
    where TMaster : IStateMachineTarget
{

    public override void Configure(StateMachineConfigurationContext context) {
        context.CreateConfiguration<TStateMachine, TMaster>();
        var root = new StateMachineRootConfigurer<TStateMachine, TStateMachineInstance, TMaster, TDef>(context);
        boundedAction(root);
    }

}
