using System.Threading.Tasks;
using MultiplayerMod.Core.Scheduling;
using MultiplayerMod.ModRuntime.StaticCompatibility;
using MultiplayerMod.Multiplayer.State;

namespace MultiplayerMod.Multiplayer.UI;

public static class LoadOverlay {
    public static void Show() {
        LoadingOverlay.Load(() => { });
        var multiplayer = Dependencies.Get<MultiplayerGame>();
        new TaskFactory(Dependencies.Get<UnityTaskScheduler>()).StartNew(
            async () => {
                // TODO block controls
                while (!multiplayer.Players.Ready) {
                    await Task.Delay(100);
                }
                LoadingOverlay.Clear();
            }
        );
    }
}
