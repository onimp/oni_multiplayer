using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Events;
using MultiplayerMod.ModRuntime;

namespace MultiplayerMod.Multiplayer.Commands;

public class MultiplayerCommandRuntimeAccessor(Runtime runtime) {
    public Runtime Runtime { get; } = runtime;
    public IDependencyContainer Dependencies { get; } = runtime.Dependencies;
    public EventDispatcher EventDispatcher { get; } = runtime.Dependencies.Get<EventDispatcher>();
    public MultiplayerGame Multiplayer { get; } = runtime.Dependencies.Get<MultiplayerGame>();
}
