using System;
using System.Collections.Generic;
using MultiplayerMod.Core.Extensions;

namespace MultiplayerMod.Multiplayer.StateMachines.Configuration;

public record StateMachineConfigurationContext {

    private readonly Dictionary<Type, StateMachineConfiguration> configurations = [];

    public bool Locked { get; private set; }

    public StateMachineConfiguration CreateConfiguration(Type stateMachineType, Type masterType) {
        if (Locked)
            throw new ConfigurationContextLockedException(
                $"Unable to create / update a configuration for {stateMachineType.GetSignature()}: context locked"
            );

        return configurations.GetOrAdd(
            stateMachineType,
            () => new StateMachineConfiguration(masterType, stateMachineType, [])
        );
    }

    // @formatter:off
    public StateMachineConfiguration CreateConfiguration<TStateMachine, TMaster>()
        where TStateMachine : StateMachine
        where TMaster : IStateMachineTarget
    {
        return CreateConfiguration(typeof(TStateMachine), typeof(TMaster));
    }
    // @formatter:on

    public StateMachineConfiguration GetConfiguration<TStateMachine>() => configurations[typeof(TStateMachine)];

    public ICollection<StateMachineConfiguration> Configurations => configurations.Values;

    public void Lock() => Locked = true;

}
