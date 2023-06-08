using System;
using System.Runtime.InteropServices;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Extensions;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Core.Unity;
using MultiplayerMod.Multiplayer;
using MultiplayerMod.Network;
using MultiplayerMod.Network.Events;
using MultiplayerMod.Platform.Steam.Network.Components;
using MultiplayerMod.Platform.Steam.Network.Messaging;
using Steamworks;
using UnityEngine;
using static Steamworks.Constants;
using static Steamworks.ESteamNetConnectionEnd;
using GNS.Sockets;

namespace MultiplayerMod.Platform.Steam.Network;

public class SteamClient : IMultiplayerClient {

    public IPlayer Player => playerContainer.Value;
    public MultiplayerClientState State { get; private set; } = MultiplayerClientState.Disconnected;
    public event EventHandler<ClientStateChangedEventArgs> StateChanged;
    public event EventHandler<CommandReceivedEventArgs> CommandReceived;

    private readonly Core.Logging.Logger log = LoggerFactory.GetLogger<SteamClient>();
    private readonly SteamLobby lobby = Container.Get<SteamLobby>();

#if USE_DEV_NET
    private readonly Lazy<IPlayer> playerContainer = new(() => new DevPlayer(Identity));
#else
    private readonly Lazy<IPlayer> playerContainer = new(() => new SteamPlayer(SteamUser.GetSteamID()));
#endif

    public static string Identity = "server";

    private readonly NetworkMessageProcessor messageProcessor = new();
    private readonly NetworkMessageFactory messageFactory = new();

    private HSteamNetConnection connection = HSteamNetConnection.Invalid;
    private readonly SteamNetworkingConfigValue_t[] networkConfig = { Configuration.SendBufferSize() };

    private GameObject gameObject;

#if USE_DEV_NET
    private NetworkingSockets devClient;
    private uint devConnection;

    private NetworkingUtils utils;
    private StatusCallback status;
    private DebugCallback debug;

    private int ReconnectAttempts = 0;

    private SteamServer getServer()
    {
        try
        {
            return (SteamServer) Container.Get<IMultiplayerServer>();
        } catch
        {
            return null;
        }
    }
#endif

    public void Connect(IMultiplayerEndpoint endpoint) {
#if USE_DEV_NET
        if (endpoint == null)
            Identity = "client";

        devClient = new NetworkingSockets();

        utils = new NetworkingUtils();
        status = (ref StatusInfo info) => 
        {
            var server = getServer();
            if (info.connection != devConnection)
            {
                if (server != null)
                {
                    server.StatusCallback(ref info);
                }
                return;
            }
            switch (info.connectionInfo.state)
            {
                case ConnectionState.None:
                    break;

                case ConnectionState.Connected:
                    log.Info("Client connected to server - ID: " + info.connection);
                    log.Info($"Sending identity '{Identity}' to server");
                    devClient.SendMessageToConnection(
                        info.connection,
                        System.Text.Encoding.ASCII.GetBytes(Identity),
                        SendFlags.Reliable);
                    SetState(MultiplayerClientState.Connected);
                    break;

                case ConnectionState.ClosedByPeer:
                case ConnectionState.ProblemDetectedLocally:
                    devClient.CloseConnection(info.connection);
                    log.Info($"Client disconnected from server {info.connectionInfo.state}");
                    if (info.connection == devConnection && ReconnectAttempts < 20)
                    {
                        ReconnectAttempts++;
                        log.Info($"Attempting to reconnect {ReconnectAttempts} times.");
                        Address address = new Address();
                        address.SetAddress("127.0.0.1", 8081);
                        devConnection = devClient.Connect(ref address);
                        log.Info($"New decConnection {devConnection}");
                    }
                    else
                    {
                        log.Info($"info.connection : {info.connection} != devConnection : {devConnection} ReconnectAttempts: {ReconnectAttempts}");
                    }
                    break;
            }
        };

        utils.SetStatusCallback(status);
        debug = (DebugType type, string message) => 
        {
            if (type > DebugType.Message)
                return;
            log.Info($"GNS Debug: {type} - {message}");
        };
        // utils.SetDebugCallback(DebugType.Message, debug);
        
        SetState(MultiplayerClientState.Connecting);
        Address address = new Address();
        address.SetAddress("127.0.0.1", 8081);
        log.Info($"Client connecting... with identity '{Identity}'");
        devConnection = devClient.Connect(ref address);
        log.Info($"devConnection = {devConnection}");
        gameObject = UnityObject.CreateStaticWithComponent<SteamClientComponent>();
        // Run callbacks immediately so that the client on the server reacts to the 
        // connection in a timely manner. Otherwise the connection would fail with
        // problem detected locally:
        devClient.RunCallbacks();
#else
        if (!SteamManager.Initialized)
            return;

        var steamServerEndpoint = (SteamServerEndpoint) endpoint;

        SetState(MultiplayerClientState.Connecting);

        SteamNetworkingUtils.GetRelayNetworkStatus(out var status);
        if (status.m_eAvail != ESteamNetworkingAvailability.k_ESteamNetworkingAvailability_Current)
            SteamNetworkingUtils.InitRelayNetworkAccess();

        if (lobby.Connected) {
            OnLobbyJoin();
            return;
        }

        lobby.OnJoin += OnLobbyJoin;
        lobby.Join(steamServerEndpoint.LobbyID);
#endif
    }

