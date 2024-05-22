using System;
using static MultiplayerMod.Multiplayer.StateMachines.Configuration.StateMachineConfigurationPhase;

namespace MultiplayerMod.Multiplayer.StateMachines.Configuration.Dsl;

public interface IStateMachineRootConfigurerDsl<TStateMachine, TStateMachineInstance, TMaster, TDef>
    where TStateMachine : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>
    where TStateMachineInstance : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>.GameInstance
    where TMaster : IStateMachineTarget {
    void PreConfigure(
        Action<StateMachinePreConfigurerDsl<TStateMachine, TStateMachineInstance, TMaster, TDef>, TStateMachine> action
    );

    void PostConfigure(
        Action<StateMachinePostConfigurerDsl<TStateMachine, TStateMachineInstance, TMaster, TDef>, TStateMachine> action
    );

    void Inline(StateMachineConfigurer configurer);
}

public class
    StateMachineRootConfigurerDsl<TStateMachine, TStateMachineInstance, TMaster, TDef>
    : IStateMachineRootConfigurerDsl<TStateMachine, TStateMachineInstance, TMaster, TDef>
    where TStateMachine : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>
    where TStateMachineInstance : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>.GameInstance
    where TMaster : IStateMachineTarget {

    private readonly StateMachineConfiguration configuration;
    private readonly PhasedConfigurers configurers;
    private readonly StateMachineConfigurationContext context;

    public StateMachineRootConfigurerDsl(StateMachineConfigurationContext context) {
        this.context = context;
        configuration = context.GetConfiguration<TStateMachine>();
        configurers = new PhasedConfigurers(this, context);
    }

    public void AddAction(StateMachineConfigurationPhase phase, Action<TStateMachine> action) => configuration.Actions
        .Add(new StateMachineConfigurationAction(phase, stateMachine => action((TStateMachine) stateMachine)));

    public void PreConfigure(
        Action<StateMachinePreConfigurerDsl<TStateMachine, TStateMachineInstance, TMaster, TDef>, TStateMachine> action
    ) => AddAction(PreConfiguration, stateMachine => action(configurers.PreConfigurerDsl, stateMachine));

    public void PostConfigure(
        Action<StateMachinePostConfigurerDsl<TStateMachine, TStateMachineInstance, TMaster, TDef>, TStateMachine> action
    ) => AddAction(PostConfiguration, stateMachine => action(configurers.PostConfigurerDsl, stateMachine));

    public void Inline(StateMachineConfigurer configurer) => configurer.Configure(context);

    protected class PhasedConfigurers(
        StateMachineRootConfigurerDsl<TStateMachine, TStateMachineInstance, TMaster, TDef> root,
        StateMachineConfigurationContext context
    ) {
        public readonly StateMachinePreConfigurerDsl<TStateMachine, TStateMachineInstance, TMaster, TDef>
            PreConfigurerDsl = new(root, context);

        public readonly StateMachinePostConfigurerDsl<TStateMachine, TStateMachineInstance, TMaster, TDef>
            PostConfigurerDsl = new(root, context);
    }

}
