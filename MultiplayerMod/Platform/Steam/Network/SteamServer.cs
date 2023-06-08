using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using GNS.Sockets;
using MultiplayerMod.Core.Collections;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Extensions;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Core.Scheduling;
using MultiplayerMod.Core.Unity;
using MultiplayerMod.Multiplayer;
using MultiplayerMod.Network;
using MultiplayerMod.Network.Events;
using MultiplayerMod.Platform.Steam.Network.Components;
using MultiplayerMod.Platform.Steam.Network.Messaging;
using Steamworks;
using UnityEngine;
using static Steamworks.Constants;
using static Steamworks.EResult;
using static Steamworks.ESteamNetConnectionEnd;
using static Steamworks.ESteamNetworkingConnectionState;
using HarmonyLib;

namespace MultiplayerMod.Platform.Steam.Network;

[HarmonyPatch(typeof(Global), "OnApplicationQuit")]
public class CleanupPatch
{
    public static void Postfix()
    {
        System.Console.WriteLine("GNS deinitialize!!!!");
        Library.Deinitialize();
    }
};


public class SteamServer : IMultiplayerServer {

    public MultiplayerServerState State { private set; get; } = MultiplayerServerState.Stopped;

    public IMultiplayerEndpoint Endpoint {
        get {
            if (State != MultiplayerServerState.Started)
                throw new NetworkPlatformException("Server isn't started");

#if USE_DEV_NET
            return new DevServerEndpoint();
#else
            return new SteamServerEndpoint(lobby.Id);
#endif
        }
    }

    public List<IPlayer> Players => new(players.Keys);

    public event EventHandler<ServerStateChangedEventArgs> StateChanged;
    public event EventHandler<PlayerConnectedEventArgs> PlayerConnected;
    public event EventHandler<CommandReceivedEventArgs> CommandReceived;

    private readonly Core.Logging.Logger log = LoggerFactory.GetLogger<SteamServer>();

    private Callback<SteamServersConnected_t> steamServersConnectedCallback;
    private TaskCompletionSource<bool> lobbyCompletionSource;
    private TaskCompletionSource<bool> steamServersCompletionSource;
    private CancellationTokenSource callbacksCancellationTokenSource;

    private HSteamNetPollGroup pollGroup;
    private HSteamListenSocket listenSocket;
    private readonly NetworkMessageProcessor messageProcessor = new();
    private readonly NetworkMessageFactory messageFactory = new();
    private readonly SteamNetworkingConfigValue_t[] networkConfig = { Configuration.SendBufferSize() };
    private Callback<SteamNetConnectionStatusChangedCallback_t> connectionStatusChangedCallback;

#if !USE_DEV_NET
    private readonly Dictionary<IPlayer, HSteamNetConnection> players = new();
    private readonly IPlayer currentPlayer = new SteamPlayer(SteamUser.GetSteamID());

    private readonly SteamLobby lobby = Container.Get<SteamLobby>();
#else
    private readonly Dictionary<IPlayer, UInt32> players = new();
    private IPlayer currentPlayer;
#endif
    private GameObject gameObject;

#if USE_DEV_NET
    private NetworkingSockets devServer;
    private uint devPollGroup;
    private uint unidentifiedPollGroup;
    private uint devListenSocket;

    public void StatusCallback(ref StatusInfo info)
    {
        if (devServer == null)
            return;
        switch (info.connectionInfo.state)
        {
            case ConnectionState.None:
                break;
            case ConnectionState.Connecting:
                log.Info($"Server accepting connection {info.connection}");
                devServer.AcceptConnection(info.connection);
                devServer.SetConnectionPollGroup(unidentifiedPollGroup, info.connection);
                break;
            case ConnectionState.Connected:
                log.Info("Client connected - ID: " + info.connection + ", IP: " + info.connectionInfo.address.GetIP());
                break;
            case ConnectionState.ClosedByPeer:
            case ConnectionState.ProblemDetectedLocally:
                devServer.CloseConnection(info.connection);
                log.Info("Client disconnected - ID: " + info.connection + ", IP: " + info.connectionInfo.address.GetIP() + " " + info.connectionInfo.state);
                break;
        }
    }
#endif

