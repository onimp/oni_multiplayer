using MultiplayerMod.Core.Events;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.Commands;

public class MultiplayerCommandContext {

    public IMultiplayerClientId? ClientId { get; }
    public Runtime Runtime => accessor.Runtime;
    public EventDispatcher EventDispatcher => accessor.EventDispatcher;
    public MultiplayerGame Multiplayer => accessor.Multiplayer;

    private readonly MultiplayerCommandRuntimeAccessor accessor;

    public MultiplayerCommandContext(IMultiplayerClientId? clientId, MultiplayerCommandRuntimeAccessor accessor) {
        this.accessor = accessor;
        ClientId = clientId;
    }

}
