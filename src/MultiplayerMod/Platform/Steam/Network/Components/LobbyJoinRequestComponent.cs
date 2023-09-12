using System;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Core.Unity;
using MultiplayerMod.Game.UI;
using MultiplayerMod.Multiplayer.Players.Events;
using Steamworks;

// ReSharper disable FieldCanBeMadeReadOnly.Local

namespace MultiplayerMod.Platform.Steam.Network.Components;

public class LobbyJoinRequestComponent : MultiplayerMonoBehaviour {

    private readonly Core.Logging.Logger log = LoggerFactory.GetLogger<LobbyJoinRequestComponent>();

    [Dependency]
    private EventDispatcher eventDispatcher = null!;

    private Callback<GameLobbyJoinRequested_t> lobbyJoinRequestedCallback = null!;

    protected override void Awake() {
        base.Awake();
        lobbyJoinRequestedCallback = Callback<GameLobbyJoinRequested_t>.Create(OnLobbyJoinRequested);
        MainMenuEvents.Initialized += OnMainMenuInitialized;
    }

    private void OnMainMenuInitialized() {
        MainMenuEvents.Initialized -= OnMainMenuInitialized;
        ProcessCommandLine();
    }

    private void ProcessCommandLine() {
        var arguments = Environment.GetCommandLineArgs();
        if (arguments.Length <= 2 || arguments[1] != "+connect_lobby")
            return;

        var id = new CSteamID(ulong.Parse(arguments[2]));
        log.Info($"Requesting connection to lobby {id} (command line)");
        DispatchJoinRequest(id);
    }

    private void OnLobbyJoinRequested(GameLobbyJoinRequested_t request) {
        log.Info($"Requesting connection to lobby {request.m_steamIDLobby} (lobby join request)");
        DispatchJoinRequest(request.m_steamIDLobby);
    }

    private void DispatchJoinRequest(CSteamID lobbyId) {
        var endpoint = new SteamServerEndpoint(lobbyId);
        var dataUpdateCallback = new Callback<LobbyDataUpdate_t>[1];
        dataUpdateCallback[0] = Callback<LobbyDataUpdate_t>.Create(_ => {
            var serverName = SteamMatchmaking.GetLobbyData(lobbyId, "server.name");
            dataUpdateCallback[0].Unregister();
            eventDispatcher.Dispatch(new MultiplayerJoinRequestedEvent(endpoint, serverName));
        });
        SteamMatchmaking.RequestLobbyData(lobbyId);
    }

    private void OnDestroy() {
        lobbyJoinRequestedCallback.Unregister();
    }

}
