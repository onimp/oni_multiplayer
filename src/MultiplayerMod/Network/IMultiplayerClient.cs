using System;
using MultiplayerMod.Multiplayer.Commands;

namespace MultiplayerMod.Network;

public interface IMultiplayerClient {
    MultiplayerClientState State { get; }
    IMultiplayerClientId Id { get; }

    void Connect(IMultiplayerEndpoint endpoint);
    void Disconnect();

    void Send(IMultiplayerCommand command, MultiplayerCommandOptions options = MultiplayerCommandOptions.None);

    /// <summary>
    /// Force any buffered (Nagle-coalesced) reliable messages onto the wire immediately, instead of
    /// waiting for the next network tick. Call this before the calling thread is about to stall for a
    /// long time (e.g. the hard-sync scene reload), otherwise the last messages sit un-flushed until the
    /// tick resumes and remote peers see stale state for the whole stall.
    /// </summary>
    void Flush();

    /// <summary>
    /// Live Steam connection quality for the link to the host (rates, ping, reliable backlog), or
    /// <c>null</c> when not connected or unavailable. Used by the hard-sync overlay for real transfer
    /// diagnostics. Note: values only advance while the network tick runs, so they freeze during a
    /// main-thread stall (e.g. the scene reload) — a frozen readout is itself a useful signal.
    /// </summary>
    ConnectionStats? GetConnectionStats();

    event Action<MultiplayerClientState> StateChanged;
    event Action<IMultiplayerCommand> CommandReceived;
}
