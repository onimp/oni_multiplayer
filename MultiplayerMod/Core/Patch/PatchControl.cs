using System.Threading;

namespace MultiplayerMod.Core.Patch;

public static class PatchControl {

    private static readonly ThreadLocal<bool> disabled = new(() => false);

    public static void RunWithDisabledPatches(System.Action action) {
        disabled.Value = true;
        try {
            action();
        } finally {
            disabled.Value = false;
        }
    }

    public static void RunIfEnabled(System.Action action) {
        if (!disabled.Value)
            action();
    }

}
