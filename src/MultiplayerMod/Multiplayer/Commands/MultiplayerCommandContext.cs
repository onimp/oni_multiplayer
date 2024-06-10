using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Events;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.Commands;

public class MultiplayerCommandContext(IMultiplayerClientId? clientId, MultiplayerCommandRuntimeAccessor accessor) {
    public IMultiplayerClientId? ClientId { get; } = clientId;
    public Runtime Runtime => accessor.Runtime;
    public IDependencyContainer Dependencies => accessor.Dependencies;
    public EventDispatcher EventDispatcher => accessor.EventDispatcher;
    public MultiplayerGame Multiplayer => accessor.Multiplayer;
}
