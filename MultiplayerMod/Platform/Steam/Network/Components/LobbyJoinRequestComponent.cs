using System;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Core.Unity;
using MultiplayerMod.Game.UI;
using Steamworks;

// ReSharper disable FieldCanBeMadeReadOnly.Local

namespace MultiplayerMod.Platform.Steam.Network.Components;

public class LobbyJoinRequestComponent : MultiplayerMonoBehaviour {

    private readonly Core.Logging.Logger log = LoggerFactory.GetLogger<LobbyJoinRequestComponent>();

    [Dependency]
    private SteamClient client = null!;

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
        log.Info($"Connecting to lobby {id} (command line)");
        client.Connect(new SteamServerEndpoint(id));
    }

    private void OnLobbyJoinRequested(GameLobbyJoinRequested_t request) {
        log.Info($"Connecting to lobby {request.m_steamIDLobby} (lobby join request)");
        client.Connect(new SteamServerEndpoint(request.m_steamIDLobby));
    }

    private void OnDestroy() {
        lobbyJoinRequestedCallback.Unregister();
    }

}
