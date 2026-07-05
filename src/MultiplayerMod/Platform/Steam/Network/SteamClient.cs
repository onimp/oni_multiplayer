using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Extensions;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Core.Unity;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Multiplayer.Commands.Registry;
using MultiplayerMod.Network;
using MultiplayerMod.Platform.Steam.Network.Components;
using MultiplayerMod.Platform.Steam.Network.Messaging;
using Steamworks;
using UnityEngine;
using static Steamworks.Constants;
using static Steamworks.ESteamNetConnectionEnd;

namespace MultiplayerMod.Platform.Steam.Network;

[Dependency, UsedImplicitly]
public class SteamClient : IMultiplayerClient {

    public IMultiplayerClientId Id => playerContainer.Value;
    public MultiplayerClientState State { get; private set; } = MultiplayerClientState.Disconnected;
    public event Action<MultiplayerClientState>? StateChanged;
    public event Action<IMultiplayerCommand>? CommandReceived;

    private readonly Core.Logging.Logger log = LoggerFactory.GetLogger<SteamClient>();
    private readonly SteamLobby lobby;
    private readonly MultiplayerCommandRegistry commands;

    private readonly Lazy<IMultiplayerClientId> playerContainer =
        new(() => new SteamMultiplayerClientId(SteamUser.GetSteamID()));

    private readonly NetworkMessageProcessor messageProcessor = new();
    private readonly NetworkMessageFactory messageFactory = new();
    private SteamNetworkingMessageSender sender = null!;
    private bool lanesConfigured;

    private HSteamNetConnection connection = HSteamNetConnection.Invalid;
    private readonly SteamNetworkingConfigValue_t[] networkConfig =
        { Configuration.SendBufferSize(), Configuration.SendRateMax() };

    private GameObject gameObject = null!;

    public SteamClient(SteamLobby lobby, MultiplayerCommandRegistry commands) {
        this.lobby = lobby;
        this.commands = commands;
    }

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
        if (State == MultiplayerClientState.Disconnected)
            throw new NetworkPlatformException("Client not connected");

        UnityObject.Destroy(gameObject);
        lobby.Leave();
        lobby.OnJoin -= OnLobbyJoin;
        SteamNetworkingSockets.CloseConnection(connection, (int) k_ESteamNetConnectionEnd_App_Generic, "", true);
        SetState(MultiplayerClientState.Disconnected);
        SteamFriends.ClearRichPresence();
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

        var lane = commands.GetCommandConfiguration(command.GetType()).Lane;
        var flags = Configuration.SendFlags(lane);
        messageFactory.Create(command, options).ForEach(handle => Send(handle, flags, lane));
    }

    public void Flush() {
        if (State != MultiplayerClientState.Connected)
            return;

        // Push everything Steam has buffered for this connection (across all lanes) onto the wire now.
        var result = SteamNetworkingSockets.FlushMessagesOnConnection(connection);
        if (result != EResult.k_EResultOK)
            log.Warning($"FlushMessagesOnConnection returned {result}");
    }

    public ConnectionStats? GetConnectionStats() {
        if (State != MultiplayerClientState.Connected)
            return null;

        var status = new SteamNetConnectionRealTimeStatus_t();
        var lanes = new SteamNetConnectionRealTimeLaneStatus_t(); // unused: nLanes = 0
        var result = SteamNetworkingSockets.GetConnectionRealTimeStatus(connection, ref status, 0, ref lanes);
        if (result != EResult.k_EResultOK)
            return null;

        return new ConnectionStats(
            status.m_nPing,
            status.m_flOutBytesPerSec,
            status.m_flInBytesPerSec,
            status.m_cbPendingReliable,
            status.m_flConnectionQualityLocal
        );
    }

    private void Send(INetworkMessageHandle handle, int flags, NetworkLane lane) {
        // Lane-aware send when lanes are configured; otherwise (or if the native lane send fails) fall
        // back to the flat API - still honours the reliability flag, only cross-lane prioritisation is
        // lost. sender.Send only returns false when nothing was queued, so this cannot duplicate a message.
        if (sender.Available && lanesConfigured && sender.Send(handle, connection, flags, lane))
            return;

        var result = SteamNetworkingSockets.SendMessageToConnection(
            connection,
            handle.Pointer,
            handle.Size,
            flags,
            out var messageOut
        );
        if (result != EResult.k_EResultOK || messageOut == 0) {
            log.Error($"Failed to send message: {result}");
            SetState(MultiplayerClientState.Error);
        }
    }

    private void SetState(MultiplayerClientState status) {
        State = status;
        StateChanged?.Invoke(status);
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

        sender = SteamNetworkingMessageSender.ForClient();
        lanesConfigured = Configuration.ConfigureClientLanes(connection);

        log.Debug($"P2P Connect to {serverId}");

        SetRichPresence();

        gameObject = UnityObject.CreateStaticWithComponent<SteamClientComponent>();
        SetState(MultiplayerClientState.Connected);
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
                CommandReceived?.Invoke(message.Command);
            SteamNetworkingMessage_t.Release(messages[i]);
        }
    }

}
