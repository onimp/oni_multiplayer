using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MultiplayerMod.Core.Collections;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Extensions;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Core.Scheduling;
using MultiplayerMod.Core.Unity;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Multiplayer.Commands.Registry;
using MultiplayerMod.Network;
using MultiplayerMod.Platform.Steam.Network.Components;
using MultiplayerMod.Platform.Steam.Network.Messaging;
using Steamworks;
using UnityEngine;
using static Steamworks.Constants;
using static Steamworks.EResult;
using static Steamworks.ESteamNetConnectionEnd;
using static Steamworks.ESteamNetworkingConnectionState;

namespace MultiplayerMod.Platform.Steam.Network;

[Dependency, UsedImplicitly]
public class SteamServer : IMultiplayerServer {

    public MultiplayerServerState State { private set; get; } = MultiplayerServerState.Stopped;

    public IMultiplayerEndpoint Endpoint {
        get {
            if (State != MultiplayerServerState.Started)
                throw new NetworkPlatformException("Server isn't started");

            return new SteamServerEndpoint(lobby.Id);
        }
    }

    public List<IMultiplayerClientId> Clients => new(clients.Select(it => it.Key));

    public event Action<MultiplayerServerState>? StateChanged;
    public event Action<IMultiplayerClientId>? ClientConnected;
    public event Action<IMultiplayerClientId>? ClientDisconnected;
    public event Action<IMultiplayerClientId, IMultiplayerCommand>? CommandReceived;

    private readonly Core.Logging.Logger log = LoggerFactory.GetLogger<SteamServer>();

    private Callback<SteamServersConnected_t> steamServersConnectedCallback = null!;
    private TaskCompletionSource<bool> lobbyCompletionSource = null!;
    private TaskCompletionSource<bool> steamServersCompletionSource = null!;
    private CancellationTokenSource callbacksCancellationTokenSource = null!;

    private HSteamNetPollGroup pollGroup;
    private HSteamListenSocket listenSocket;
    private readonly NetworkMessageProcessor messageProcessor = new();
    private readonly NetworkMessageFactory messageFactory = new();
    private SteamNetworkingMessageSender sender = null!;
    private readonly HashSet<HSteamNetConnection> lanedConnections = new();
    private readonly SteamNetworkingConfigValue_t[] networkConfig = {
        Configuration.SendBufferSize(), Configuration.SendRateMin(), Configuration.SendRateMax(),
        Configuration.IceEnable()
    };
    private Callback<SteamNetConnectionStatusChangedCallback_t> connectionStatusChangedCallback = null!;

    private readonly Dictionary<IMultiplayerClientId, HSteamNetConnection> clients = new();
    private readonly IMultiplayerClientId currentPlayer = new SteamMultiplayerClientId(SteamUser.GetSteamID());

    private readonly UnityTaskScheduler scheduler;
    private readonly MultiplayerCommandRegistry commands;
    private readonly SteamLobby lobby;

    private GameObject? gameObject;

    public SteamServer(SteamLobby lobby, UnityTaskScheduler scheduler, MultiplayerCommandRegistry commands) {
        this.lobby = lobby;
        this.scheduler = scheduler;
        this.commands = commands;
    }

    public void Start() {
        if (!SteamManager.Initialized)
            throw new NetworkPlatformException("Steam API is not initialized");

        log.Debug("Starting...");
        SetState(MultiplayerServerState.Preparing);
        try {
            Initialize();
        } catch (Exception) {
            Reset();
            SetState(MultiplayerServerState.Error);
            throw;
        }
        gameObject = UnityObject.CreateStaticWithComponent<SteamServerComponent>();
    }

    public void Stop() {
        if (State <= MultiplayerServerState.Stopped)
            throw new NetworkPlatformException("Server isn't started");

        log.Debug("Stopping...");
        if (gameObject != null)
            UnityObject.Destroy(gameObject);
        Reset();
        SetState(MultiplayerServerState.Stopped);
    }

    public void Tick() {
        switch (State) {
            case MultiplayerServerState.Starting:
            case MultiplayerServerState.Started:
                GameServer.RunCallbacks();
                SteamGameServerNetworkingSockets.RunCallbacks();
                ReceiveMessages();
                break;
        }
    }

