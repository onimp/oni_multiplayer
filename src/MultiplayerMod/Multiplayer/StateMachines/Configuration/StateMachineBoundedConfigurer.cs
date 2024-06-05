using MultiplayerMod.Core.Extensions;
using MultiplayerMod.Multiplayer.StateMachines.Configuration.Configurers;

namespace MultiplayerMod.Multiplayer.StateMachines.Configuration;

public abstract class StateMachineBoundedConfigurer<TStateMachine, TStateMachineInstance, TMaster>
    : StateMachineBoundedConfigurer<TStateMachine, TStateMachineInstance, TMaster, object>
    where TStateMachine : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, object>
    where TStateMachineInstance : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, object>.GameInstance
    where TMaster : IStateMachineTarget;

public abstract class StateMachineBoundedConfigurer<TStateMachine, TStateMachineInstance, TMaster, TDef>
    where TStateMachine : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>
    where TStateMachineInstance : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>.GameInstance
    where TMaster : IStateMachineTarget {

    protected virtual StateMachineConfigurer[] Inline() => [];

    protected virtual void PreConfigure(
        StateMachinePreConfigurer<TStateMachine, TStateMachineInstance, TMaster, TDef> configurer,
        TStateMachine stateMachine
    ) { }

    protected virtual void PostConfigure(
        StateMachinePostConfigurer<TStateMachine, TStateMachineInstance, TMaster, TDef> configurer,
        TStateMachine stateMachine
    ) { }

    public void Configure(StateMachineRootConfigurer<TStateMachine, TStateMachineInstance, TMaster, TDef> root) {
        Inline().ForEach(root.Inline);
        root.PreConfigure(PreConfigure);
        root.PostConfigure(PostConfigure);
    }

}

