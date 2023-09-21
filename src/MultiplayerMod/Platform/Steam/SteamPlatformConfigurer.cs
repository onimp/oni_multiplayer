using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Core.Unity;
using MultiplayerMod.ModRuntime.Loader;
using MultiplayerMod.Platform.Steam.Network.Components;

namespace MultiplayerMod.Platform.Steam;

[UsedImplicitly]
[ModComponentOrder(ModComponentOrder.Platform)]
public class SteamPlatformConfigurer : IModComponentConfigurer {

    private readonly Core.Logging.Logger log = LoggerFactory.GetLogger<SteamPlatformConfigurer>();

    public void Configure(DependencyContainerBuilder builder) {
        var steam = DistributionPlatform.Inst.Platform == "Steam";
        if (!steam)
            return;

        log.Info("Steam platform detected");

        UnityObject.CreateStaticWithComponent<LobbyJoinRequestComponent>();
    }

}
