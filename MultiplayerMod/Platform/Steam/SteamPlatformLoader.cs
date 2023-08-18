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

// ReSharper disable once UnusedType.Global
[ModComponentOrder(ModComponentOrder.Platform)]
public class SteamPlatformLoader : IModComponentLoader {

    private readonly Core.Logging.Logger log = LoggerFactory.GetLogger<SteamPlatformLoader>();

    public void OnLoad(Harmony harmony) {
        var steam = DistributionPlatform.Inst.Platform == "Steam";
        if (!steam)
            return;

        log.Info("Steam platform detected");
        Dependencies.Register<IMultiplayerOperations, SteamMultiplayerOperations>();
        Dependencies.Register<IMultiplayerServer, SteamServer>();
        Dependencies.Register<IMultiplayerClient, SteamClient>();
        Dependencies.Register<SteamLobby>();
        UnityObject.CreateStaticWithComponent<LobbyJoinRequestComponent>();
    }

}
