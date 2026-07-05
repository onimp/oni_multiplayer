using System;
using System.Collections.Generic;
using MultiplayerMod.Multiplayer.Commands;

namespace MultiplayerMod.Network;

public interface IMultiplayerServer {
    void Start();
    void Stop();

    MultiplayerServerState State { get; }
    IMultiplayerEndpoint Endpoint { get; }
    List<IMultiplayerClientId> Clients { get; }

    void Send(IMultiplayerClientId clientId, IMultiplayerCommand command);

    // TODO: Temporary solution for simplification.
    // TODO: Should be extracted into the upper "players" layer later.
    // Send a command to all clients, including host
    void SendAll(IMultiplayerCommand command);

    // Send a command to all clients
    void Send(IMultiplayerCommand command);

    /// <summary>
    /// Force any buffered (Nagle-coalesced) reliable messages onto the wire immediately for every client
    /// connection, instead of waiting for the next network tick. Call this right after sending something
    /// the host must deliver before its own main thread stalls for a while (e.g. the hard-sync save, sent
    /// just before ONI's new-day timelapse screenshot freezes the frame), otherwise the payload sits
    /// un-flushed until the tick resumes and clients wait out the whole stall.
    /// </summary>
    void Flush();

    /// <summary>
    /// Live Steam connection quality aggregated across all client connections (summed rates and reliable
    /// backlog, worst ping/quality), or <c>null</c> when no clients are connected. Used by the hard-sync
    /// overlay to show the host's real upload throughput and how much of the save is still in flight.
    /// </summary>
    ConnectionStats? GetConnectionStats();

    event Action<MultiplayerServerState> StateChanged;
    event Action<IMultiplayerClientId> ClientConnected;
    event Action<IMultiplayerClientId> ClientDisconnected;
    event Action<IMultiplayerClientId, IMultiplayerCommand> CommandReceived;
}
