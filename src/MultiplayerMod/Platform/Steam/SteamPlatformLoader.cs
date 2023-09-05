using JetBrains.Annotations;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Core.Unity;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.ModRuntime.Loader;
using MultiplayerMod.Multiplayer;
using MultiplayerMod.Multiplayer.Players;
using MultiplayerMod.Network;
using MultiplayerMod.Platform.Steam.Network;
using MultiplayerMod.Platform.Steam.Network.Components;

namespace MultiplayerMod.Platform.Steam;

[UsedImplicitly]
[ModComponentOrder(ModComponentOrder.Platform)]
public class SteamPlatformLoader : IModComponentLoader {

    private readonly Core.Logging.Logger log = LoggerFactory.GetLogger<SteamPlatformLoader>();

    public void Load(Runtime runtime) {
        var steam = DistributionPlatform.Inst.Platform == "Steam";
        if (!steam)
            return;

        log.Info("Steam platform detected");

        var dependencies = runtime.Dependencies;
        dependencies.Register<IPlayerProfileProvider, SteamPlayerProfileProvider>();
        dependencies.Register<IMultiplayerOperations, SteamMultiplayerOperations>();
        dependencies.Register<IMultiplayerServer, SteamServer>();
        dependencies.Register<IMultiplayerClient, SteamClient>();
        dependencies.Register<SteamLobby>();

        UnityObject.CreateStaticWithComponent<LobbyJoinRequestComponent>();
    }

}
