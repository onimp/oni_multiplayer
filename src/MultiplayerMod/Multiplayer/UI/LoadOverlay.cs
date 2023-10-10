using MultiplayerMod.Core.Events;
using MultiplayerMod.ModRuntime.StaticCompatibility;
using MultiplayerMod.Multiplayer.Players.Events;

namespace MultiplayerMod.Multiplayer.UI;

public static class LoadOverlay {

    private static bool active;

    public static void Show(string text) {
        if (active)
            return;

        LoadingOverlay.Load(() => { });
        LoadingOverlay.instance.GetComponentInChildren<LocText>().text = text;

        var eventDispatcher = Dependencies.Get<EventDispatcher>();
        eventDispatcher.Subscribe<PlayersReadyEvent>(
            (_, subscription) => {
                LoadingOverlay.Clear();
                subscription.Cancel();
                active = false;
            }
        );

        active = true;
    }

}