    public void Disconnect() {
        if (State <= MultiplayerClientState.Disconnected)
            throw new NetworkPlatformException("Client not connected");

        UnityObject.Destroy(gameObject);
#if !USE_DEV_NET
        lobby.Leave();
        lobby.OnJoin -= OnLobbyJoin;
        SteamNetworkingSockets.CloseConnection(connection, (int) k_ESteamNetConnectionEnd_App_Generic, "", false);
#endif
    }

    public void Tick() {
#if USE_DEV_NET
        devClient.RunCallbacks();
#endif
        if (State != MultiplayerClientState.Connected)
            return;
#if USE_DEV_NET
        ReceiveDevCommands();
#else
        SteamNetworkingSockets.RunCallbacks();
        ReceiveCommands();
#endif
    }

    public void Send(IMultiplayerCommand command, MultiplayerCommandOptions options = MultiplayerCommandOptions.None) {
        if (State != MultiplayerClientState.Connected)
            throw new NetworkPlatformException("Client not connected");
#if !USE_DEV_NET
        messageFactory.Create(command, options).ForEach(
            handle => {
                var result = SteamNetworkingSockets.SendMessageToConnection(
                    connection,
                    handle.Pointer,
                    handle.Size,
                    k_nSteamNetworkingSend_Reliable,
                    out var messageOut
                );
                if (result != EResult.k_EResultOK && messageOut == 0)
                    log.Error($"Failed to send {command}: {result}");
            }
        );
#else
        log.Info($"Sending {command} to server.");
        messageFactory.Create(command, options).ForEach(
            handle => {
                var result = devClient.SendMessageToConnection(
                    devConnection,
                    handle.Pointer,
                    handle.Size,
                    SendFlags.Reliable);
                if (result != Result.OK)
                    log.Error($"Failed to send {command}: {result}");
            }
        );
#endif
    }

    private void SetState(MultiplayerClientState status) {
        State = status;
        StateChanged?.Invoke(this, new ClientStateChangedEventArgs(status));
    }

    private void OnLobbyJoin() {
        var serverId = lobby.GameServerId;
        if (serverId == CSteamID.Nil) {
            log.Error("Unable to get lobby game server");
            SetState(MultiplayerClientState.Error);
            return;
        }
        log.Debug($"Lobby game server is {serverId}");
        var identity = GetNetworkingIdentity(serverId);
        connection = SteamNetworkingSockets.ConnectP2P(ref identity, 0, networkConfig.Length, networkConfig);

        log.Debug($"P2P Connect to {serverId}");

        SetRichPresence();
        SetState(MultiplayerClientState.Connected);

        gameObject = UnityObject.CreateStaticWithComponent<SteamClientComponent>();
    }

    private void SetRichPresence() {
        SteamFriends.SetRichPresence("connect", $"+connect_lobby {lobby.Id}");
    }

    private SteamNetworkingIdentity GetNetworkingIdentity(CSteamID steamId) {
        var identity = new SteamNetworkingIdentity();
        identity.SetSteamID(steamId);
        return identity;
    }

    private void ReceiveCommands() {
        var messages = new IntPtr[128];
        var messagesCount = SteamNetworkingSockets.ReceiveMessagesOnConnection(connection, messages, 128);
        for (var i = 0; i < messagesCount; i++) {
            var steamMessage = Marshal.PtrToStructure<SteamNetworkingMessage_t>(messages[i]);
            var message = messageProcessor.Process(
                steamMessage.m_conn.m_HSteamNetConnection,
                steamMessage.GetNetworkMessageHandle()
            );
            if (message != null)
                CommandReceived?.Invoke(this, new CommandReceivedEventArgs(null, message.Command));
            SteamNetworkingMessage_t.Release(messages[i]);
        }
    }

#if USE_DEV_NET
    private void ReceiveDevCommands() {
        const int maxMessages = 20;

        NetworkingMessage[] netMessages = new NetworkingMessage[maxMessages];
        int netMessagesCount = devClient.ReceiveMessagesOnConnection(devConnection, netMessages, maxMessages);

        if (netMessagesCount > 0)
        {
          log.Info($"netMessagesCount = {netMessagesCount}");
          for (int i = 0; i < netMessagesCount; i++)
          {
            ref NetworkingMessage netMessage = ref netMessages[i];

            log.Info("Message received from server - Channel ID: " + netMessage.channel + ", Data length: " + netMessage.length);
            var message = messageProcessor.Process(
                devConnection,
                new NetworkMessageHandle(netMessage.data, (uint)netMessage.length)
            );
            if (message != null)
            {
                log.Info($"Received message: {message}");
                CommandReceived?.Invoke(this, new CommandReceivedEventArgs(null, message.Command));
            }
            else
            {
                log.Info("messageProcessor.Process returned null!");
            }

            netMessage.Destroy();
          }
        }
    }
#endif
}