    public void Send(IMultiplayerClientId clientId, IMultiplayerCommand command) {
        var connections = new SingletonCollection<HSteamNetConnection>(clients[clientId]);
        SendCommand(command, MultiplayerCommandOptions.None, connections);
    }

    public void SendAll(IMultiplayerCommand command) => Send(command, MultiplayerCommandOptions.None);

    public void Send(IMultiplayerCommand command) => Send(command,MultiplayerCommandOptions.SkipHost);

    public void Flush() {
        if (State != MultiplayerServerState.Started)
            return;

        // Push everything Steam has buffered for each client connection (across all lanes) onto the wire now.
        foreach (var connection in clients.Values) {
            var result = SteamGameServerNetworkingSockets.FlushMessagesOnConnection(connection);
            if (result != k_EResultOK)
                log.Warning($"FlushMessagesOnConnection returned {result}");
        }
    }

    public ConnectionStats? GetConnectionStats() {
        if (State != MultiplayerServerState.Started || clients.Count == 0)
            return null;

        float outRate = 0, inRate = 0, quality = 1f;
        int ping = 0, pending = 0;
        var sampled = false;
        foreach (var connection in clients.Values) {
            var status = new SteamNetConnectionRealTimeStatus_t();
            var lanes = new SteamNetConnectionRealTimeLaneStatus_t(); // unused: nLanes = 0
            var result = SteamGameServerNetworkingSockets.GetConnectionRealTimeStatus(
                connection, ref status, 0, ref lanes
            );
            if (result != k_EResultOK)
                continue;

            sampled = true;
            outRate += status.m_flOutBytesPerSec;
            inRate += status.m_flInBytesPerSec;
            pending += status.m_cbPendingReliable;
            ping = Math.Max(ping, status.m_nPing);
            quality = Math.Min(quality, status.m_flConnectionQualityLocal);
        }

        return sampled ? new ConnectionStats(ping, outRate, inRate, pending, quality) : null;
    }

    private void Send(IMultiplayerCommand command, MultiplayerCommandOptions options) {
        IEnumerable<KeyValuePair<IMultiplayerClientId, HSteamNetConnection>> recipients = clients;
        if (options.HasFlag(MultiplayerCommandOptions.SkipHost))
            recipients = recipients.Where(entry => !entry.Key.Equals(currentPlayer));

        SendCommand(command, options, recipients.Select(it => it.Value));
    }

    private void SetState(MultiplayerServerState state) {
        State = state;
        StateChanged?.Invoke(state);
    }

    private void Initialize() {
        steamServersConnectedCallback = Callback<SteamServersConnected_t>
            .CreateGameServer(_ => ConnectedToSteamCallback());

        lobbyCompletionSource = new TaskCompletionSource<bool>();
        steamServersCompletionSource = new TaskCompletionSource<bool>();
        callbacksCancellationTokenSource = new CancellationTokenSource();
        Task.WhenAll(lobbyCompletionSource.Task, steamServersCompletionSource.Task)
            .ContinueWith(
                _ => OnServerStarted(),
                callbacksCancellationTokenSource.Token,
                TaskContinuationOptions.None,
                scheduler
            );

        lobby.OnCreate += OnLobbyCreated;
        lobby.Create();

        var version = typeof(SteamServer).Assembly.GetName().Version.ToString();
        log.Debug($"Initializing game server version {version}");
        if (!GameServer.Init(0, 27020, 27015, EServerMode.eServerModeNoAuthentication, version))
            throw new NetworkPlatformException("Game server init failed");

        SteamGameServer.SetModDir("OxygenNotIncluded");
        SteamGameServer.SetProduct("OxygenNotIncluded Multiplayer");
        SteamGameServer.SetGameDescription("OxygenNotIncluded Multiplayer");

        SteamGameServer.LogOnAnonymous();

        SteamNetworkingUtils.GetRelayNetworkStatus(out var status);
        if (status.m_eAvail != ESteamNetworkingAvailability.k_ESteamNetworkingAvailability_Current)
            SteamNetworkingUtils.InitRelayNetworkAccess();

        connectionStatusChangedCallback = Callback<SteamNetConnectionStatusChangedCallback_t>
            .CreateGameServer(HandleConnectionStatusChanged);

        listenSocket = SteamGameServerNetworkingSockets.CreateListenSocketP2P(0, networkConfig.Length, networkConfig);
        pollGroup = SteamGameServerNetworkingSockets.CreatePollGroup();
        sender = SteamNetworkingMessageSender.ForServer();

        SetState(MultiplayerServerState.Starting);
    }