    public void Start() {
#if !USE_DEV_NET
        if (!SteamManager.Initialized)
            throw new NetworkPlatformException("Steam API is not initialized");
#endif

        log.Debug("Starting...");
        SetState(MultiplayerServerState.Preparing);
#if USE_DEV_NET
        SetState(MultiplayerServerState.Starting);
        devServer = new NetworkingSockets();
        devPollGroup = devServer.CreatePollGroup();
        unidentifiedPollGroup = devServer.CreatePollGroup();
        Address serverAddress = new Address();
        serverAddress.SetAddress("127.0.0.1", 8081);
        var configuration = new GNS.Sockets.Configuration {
            data = new GNS.Sockets.Configuration.ConfigurationData {Int32 = 10485760}, // 10 MiB
            value = ConfigurationValue.SendBufferSize,
            dataType = ConfigurationDataType.Int32
        };
        var timeoutInitial = new GNS.Sockets.Configuration {
            data = new GNS.Sockets.Configuration.ConfigurationData {Int32 = 10000000},
            value = ConfigurationValue.TimeoutInitial,
            dataType = ConfigurationDataType.Int32,
        };
        var timeoutConnect = new GNS.Sockets.Configuration {
            data = new GNS.Sockets.Configuration.ConfigurationData {Int32 = 10000000},
            value = ConfigurationValue.TimeoutConnected,
            dataType = ConfigurationDataType.Int32,
        };
        devListenSocket = devServer.CreateListenSocket(
            ref serverAddress,
            new GNS.Sockets.Configuration[3] {
                configuration, timeoutInitial, timeoutConnect});
        SetState(MultiplayerServerState.Started);
#else
        try {
            Initialize();
        } catch (Exception) {
            Reset();
            SetState(MultiplayerServerState.Error);
            throw;
        }
#endif
        gameObject = UnityObject.CreateStaticWithComponent<SteamServerComponent>();
    }

    public void Stop() {
        if (State <= MultiplayerServerState.Stopped)
            throw new NetworkPlatformException("Server isn't started");

        log.Debug("Stopping...");
        UnityObject.Destroy(gameObject);
        Reset();
        SetState(MultiplayerServerState.Stopped);
    }

    public void Tick() {
        switch (State) {
            case MultiplayerServerState.Starting:
            case MultiplayerServerState.Started:
#if USE_DEV_NET
                devServer.RunCallbacks();
#else
                GameServer.RunCallbacks();
                SteamGameServerNetworkingSockets.RunCallbacks();
#endif
#if USE_DEV_NET
                IdentifyClients();
                ReceiveDevMessages();
#else
                ReceiveMessages();
#endif
                break;
        }
    }

    public void Send(IPlayer player, IMultiplayerCommand command) {
#if !USE_DEV_NET
        var connections = new SingletonCollection<HSteamNetConnection>(players[player]);
#else
        var connections = new SingletonCollection<uint>(players[player]);
#endif
        SendCommand(command, MultiplayerCommandOptions.None, connections);
    }

    public void Send(IMultiplayerCommand command, MultiplayerCommandOptions options) {
#if !USE_DEV_NET
        IEnumerable<KeyValuePair<IPlayer, HSteamNetConnection>> recipients = players;
#else
        IEnumerable<KeyValuePair<IPlayer, uint>> recipients = players;
#endif
        if (options.HasFlag(MultiplayerCommandOptions.SkipHost))
            recipients = recipients.Where(entry => !entry.Key.Equals(currentPlayer));

        SendCommand(command, options, recipients.Select(it => it.Value));
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

#if !USE_DEV_NET
        lobby.OnCreate += OnLobbyCreated;
        lobby.Create();
#endif

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

        SetState(MultiplayerServerState.Starting);
    }

    private void Reset() {
#if !USE_DEV_NET
        lobby.OnCreate -= OnLobbyCreated;
        lobby.Leave();

        connectionStatusChangedCallback.Unregister();
        SteamGameServerNetworkingSockets.DestroyPollGroup(pollGroup);
        SteamGameServerNetworkingSockets.CloseListenSocket(listenSocket);

        GameServer.Shutdown();
#endif
#if USE_DEV_NET
        devServer.DestroyPollGroup(devPollGroup);
        devServer.DestroyPollGroup(unidentifiedPollGroup);
        log.Info("Destroying poll groups");
#else
        steamServersConnectedCallback.Unregister();

        ResetTaskCompletionSource(ref lobbyCompletionSource);
        ResetTaskCompletionSource(ref steamServersCompletionSource);
        callbacksCancellationTokenSource.Cancel();
#endif
    }

    private void ResetTaskCompletionSource<T>(ref TaskCompletionSource<T> source) {
        source.TrySetCanceled();
        source = null;
    }

    private void OnServerStarted() {
#if !USE_DEV_NET
        lobby.GameServerId = SteamGameServer.GetSteamID();
#endif
        SetState(MultiplayerServerState.Started);
    }

    private void OnLobbyCreated() => lobbyCompletionSource.SetResult(true);

    private void ConnectedToSteamCallback() => steamServersCompletionSource.SetResult(true);

