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

        var multiplayer = Dependencies.Get<MultiplayerGame>();
        var eventDispatcher = Dependencies.Get<EventDispatcher>();
        eventDispatcher.Subscribe<PlayerStateChangedEvent>(
            (_, subscription) => {
                if (!multiplayer.Players.Ready)
                    return;

                LoadingOverlay.Clear();
                subscription.Cancel();
                active = false;
            }
        );

        active = true;
    }

}
