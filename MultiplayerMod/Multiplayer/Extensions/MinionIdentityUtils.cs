using System.Linq;
using MultiplayerMod.Core.Logging;

namespace MultiplayerMod.Multiplayer.Extensions;

public class MinionIdentityUtils {

    private static readonly Core.Logging.Logger log = LoggerFactory.GetLogger<MinionIdentityUtils>();

    public static MinionIdentity GetLiveMinion(string properName) {
        var minionIdentity =
            global::Components.LiveMinionIdentities.Items.FirstOrDefault(
                minion => minion.GetProperName() == properName
            );
        if (minionIdentity != null) return minionIdentity;

        log.Warning($"Minion {properName} is not found.");
        return null;
    }
}
