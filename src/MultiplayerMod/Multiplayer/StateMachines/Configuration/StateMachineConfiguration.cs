using System;
using System.Collections.Generic;

namespace MultiplayerMod.Multiplayer.StateMachines.Configuration;

public record StateMachineConfiguration(
    Type MasterType,
    Type StateMachineType,
    List<StateMachineConfigurationAction> Actions
);
