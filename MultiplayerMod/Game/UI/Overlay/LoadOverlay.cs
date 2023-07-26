using System.Linq;
using System.Threading.Tasks;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Scheduling;
using MultiplayerMod.Multiplayer.State;

namespace MultiplayerMod.Game.UI.Overlay;

public static class LoadOverlay {
    public static void Show() {
        LoadingOverlay.Load(() => { });
        new TaskFactory(Container.Get<UnityTaskScheduler>()).StartNew(
            async () => {
                // TODO block controls
                while (MultiplayerGame.State.Players.Count == 0 ||
                       MultiplayerGame.State.Players.Values.Any(state => !state.WorldSpawned)) {
                    await Task.Delay(100);
                }
                LoadingOverlay.Clear();
            }
        );
    }
}
