﻿extern alias ValveSockets;
using HarmonyLib;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Loader;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Core.Unity;
using MultiplayerMod.Multiplayer;
using MultiplayerMod.Network;
using MultiplayerMod.Platform.Steam.Network;
using MultiplayerMod.Platform.Steam.Network.Components;

namespace MultiplayerMod.Platform.Steam;

public class SteamPlatformLoader : IModComponentLoader {

    private readonly Core.Logging.Logger log = LoggerFactory.GetLogger<SteamPlatformLoader>();

    public void OnLoad(Harmony harmony) {
        var steam = DistributionPlatform.Inst.Platform == "Steam";
        if (!steam)
            return;

        log.Info("Steam platform detected");
        Container.Register<IMultiplayerOperations, SteamMultiplayerOperations>();
#if USE_DEV_NET
        Library.Initialize();
        Container.Register<IMultiplayerServer, GNSServer>();
        Container.Register<IMultiplayerClient, GNSClient>();
#else
        Container.Register<IMultiplayerServer, SteamServer>();
        Container.Register<IMultiplayerClient, SteamClient>();
        Container.Register<SteamLobby>();
        UnityObject.CreateStaticWithComponent<LobbyJoinRequestComponent>();
#endif
    }

}
