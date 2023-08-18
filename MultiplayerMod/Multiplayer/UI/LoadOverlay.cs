using System.Linq;
using System.Threading.Tasks;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Scheduling;
using MultiplayerMod.Multiplayer.State;

namespace MultiplayerMod.Multiplayer.UI;

public static class LoadOverlay {
    public static void Show() {
        LoadingOverlay.Load(() => { });
        var multiplayer = Dependencies.Get<MultiplayerGame>();
        new TaskFactory(Dependencies.Get<UnityTaskScheduler>()).StartNew(
            async () => {
                // TODO block controls
                while (multiplayer.State.Players.Count == 0 ||
                       multiplayer.State.Players.Values.Any(state => !state.WorldSpawned)) {
                    await Task.Delay(100);
                }
                LoadingOverlay.Clear();
            }
        );
    }
}