    private void Reset() {
        lobby.OnCreate -= OnLobbyCreated;
        lobby.Leave();

        connectionStatusChangedCallback.Unregister();
        SteamGameServerNetworkingSockets.DestroyPollGroup(pollGroup);
        SteamGameServerNetworkingSockets.CloseListenSocket(listenSocket);

        GameServer.Shutdown();

        steamServersConnectedCallback.Unregister();

        lobbyCompletionSource.TrySetCanceled();
        steamServersCompletionSource.TrySetCanceled();
        callbacksCancellationTokenSource.Cancel();
    }

    private void OnServerStarted() {
        // Runs as a main-thread scheduled continuation once the lobby + Steam-servers handshakes both complete.
        // Between those completing and this executing, the server can have been torn down or superseded: Stop()
        // / a failed Start() / a rapid restart all call Reset(), which GameServer.Shutdown()s and cancels the
        // continuation token. If we then call SteamGameServer.GetSteamID() the Steamworks wrapper throws
        // "Steamworks GameServer is not initialized" - and because this executes inside UnityTaskScheduler.Tick,
        // an uncaught throw is rethrown as a hard ONI crash. Bail if this start was cancelled or is no longer the
        // one coming up, and treat a genuinely unavailable GameServer as an errored start instead of crashing.
        if (callbacksCancellationTokenSource.IsCancellationRequested || State != MultiplayerServerState.Starting) {
            log.Warning("Server start completed after teardown/cancellation; skipping OnServerStarted");
            return;
        }

        try {
            lobby.GameServerId = SteamGameServer.GetSteamID();
        } catch (Exception exception) {
            log.Error($"Server start aborted - GameServer unavailable: {exception.Message}");
            SetState(MultiplayerServerState.Error);
            return;
        }
        SetState(MultiplayerServerState.Started);
    }

    private void OnLobbyCreated() {
        SteamMatchmaking.SetLobbyData(lobby.Id, "server.name", $"{SteamFriends.GetPersonaName()}");
        lobbyCompletionSource.SetResult(true);
    }

    private void ConnectedToSteamCallback() => steamServersCompletionSource.SetResult(true);

    private void ReceiveMessages() {
        var messages = new IntPtr[128];
        var messagesCount = SteamGameServerNetworkingSockets.ReceiveMessagesOnPollGroup(pollGroup, messages, 128);
        for (var i = 0; i < messagesCount; i++) {
            var steamMessage = Marshal.PtrToStructure<SteamNetworkingMessage_t>(messages[i]);
            var message = messageProcessor.Process(
                steamMessage.m_conn.m_HSteamNetConnection,
                steamMessage.GetNetworkMessageHandle()
            );
            if (message != null) {
                IMultiplayerClientId id = new SteamMultiplayerClientId(steamMessage.m_identityPeer.GetSteamID());
                var configuration = commands.GetCommandConfiguration(message.Command.GetType());
                if (configuration.ExecuteOnServer) {
                    CommandReceived?.Invoke(id, message.Command);
                } else {
                    var connections = clients.Where(it => !it.Key.Equals(id)).Select(it => it.Value);
                    SendCommand(message.Command, message.Options, connections);
                }
            }
            SteamNetworkingMessage_t.Release(messages[i]);
        }
    }

    private void SendCommand(
        IMultiplayerCommand command,
        MultiplayerCommandOptions options,
        IEnumerable<HSteamNetConnection> connections
    ) {
        var lane = commands.GetCommandConfiguration(command.GetType()).Lane;
        var flags = Configuration.SendFlags(lane);
        var sequence = messageFactory.Create(command, options);
        sequence.ForEach(handle => connections.ForEach(connection => Send(handle, connection, flags, lane)));
    }

