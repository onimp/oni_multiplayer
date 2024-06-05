using System;
using static MultiplayerMod.Multiplayer.StateMachines.Configuration.StateMachineConfigurationPhase;

namespace MultiplayerMod.Multiplayer.StateMachines.Configuration.Configurers;

public interface IStateMachineRootConfigurer<TStateMachine, TStateMachineInstance, TMaster, TDef>
    where TStateMachine : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>
    where TStateMachineInstance : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>.GameInstance
    where TMaster : IStateMachineTarget
{
    void PreConfigure(
        Action<StateMachinePreConfigurer<TStateMachine, TStateMachineInstance, TMaster, TDef>, TStateMachine> action
    );

    void PostConfigure(
        Action<StateMachinePostConfigurer<TStateMachine, TStateMachineInstance, TMaster, TDef>, TStateMachine> action
    );

    void Inline(StateMachineConfigurer configurer);
}

public class StateMachineRootConfigurer<TStateMachine, TStateMachineInstance, TMaster, TDef>
    : IStateMachineRootConfigurer<TStateMachine, TStateMachineInstance, TMaster, TDef>
    where TStateMachine : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>
    where TStateMachineInstance : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>.GameInstance
    where TMaster : IStateMachineTarget
{

    private readonly StateMachineConfiguration configuration;
    private readonly PhasedConfigurers configurers;
    private readonly StateMachineConfigurationContext context;

    public StateMachineRootConfigurer(StateMachineConfigurationContext context) {
        this.context = context;
        configuration = context.GetConfiguration<TStateMachine>();
        configurers = new PhasedConfigurers(this, context);
    }

    public void AddAction(StateMachineConfigurationPhase phase, Action<TStateMachine> action) => configuration.Actions
        .Add(new StateMachineConfigurationAction(phase, stateMachine => action((TStateMachine) stateMachine)));

    public void PreConfigure(
        Action<StateMachinePreConfigurer<TStateMachine, TStateMachineInstance, TMaster, TDef>, TStateMachine> action
    ) => AddAction(PreConfiguration, stateMachine => action(configurers.PreConfigurer, stateMachine));

    public void PostConfigure(
        Action<StateMachinePostConfigurer<TStateMachine, TStateMachineInstance, TMaster, TDef>, TStateMachine> action
    ) => AddAction(PostConfiguration, stateMachine => action(configurers.PostConfigurer, stateMachine));

    public void Inline(StateMachineConfigurer configurer) => configurer.Configure(context);

    protected class PhasedConfigurers(
        StateMachineRootConfigurer<TStateMachine, TStateMachineInstance, TMaster, TDef> root,
        StateMachineConfigurationContext context
    ) {
        public readonly StateMachinePreConfigurer<TStateMachine, TStateMachineInstance, TMaster, TDef>
            PreConfigurer = new(root, context);

        public readonly StateMachinePostConfigurer<TStateMachine, TStateMachineInstance, TMaster, TDef>
            PostConfigurer = new(root, context);
    }

}
