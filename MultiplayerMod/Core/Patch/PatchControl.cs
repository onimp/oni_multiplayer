using MultiplayerMod.Core.Patch.Context;

namespace MultiplayerMod.Core.Patch;

public static class PatchControl {

    public static readonly PatchContext DisablePatches = new(patchesEnabled: false);
    public static readonly PatchContext EnablePatches = new(patchesEnabled: true);

    public static void RunWithDisabledPatches(System.Action action) => PatchContext.Use(DisablePatches, action);

    public static void RunIfEnabled(System.Action action) {
        if (PatchContext.Global.PatchesEnabled && PatchContext.Current.PatchesEnabled)
            action();
    }

}
