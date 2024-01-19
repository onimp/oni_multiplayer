using System;

namespace MultiplayerMod.Game.Chores.Types;

public record StatesTransitionConfig(
    StatesTransitionStatus Status,
    Type StateType,
    StateTransitionConfig[] StateTransitionConfigs
) {

    public static StatesTransitionConfig Enabled<T>(
        params StateTransitionConfig[] stateTransitionConfigs
    ) where T : StateMachine {
        return new StatesTransitionConfig(StatesTransitionStatus.On, typeof(T), stateTransitionConfigs);
    }

    public static StatesTransitionConfig Disabled() {
        return new StatesTransitionConfig(StatesTransitionStatus.Off, null!, Array.Empty<StateTransitionConfig>());
    }
}
