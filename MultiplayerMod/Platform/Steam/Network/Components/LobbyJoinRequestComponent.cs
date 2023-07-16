﻿using System;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Network;
using Steamworks;
using UnityEngine;

namespace MultiplayerMod.Platform.Steam.Network.Components;

public class LobbyJoinRequestComponent : MonoBehaviour {

    private readonly Core.Logging.Logger log = LoggerFactory.GetLogger<LobbyJoinRequestComponent>();

    private SteamClient client = null!;
    private Callback<GameLobbyJoinRequested_t> lobbyJoinRequestedCallback = null!;

    private void Awake() {
        client = (SteamClient) Container.Get<IMultiplayerClient>();
        lobbyJoinRequestedCallback = Callback<GameLobbyJoinRequested_t>.Create(OnLobbyJoinRequested);
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
