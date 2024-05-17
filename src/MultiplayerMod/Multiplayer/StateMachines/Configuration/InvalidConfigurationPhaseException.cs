using System;
using MultiplayerMod.Core.Extensions;

namespace MultiplayerMod.Multiplayer.StateMachines.Configuration;

public class InvalidConfigurationPhaseException(
    Type stateMachineType,
    StateMachineConfigurationPhase targetPhase,
    StateMachineConfigurationPhase currentPhase
) : StateMachineConfigurationException(
    $"Configuration for {stateMachineType.GetPrettyName()} " +
    $"produced an action targeting \"{targetPhase}\" phase " +
    $"that wouldn't be run after the current \"{currentPhase}\" phase"
);
