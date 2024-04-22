using MultiplayerMod.Exceptions;

namespace MultiplayerMod.Multiplayer.StateMachines;

public class StateMachineConfigurationException(string message) : MultiplayerException(message);
