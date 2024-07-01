using MultiplayerMod.Exceptions;

namespace MultiplayerMod.Multiplayer.StateMachines.RuntimeTools;

public class StateMachineStateNotFoundException(StateMachine stateMachine, string name) : MultiplayerException(
    $"State \"{name}\" not found in \"{stateMachine.name}\""
);
