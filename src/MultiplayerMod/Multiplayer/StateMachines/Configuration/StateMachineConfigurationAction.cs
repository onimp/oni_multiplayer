namespace MultiplayerMod.Multiplayer.StateMachines.Configuration;

public record StateMachineConfigurationAction(
    StateMachineConfigurationPhase Phase,
    System.Action<StateMachine> Configure
);
