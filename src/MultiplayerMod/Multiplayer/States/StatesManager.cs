using System.Collections.Generic;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Multiplayer.Objects;

namespace MultiplayerMod.Multiplayer.States;

[Dependency, UsedImplicitly]
public class StatesManager {

    public virtual void AllowTransition(MultiplayerId choreId, string? targetState, Dictionary<int, object> args) {
        // TODO implement me.
    }

    public virtual void DisableChoreStateTransition(StateMachine.BaseState stateToBeSynced) {
        // TODO implement me.
    }
}
