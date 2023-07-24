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

namespace MultiplayerMod.Platform.Steam.Network;

public class SteamClient : IMultiplayerClient {

    public IPlayer Player => playerContainer.Value;
    public MultiplayerClientState State { get; private set; } = MultiplayerClientState.Disconnected;
    public event Action<ClientStateChangedEventArgs>? StateChanged;
    public event Action<CommandReceivedEventArgs>? CommandReceived;

    private readonly Core.Logging.Logger log = LoggerFactory.GetLogger<SteamClient>();
    private readonly SteamLobby lobby = Container.Get<SteamLobby>();
    private readonly Lazy<IPlayer> playerContainer = new(() => new SteamPlayer(SteamUser.GetSteamID()));

    private readonly NetworkMessageProcessor messageProcessor = new();
    private readonly NetworkMessageFactory messageFactory = new();

    private HSteamNetConnection connection = HSteamNetConnection.Invalid;
    private readonly SteamNetworkingConfigValue_t[] networkConfig = { Configuration.SendBufferSize() };

    private GameObject gameObject = null!;

    public void Connect(IMultiplayerEndpoint endpoint) {
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
    }

    public void Disconnect() {
        if (State <= MultiplayerClientState.Disconnected)
            throw new NetworkPlatformException("Client not connected");

        UnityObject.Destroy(gameObject);
        lobby.Leave();
        lobby.OnJoin -= OnLobbyJoin;
        SteamNetworkingSockets.CloseConnection(connection, (int) k_ESteamNetConnectionEnd_App_Generic, "", false);
    }

    public void Tick() {
        if (State != MultiplayerClientState.Connected)
            return;

        SteamNetworkingSockets.RunCallbacks();
        ReceiveCommands();
    }

    public void Send(IMultiplayerCommand command, MultiplayerCommandOptions options = MultiplayerCommandOptions.None) {
        if (State != MultiplayerClientState.Connected)
            throw new NetworkPlatformException("Client not connected");

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
    }

    private void SetState(MultiplayerClientState status) {
        State = status;
        StateChanged?.Invoke(new ClientStateChangedEventArgs(status));
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

    private static SteamNetworkingIdentity GetNetworkingIdentity(CSteamID steamId) {
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
                CommandReceived?.Invoke(new CommandReceivedEventArgs(null, message.Command));
            SteamNetworkingMessage_t.Release(messages[i]);
        }
    }

}
