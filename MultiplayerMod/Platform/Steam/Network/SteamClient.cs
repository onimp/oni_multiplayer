using System;
using System.Runtime.InteropServices;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Unity;
using MultiplayerMod.Multiplayer;
using MultiplayerMod.Network;
using MultiplayerMod.Network.Events;
using MultiplayerMod.Network.Messaging;
using MultiplayerMod.Platform.Steam.Network.Components;
using Steamworks;
using UnityEngine;
using static Steamworks.Constants;
using static Steamworks.ESteamNetConnectionEnd;
using Sockets = Steamworks.SteamNetworkingSockets;

namespace MultiplayerMod.Platform.Steam.Network;

public class SteamClient : IMultiplayerClient {

    public IPlayer Player => playerContainer.Value;
    public MultiplayerClientState State { get; private set; } = MultiplayerClientState.Disconnected;
    public event EventHandler<ClientStateChangedEventArgs> StateChanged;
    public event EventHandler<CommandReceivedEventArgs> CommandReceived;

    private readonly Core.Logging.Logger log = new(typeof(SteamClient));
    private readonly SteamLobby lobby = Container.Get<SteamLobby>();
    private readonly Lazy<IPlayer> playerContainer = new(() => new SteamPlayer(SteamUser.GetSteamID()));

    private HSteamNetConnection connection = HSteamNetConnection.Invalid;
    private readonly SteamNetworkingConfigValue_t[] networkConfig = { Configuration.SendBufferSize() };

    private GameObject gameObject;

    public void Connect(IMultiplayerEndpoint endpoint) {
        if (!SteamManager.Initialized)
            return;

        var steamServerEndpoint = (SteamServerEndpoint) endpoint;

        SetState(MultiplayerClientState.Connecting);

        SteamNetworkingUtils.InitRelayNetworkAccess();

        if (lobby.Connected) {
            OnLobbyJoin();
            return;
        }

        lobby.Join(steamServerEndpoint.LobbyID);
        lobby.OnJoin += OnLobbyJoin;
    }

    public void Disconnect() {
        if (State != MultiplayerClientState.Connected)
            throw new NetworkPlatformException("Client not connected");

        UnityObject.Destroy(gameObject);
        lobby.Leave();
        lobby.OnJoin -= OnLobbyJoin;
        Sockets.CloseConnection(connection, (int) k_ESteamNetConnectionEnd_App_Generic, "", false);
    }

    public void Tick() {
        if (State != MultiplayerClientState.Connected)
            return;

        Sockets.RunCallbacks();
        ReceiveCommands();
    }

    public void Send(IMultiplayerCommand command, MultiplayerCommandOptions options = MultiplayerCommandOptions.None) {
        if (State != MultiplayerClientState.Connected)
            throw new NetworkPlatformException("Client not connected");

        using var data = NetworkSerializer.Serialize(new NetworkMessage(command, options));
        var result = Sockets.SendMessageToConnection(
            connection,
            data.GetPointer(),
            data.GetSize(),
            k_nSteamNetworkingSend_Reliable,
            out var messageOut
        );
        if (result != EResult.k_EResultOK && messageOut == 0)
            log.Error($"Failed to send {command}: {result}");
    }

    private void SetState(MultiplayerClientState status) {
        State = status;
        StateChanged?.Invoke(this, new ClientStateChangedEventArgs(status));
    }

    private void OnLobbyJoin() {
        var result = SteamMatchmaking.GetLobbyGameServer(lobby.Id, out _, out _, out var serverId);
        if (!result) {
            log.Error("Unable to get lobby game server");
            SetState(MultiplayerClientState.Error);
            return;
        }
        log.Debug($"Lobby game server is {serverId}");
        var identity = GetNetworkingIdentity(serverId);
        connection = Sockets.ConnectP2P(ref identity, 0, networkConfig.Length, networkConfig);

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
        var messagesCount = Sockets.ReceiveMessagesOnConnection(connection, messages, 128);
        for (var i = 0; i < messagesCount; i++) {
            var message = (SteamNetworkingMessage_t) Marshal.PtrToStructure(
                messages[i],
                typeof(SteamNetworkingMessage_t)
            );
            var data = NetworkSerializer.Deserialize(message.GetNetworkMessageHandle());
            var player = new SteamPlayer(message.m_identityPeer.GetSteamID());
            CommandReceived?.Invoke(this, new CommandReceivedEventArgs(player, data.Command));
            SteamNetworkingMessage_t.Release(messages[i]);
        }
    }

}