    private void Send(INetworkMessageHandle handle, HSteamNetConnection connection, int flags, NetworkLane lane) {
        // Lane-aware send when the connection's lanes are configured; otherwise (or if the native lane
        // send fails) fall back to the flat API - still honours the reliability flag, only cross-lane
        // prioritisation is lost. sender.Send only returns false when nothing was queued, so this cannot
        // duplicate a message.
        if (sender.Available && lanedConnections.Contains(connection) && sender.Send(handle, connection, flags, lane))
            return;

        var result = SendRaw(handle, connection, flags);
        if (result == k_EResultOK)
            return;

        // A full send buffer (k_EResultLimitExceeded) DROPS the message rather than blocking, which for a
        // hard-sync fragment stalls the client's reassembly forever. Flush the connection to drain some of
        // the backlog onto the wire and retry once. This is a best-effort safety net on top of the large
        // SendBufferSize; the WorldManager hard-sync watchdog re-syncs if a fragment is still lost.
        if (result == k_EResultLimitExceeded) {
            SteamGameServerNetworkingSockets.FlushMessagesOnConnection(connection);
            result = SendRaw(handle, connection, flags);
            if (result == k_EResultOK)
                return;
            log.Error(
                $"Send buffer overflow (k_EResultLimitExceeded) persisted after flush+retry, message DROPPED " +
                $"({handle.Size} bytes); a hard-sync fragment may be lost. Increase SendBufferSize."
            );
            return;
        }

        log.Error($"Failed to send message, result: {result}");
    }

    private EResult SendRaw(INetworkMessageHandle handle, HSteamNetConnection connection, int flags) =>
        SteamGameServerNetworkingSockets.SendMessageToConnection(
            connection,
            handle.Pointer,
            handle.Size,
            flags,
            out _
        );

    private void HandleConnectionStatusChanged(SteamNetConnectionStatusChangedCallback_t data) {
        var connection = data.m_hConn;
        var clientSteamId = data.m_info.m_identityRemote.GetSteamID();
        var state = data.m_info.m_eState;
        switch (state) {
            case k_ESteamNetworkingConnectionState_Connecting:
                if (TryAcceptConnection(connection, clientSteamId))
                    ClientConnected?.Invoke(new SteamMultiplayerClientId(clientSteamId));
                break;
            case k_ESteamNetworkingConnectionState_ProblemDetectedLocally:
            case k_ESteamNetworkingConnectionState_ClosedByPeer:
                CloseConnection(connection, clientSteamId);
                break;
            default:
                log.Trace(
                    () => $"Unhandled connection state change {data.m_eOldState} -> {state} for client {clientSteamId}"
                );
                break;
        }
    }

    private bool TryAcceptConnection(HSteamNetConnection connection, CSteamID clientSteamId) {
        var result = SteamGameServerNetworkingSockets.AcceptConnection(connection);
        if (result != k_EResultOK) {
            log.Error($"Unable to accept connection from {clientSteamId} (error {result})");
            var reason = (int) k_ESteamNetConnectionEnd_AppException_Generic + (int) result;
            SteamGameServerNetworkingSockets.CloseConnection(
                connection,
                reason,
                pszDebug: $"Failed to accept connection (error {result})",
                bEnableLinger: false
            );
            return false;
        }
        SteamGameServerNetworkingSockets.SetConnectionPollGroup(connection, pollGroup);
        clients[new SteamMultiplayerClientId(clientSteamId)] = connection;
        if (Configuration.ConfigureServerLanes(connection))
            lanedConnections.Add(connection);
        log.Debug($"Connection accepted from {clientSteamId}");
        return true;
    }

    private void CloseConnection(HSteamNetConnection connection, CSteamID clientSteamId) {
        lanedConnections.Remove(connection);
        ClientDisconnected?.Invoke(new SteamMultiplayerClientId(clientSteamId));
        SteamGameServerNetworkingSockets.CloseConnection(
            connection,
            (int) k_ESteamNetConnectionEnd_App_Generic,
            null,
            false
        );
        clients.Remove(new SteamMultiplayerClientId(clientSteamId));
        Debug.Log($"Connection closed for {clientSteamId}");
    }

}
