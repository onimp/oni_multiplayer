using HarmonyLib;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Loader;
using MultiplayerMod.Core.Unity;
using MultiplayerMod.Network;
using MultiplayerMod.Platform.Steam.Network;
using MultiplayerMod.Platform.Steam.Network.Components;

namespace MultiplayerMod.Platform.Steam;

public class SteamPlatformLoader : IModComponentLoader {

    private readonly Core.Logging.Logger log = new(typeof(SteamPlatformLoader));

    public void OnLoad(Harmony harmony) {
        var steam = DistributionPlatform.Inst.Platform == "Steam";
        if (!steam)
            return;

        log.Info("Steam platform detected");
        Container.Register<IMultiplayerServer, SteamServer>();
        Container.Register<IMultiplayerClient, SteamClient>();
        Container.Register<SteamLobby>();
        UnityObject.CreateStaticWithComponent<LobbyJoinRequestComponent>();
    }

}
