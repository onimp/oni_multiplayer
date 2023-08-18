using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Patch.Context;
using MultiplayerMod.Multiplayer.State;

namespace MultiplayerMod.Core.Patch;

public static class PatchControl {

    public static readonly PatchContext DisablePatches = new(patchesEnabled: false);
    public static readonly PatchContext EnablePatches = new(patchesEnabled: true);

    public static void RunWithDisabledPatches(System.Action action) => PatchContext.Use(DisablePatches, action);

    public static void RunIfEnabled(System.Action action) {
        if (PatchContext.Global.PatchesEnabled && PatchContext.Current.PatchesEnabled)
            action();
    }

    public static void RunIfSpawned(System.Action action) {
        var multiplayerState = Dependencies.Get<MultiplayerGame>().State;
        if (multiplayerState.Players.Count > 0 && multiplayerState.Current.WorldSpawned)
            action();
    }

}
