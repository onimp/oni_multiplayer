using System.Collections.Generic;

namespace MultiplayerMod.Multiplayer.States;

public interface IWaitHostState {
    void AllowTransition(StateMachine.Instance smi, string? target, Dictionary<int, object?> args);
}
