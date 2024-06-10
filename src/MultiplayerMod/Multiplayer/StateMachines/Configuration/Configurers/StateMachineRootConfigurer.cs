using System;
using MultiplayerMod.Core.Dependency;
using static MultiplayerMod.Multiplayer.StateMachines.Configuration.StateMachineConfigurationPhase;

namespace MultiplayerMod.Multiplayer.StateMachines.Configuration.Configurers;

public interface IStateMachineRootConfigurer<TStateMachine, TStateMachineInstance, TMaster, TDef>
    where TStateMachine : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>
    where TStateMachineInstance : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>.GameInstance
    where TMaster : IStateMachineTarget
{

    IDependencyContainer Dependencies { get; }

    void PreConfigure(Action<StateMachinePreConfigurer<TStateMachine, TStateMachineInstance, TMaster, TDef>> action);

    void PreConfigure(
        MultiplayerMode mode,
        Action<StateMachinePreConfigurer<TStateMachine, TStateMachineInstance, TMaster, TDef>> action
    );

    void PostConfigure(Action<StateMachinePostConfigurer<TStateMachine, TStateMachineInstance, TMaster, TDef>> action);

    void PostConfigure(
        MultiplayerMode mode,
        Action<StateMachinePostConfigurer<TStateMachine, TStateMachineInstance, TMaster, TDef>> action
    );

    void Inline(StateMachineConfigurer configurer);

}

public class StateMachineRootConfigurer<TStateMachine, TStateMachineInstance, TMaster, TDef>(
    StateMachineConfigurationContext context
) : IStateMachineRootConfigurer<TStateMachine, TStateMachineInstance, TMaster, TDef>
    where TStateMachine : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>
    where TStateMachineInstance : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>.GameInstance
    where TMaster : IStateMachineTarget {

    private readonly StateMachineConfiguration configuration = context.GetConfiguration<TStateMachine>();

    public IDependencyContainer Dependencies { get; } = context.Dependencies;

    public void AddAction(StateMachineConfigurationPhase phase, Action<TStateMachine> action) => configuration.Actions
        .Add(new StateMachineConfigurationAction(phase, stateMachine => action((TStateMachine) stateMachine)));

    public void PreConfigure(
        Action<StateMachinePreConfigurer<TStateMachine, TStateMachineInstance, TMaster, TDef>> action
    ) => AddAction(
        PreConfiguration,
        stateMachine => action(
            new StateMachinePreConfigurer<TStateMachine, TStateMachineInstance, TMaster, TDef>(
                this,
                context,
                stateMachine
            )
        )
    );

    public void PreConfigure(
        MultiplayerMode mode,
        Action<StateMachinePreConfigurer<TStateMachine, TStateMachineInstance, TMaster, TDef>> action
    ) => PreConfigure(it => {
        if (it.MultiplayerMode == mode)
            action(it);
    });

    public void PostConfigure(
        Action<StateMachinePostConfigurer<TStateMachine, TStateMachineInstance, TMaster, TDef>> action
    ) => AddAction(
        PostConfiguration,
        stateMachine => action(
            new StateMachinePostConfigurer<TStateMachine, TStateMachineInstance, TMaster, TDef>(
                this,
                context,
                stateMachine
            )
        )
    );

    public void PostConfigure(
        MultiplayerMode mode,
        Action<StateMachinePostConfigurer<TStateMachine, TStateMachineInstance, TMaster, TDef>> action
    ) => PostConfigure(it => {
        if (it.MultiplayerMode == mode)
            action(it);
    });

    public void Inline(StateMachineConfigurer configurer) => configurer.Configure(context);

}
