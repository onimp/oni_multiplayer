using System;
using System.Linq;
using MultiplayerMod.Multiplayer.StateMachines.Configuration.Configurers;

namespace MultiplayerMod.Multiplayer.StateMachines.Configuration;

public abstract class StateMachineConfigurer(Type masterType, Type stateMachineType) {
    public Type MasterType { get; } = masterType;
    public Type StateMachineType { get; } = stateMachineType;

    public abstract void Configure(StateMachineConfigurationContext context);
}

public abstract class StateMachineConfigurer<TStateMachine, TStateMachineInstance, TMaster, TConfigurer>
    : StateMachineConfigurer<TStateMachine, TStateMachineInstance, TMaster, object, TConfigurer>
    where TStateMachine : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, object>
    where TStateMachineInstance : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, object>.GameInstance
    where TMaster : IStateMachineTarget
    where TConfigurer : StateMachineBoundedConfigurer<TStateMachine, TStateMachineInstance, TMaster, object>;


public class StateMachineConfigurer<TStateMachine, TStateMachineInstance, TMaster, TDef, TConfigurer>()
    : StateMachineConfigurer(typeof(TMaster), typeof(TStateMachine))
    where TStateMachine : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>
    where TStateMachineInstance : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>.GameInstance
    where TMaster : IStateMachineTarget
    where TConfigurer : StateMachineBoundedConfigurer<TStateMachine, TStateMachineInstance, TMaster, TDef>
{

    public override void Configure(StateMachineConfigurationContext context) {
        context.CreateConfiguration<TStateMachine, TMaster>();
        var root = new StateMachineRootConfigurer<TStateMachine, TStateMachineInstance, TMaster, TDef>(context);
        var constructor = typeof(TConfigurer).GetConstructors()[0];
        var arguments = constructor.GetParameters()
            .Select(
                parameter => parameter.ParameterType switch {
                    StateMachineRootConfigurer<TStateMachine, TStateMachineInstance, TMaster, TDef> => root,
                    _ => context.Dependencies.Get(parameter.ParameterType)
                }
            )
            .ToArray();
        var configurer = (TConfigurer) constructor.Invoke(arguments);
        configurer.Execute(root);
    }

}
