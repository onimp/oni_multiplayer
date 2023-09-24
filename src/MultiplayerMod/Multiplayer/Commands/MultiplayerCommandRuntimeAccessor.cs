using MultiplayerMod.Core.Events;
using MultiplayerMod.ModRuntime;

namespace MultiplayerMod.Multiplayer.Commands;

public class MultiplayerCommandRuntimeAccessor {

    public Runtime Runtime { get; }
    public EventDispatcher EventDispatcher { get; }
    public MultiplayerGame Multiplayer { get; }

    public MultiplayerCommandRuntimeAccessor(Runtime runtime) {
        Runtime = runtime;
        EventDispatcher = runtime.Dependencies.Get<EventDispatcher>();
        Multiplayer = runtime.Dependencies.Get<MultiplayerGame>();
    }

}
