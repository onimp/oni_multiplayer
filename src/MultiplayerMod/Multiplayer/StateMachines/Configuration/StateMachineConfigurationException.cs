using MultiplayerMod.Exceptions;

namespace MultiplayerMod.Multiplayer.StateMachines.Configuration;

public class StateMachineConfigurationException(string message) : MultiplayerException(message);
