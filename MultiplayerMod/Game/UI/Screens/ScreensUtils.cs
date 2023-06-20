using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiplayerMod.Game.UI.Screens;

public static class ScreensUtils {

    private const int delayMS = 1;
    private const int maxWaitMS = 50;

    public static async Task<List<ITelepadDeliverable>> WaitForAllDeliverablesReady(ImmigrantScreen instance) {
        var currentDelay = 0;
        while (currentDelay < maxWaitMS) {
            var readyDeliverables = instance.containers?.Select(
                container => container switch {
                    CharacterContainer characterContainer => (ITelepadDeliverable) characterContainer.stats,
                    CarePackageContainer packageContainer => packageContainer.carePackageInstanceData,
                    _ => null
                }
            ).Where(deliverable => deliverable != null).ToList();
            if (readyDeliverables != null && readyDeliverables.Count == instance.containers.Count)
                return readyDeliverables;

            await Task.Delay(delayMS);
            currentDelay += delayMS;
        }
        return null;
    }
}
