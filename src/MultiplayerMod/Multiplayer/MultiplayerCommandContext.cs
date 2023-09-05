using System;
using System.Threading;
using MultiplayerMod.Core.Events;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.Multiplayer.State;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer;

public class MultiplayerCommandContext {

    public IMultiplayerClientId? ClientId { get; }
    public Runtime Runtime { get; }

    private readonly Lazy<EventDispatcher> eventDispatcher;
    private readonly Lazy<MultiplayerGame> multiplayer;

    public EventDispatcher EventDispatcher => eventDispatcher.Value;
    public MultiplayerGame Multiplayer => multiplayer.Value;

    public MultiplayerCommandContext(IMultiplayerClientId? clientId, Runtime runtime) {
        Runtime = runtime;
        ClientId = clientId;
        eventDispatcher = new Lazy<EventDispatcher>(
            () => Runtime.Dependencies.Get<EventDispatcher>(),
            LazyThreadSafetyMode.None
        );
        multiplayer = new Lazy<MultiplayerGame>(
            () => Runtime.Dependencies.Get<MultiplayerGame>(),
            LazyThreadSafetyMode.None
        );
    }

}
