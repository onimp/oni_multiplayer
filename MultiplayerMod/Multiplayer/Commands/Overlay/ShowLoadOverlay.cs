using System;
using System.Linq;
using System.Threading.Tasks;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Scheduling;
using MultiplayerMod.Multiplayer.State;

namespace MultiplayerMod.Multiplayer.Commands.Overlay;

[Serializable]
public class ShowLoadOverlay : IMultiplayerCommand {
    public void Execute() => Show();

    public static void Show() {
        new TaskFactory(Container.Get<UnityTaskScheduler>()).StartNew(
            async () => {
                // TODO block controls
                LoadingOverlay.Load(() => { });
                while (MultiplayerGame.State.Players.Count == 0 ||
                       MultiplayerGame.State.Players.Values.Any(state => !state.Spawned)) {
                    await Task.Delay(100);
                }
                LoadingOverlay.Clear();
            }
        );
    }
}