    public void ReceiveMessages() {
        var messages = new IntPtr[128];
        var messagesCount = SteamGameServerNetworkingSockets.ReceiveMessagesOnPollGroup(pollGroup, messages, 128);
        for (var i = 0; i < messagesCount; i++) {
            var steamMessage = Marshal.PtrToStructure<SteamNetworkingMessage_t>(messages[i]);
            var message = messageProcessor.Process(
                steamMessage.m_conn.m_HSteamNetConnection,
                steamMessage.GetNetworkMessageHandle()
            );
            if (message != null) {
                IPlayer player = new SteamPlayer(steamMessage.m_identityPeer.GetSteamID());
                if (message.Options.HasFlag(MultiplayerCommandOptions.ExecuteOnServer)) {
                    CommandReceived?.Invoke(this, new CommandReceivedEventArgs(player, message.Command));
                } else {
                    var connections = players.Where(it => !it.Key.Equals(player)).Select(it => it.Value);
#if !USE_DEV_NET
                    SendCommand(message.Command, message.Options, connections);
#endif
                }
            }
            SteamNetworkingMessage_t.Release(messages[i]);
        }
    }

#if USE_DEV_NET
    private void IdentifyClients()
    {
        const int maxMessages = 20;
        NetworkingMessage[] netMessages = new NetworkingMessage[maxMessages];
        int count = devServer.ReceiveMessagesOnPollGroup(unidentifiedPollGroup, netMessages, maxMessages);
        if (count == 0)
            return;
        for (int i = 0; i < count; ++i)
        {
            ref NetworkingMessage netMessage = ref netMessages[i];
            log.Info("IdentifyClients(): Message received from - ID: " + netMessage.connection 
            + ", Channel ID: " + netMessage.channel 
            + ", Data length: " + netMessage.length);
            var connection = netMessage.connection;
            try
            {
                byte[] buffer = new byte[64];
                netMessage.CopyTo(buffer);
               int pos = Array.IndexOf(buffer, (byte)0);
                if (pos < 0)
                    pos = buffer.Length;
                var identity = System.Text.Encoding.ASCII.GetString(buffer, 0, pos);
                log.Info($"Identified client '{identity}'");
                var connectedPlayer = new DevPlayer(identity);
                if (identity == "server")
                    currentPlayer = connectedPlayer;
                devServer.SetConnectionPollGroup(devPollGroup, connection);
                players[connectedPlayer] = connection;
                PlayerConnected?.Invoke(this, new PlayerConnectedEventArgs(connectedPlayer));
            }
            catch(Exception)
            {
                devServer.CloseConnection(connection);
                log.Info($"Failed to identify client - disconnecting {connection}");
            }
        }
    }

    public void ReceiveDevMessages()
    {
        const int maxMessages = 20;

        NetworkingMessage[] netMessages = new NetworkingMessage[maxMessages];

        int netMessagesCount = devServer.ReceiveMessagesOnPollGroup(devPollGroup, netMessages, maxMessages);
        if (netMessagesCount > 0)
        {
            for (int i = 0; i < netMessagesCount; i++)
            {
                ref NetworkingMessage netMessage = ref netMessages[i];

                log.Info("Message received from - ID: " + netMessage.connection + ", Channel ID: " + netMessage.channel + ", Data length: " + netMessage.length);
                var message = messageProcessor.Process(
                    netMessage.connection,
                    new NetworkMessageHandle(netMessage.data, (uint)netMessage.length)
                );
                log.Info ($"Received message {message}");
                if (message != null)
                {
                    var connection = netMessage.connection;
                    var temp = players.Where(it => it.Value.Equals(connection));
                    log.Info($"temp = {temp}");
                    IPlayer player = temp.Select(it => it.Key).First();
                    log.Info($"player = {player}");
                    log.Info($"executeOnServer = {message.Options.HasFlag(MultiplayerCommandOptions.ExecuteOnServer)}");
                    if (message.Options.HasFlag(MultiplayerCommandOptions.ExecuteOnServer))
                    {
                        CommandReceived?.Invoke(this, new CommandReceivedEventArgs(player, message.Command));
                    } else
                    {
                        var connections = players.Where(it => !it.Key.Equals(player)).Select(it => it.Value);
                        log.Info($"connections = {connections.Count()}: {connections}");
                        SendCommand(message.Command, message.Options, connections);
                    }
                }
                netMessage.Destroy();
            }
        }
    }

    private void SendCommand(
        IMultiplayerCommand command,
        MultiplayerCommandOptions options,
        IEnumerable<uint> connections
    ) {
        var sequence = messageFactory.Create(command, options);
        log.Info($"Sending command to clients: {command} in {sequence.Count()} parts");
        sequence.ForEach(handle => connections.ForEach(it => Send (handle, it)));
    }
#else
    private void SendCommand(
        IMultiplayerCommand command,
        MultiplayerCommandOptions options,
        IEnumerable<HSteamNetConnection> connections
    ) {
        var sequence = messageFactory.Create(command, options);
        sequence.ForEach(handle => connections.ForEach(connection => Send(handle, connection)));
    }
#endif

    private void Send(INetworkMessageHandle handle, HSteamNetConnection connection) {
        var result = SteamGameServerNetworkingSockets.SendMessageToConnection(
            connection,
            handle.Pointer,
            handle.Size,
            k_nSteamNetworkingSend_Reliable,
            out _
        );
        if (result != k_EResultOK)
            log.Error($"Failed to send message, result: {result}");
    }

#if USE_DEV_NET
    private void Send(INetworkMessageHandle handle, uint connection) {
        var result = devServer.SendMessageToConnection(
            connection,
            handle.Pointer,
            handle.Size,
            SendFlags.Reliable);
        if (result != Result.OK)
            log.Error($"Failed to send message, result: {result}");
    }
#endif

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
#if !USE_DEV_NET
        players[new SteamPlayer(clientSteamId)] = connection;
#endif
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
