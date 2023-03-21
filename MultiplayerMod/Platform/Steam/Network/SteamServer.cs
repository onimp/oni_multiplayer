using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Extensions;
using MultiplayerMod.Core.Scheduling;
using MultiplayerMod.Core.Unity;
using MultiplayerMod.Multiplayer;
using MultiplayerMod.Network;
using MultiplayerMod.Network.Events;
using MultiplayerMod.Network.Messaging;
using MultiplayerMod.Platform.Steam.Network.Components;
using Steamworks;
using UnityEngine;
using static Steamworks.Constants;
using static Steamworks.EResult;
using static Steamworks.ESteamNetConnectionEnd;
using static Steamworks.ESteamNetworkingConnectionState;

namespace MultiplayerMod.Platform.Steam.Network;

public class SteamServer : IMultiplayerServer {

    public MultiplayerServerState State { private set; get; } = MultiplayerServerState.Stopped;

    public IMultiplayerEndpoint Endpoint {
        get {
            if (State != MultiplayerServerState.Started)
                throw new NetworkPlatformException("Server isn't started");

            return new SteamServerEndpoint(lobby.Id);
        }
    }

    public List<IPlayer> Players => new(players.Keys);

    public event EventHandler<ServerStateChangedEventArgs> StateChanged;
    public event EventHandler<PlayerConnectedEventArgs> PlayerConnected;
    public event EventHandler<CommandReceivedEventArgs> CommandReceived;

    private readonly Core.Logging.Logger log = new(typeof(SteamServer));

    private Callback<SteamServersConnected_t> steamServersConnectedCallback;
    private TaskCompletionSource<bool> lobbyCompletionSource;
    private TaskCompletionSource<bool> steamServersCompletionSource;
    private CancellationTokenSource callbacksCancellationTokenSource;

    private HSteamNetPollGroup pollGroup;
    private HSteamListenSocket listenSocket;
    private readonly SteamNetworkingConfigValue_t[] networkConfig = { Configuration.SendBufferSize() };
    private Callback<SteamNetConnectionStatusChangedCallback_t> connectionStatusChangedCallback;

    private readonly Dictionary<IPlayer, HSteamNetConnection> players = new();
    private readonly IPlayer currentPlayer = new SteamPlayer(SteamUser.GetSteamID());

    private readonly SteamLobby lobby = Container.Get<SteamLobby>();

    private GameObject gameObject;

    public void Start() {
        if (!SteamManager.Initialized)
            throw new NetworkPlatformException("Steam API is not initialized");

        log.Debug("Starting...");
        SetState(MultiplayerServerState.Starting);
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
        if (State != MultiplayerServerState.Started)
            throw new NetworkPlatformException("Server isn't started");

        log.Debug("Stopping...");
        UnityObject.Destroy(gameObject);
        Reset();
        SetState(MultiplayerServerState.Stopped);
    }

    public void Tick() {
        if (State != MultiplayerServerState.Started)
            return;

        GameServer.RunCallbacks();
        SteamGameServerNetworkingSockets.RunCallbacks();
        ReceiveMessages();
    }

    public void Send(IPlayer player, IMultiplayerCommand command) {
        using var handle = Serialize(command, MultiplayerCommandOptions.None);
        Send(players[player], handle);
    }

    public void Send(IMultiplayerCommand command, MultiplayerCommandOptions options) {
        using var handle = Serialize(command, options);

        IEnumerable<KeyValuePair<IPlayer, HSteamNetConnection>> recipients = players;
        if (options.HasFlag(MultiplayerCommandOptions.SkipHost))
            recipients = recipients.Where(entry => entry.Key != currentPlayer);

        // ReSharper disable once AccessToDisposedClosure
        recipients.ForEach(it => Send(it.Value, handle));
    }

    private INetworkMessageHandle Serialize(IMultiplayerCommand command, MultiplayerCommandOptions options) {
        var message = new NetworkMessage(command, options);
        return NetworkSerializer.Serialize(message);
    }

    private void SetState(MultiplayerServerState state) {
        State = state;
        StateChanged?.Invoke(this, new ServerStateChangedEventArgs(state));
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
                Container.Get<UnityTaskScheduler>()
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
    }

    private void Reset() {
        lobby.OnCreate -= OnLobbyCreated;
        lobby.Leave();

        connectionStatusChangedCallback.Unregister();
        SteamGameServerNetworkingSockets.DestroyPollGroup(pollGroup);
        SteamGameServerNetworkingSockets.CloseListenSocket(listenSocket);

        GameServer.Shutdown();

        steamServersConnectedCallback.Unregister();

        ResetTaskCompletionSource(ref lobbyCompletionSource);
        ResetTaskCompletionSource(ref steamServersCompletionSource);
        callbacksCancellationTokenSource.Cancel();
    }

    private void ResetTaskCompletionSource<T>(ref TaskCompletionSource<T> source) {
        source.TrySetCanceled();
        source = null;
    }

    private void OnServerStarted() => SetState(MultiplayerServerState.Started);

    private void OnLobbyCreated() => lobbyCompletionSource.SetResult(true);

    private void ConnectedToSteamCallback() => steamServersCompletionSource.SetResult(true);

    public void ReceiveMessages() {
        var messages = new IntPtr[128];
        var messagesCount = SteamGameServerNetworkingSockets.ReceiveMessagesOnPollGroup(pollGroup, messages, 128);
        for (var i = 0; i < messagesCount; i++) {
            var steamMessage = (SteamNetworkingMessage_t) Marshal.PtrToStructure(
                messages[i],
                typeof(SteamNetworkingMessage_t)
            );
            var handle = steamMessage.GetNetworkMessageHandle();
            var message = NetworkSerializer.Deserialize(handle);

            IPlayer player = new SteamPlayer(steamMessage.m_identityPeer.GetSteamID());
            if (!message.Options.HasFlag(MultiplayerCommandOptions.ExecuteOnServer))
                players.Where(it => it.Key != player).ForEach(it => Send(it.Value, handle));
            else
                CommandReceived?.Invoke(this, new CommandReceivedEventArgs(player, message.Command));
            SteamNetworkingMessage_t.Release(messages[i]);
        }
    }

    private void Send(HSteamNetConnection connection, INetworkMessageHandle handle) {
        var result = SteamGameServerNetworkingSockets.SendMessageToConnection(
            connection,
            handle.GetPointer(),
            handle.GetSize(),
            k_nSteamNetworkingSend_Reliable,
            out _
        );
        if (result != k_EResultOK)
            log.Error($"Failed to send message, result: {result}");
    }

    private void HandleConnectionStatusChanged(SteamNetConnectionStatusChangedCallback_t data) {
        var connection = data.m_hConn;
        var clientSteamId = data.m_info.m_identityRemote.GetSteamID();
        var state = data.m_info.m_eState;
        switch (state) {
            case k_ESteamNetworkingConnectionState_Connecting:
                if (TryAcceptConnection(connection, clientSteamId))
                    PlayerConnected?.Invoke(this, new PlayerConnectedEventArgs(new SteamPlayer(clientSteamId)));
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
        players[new SteamPlayer(clientSteamId)] = connection;
        log.Debug($"Connection accepted from {clientSteamId}");
        return true;
    }

    private void CloseConnection(HSteamNetConnection connection, CSteamID clientSteamId) {
        SteamGameServerNetworkingSockets.CloseConnection(
            connection,
            nReason: (int) k_ESteamNetConnectionEnd_App_Generic,
            pszDebug: null,
            bEnableLinger: false
        );
        players.Remove(new SteamPlayer(clientSteamId));
        Debug.Log($"Connection closed for {clientSteamId}");
    }

}
