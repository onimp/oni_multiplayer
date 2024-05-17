using System;
using MultiplayerMod.Multiplayer.StateMachines.Configuration;

namespace MultiplayerMod.Multiplayer.Chores;

public record ChoreConfiguration(Type ChoreType, StateMachineConfigurer? StatesConfigurer);
