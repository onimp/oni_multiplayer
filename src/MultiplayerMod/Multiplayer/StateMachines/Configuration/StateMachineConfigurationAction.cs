using System;

namespace MultiplayerMod.Multiplayer.StateMachines.Configuration;

public record StateMachineConfigurationAction(
    StateMachineConfigurationPhase Phase,
    Action<StateMachine> Configure
);
