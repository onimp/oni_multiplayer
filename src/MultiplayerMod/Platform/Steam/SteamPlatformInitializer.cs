using JetBrains.Annotations;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Core.Unity;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.ModRuntime.Loader;
using MultiplayerMod.Platform.Steam.Network.Components;

namespace MultiplayerMod.Platform.Steam;

[UsedImplicitly]
[ModComponentOrder(ModComponentOrder.Platform)]
public class SteamPlatformInitializer : IModComponentInitializer {

    private readonly Core.Logging.Logger log = LoggerFactory.GetLogger<SteamPlatformInitializer>();

    public void Initialize(Runtime runtime) {
        var steam = DistributionPlatform.Inst.Platform == "Steam";
        if (!steam)
            return;

        log.Info("Steam platform detected");

        UnityObject.CreateStaticWithComponent<LobbyJoinRequestComponent>();
    }

}
