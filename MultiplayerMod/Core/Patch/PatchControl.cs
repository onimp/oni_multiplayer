using System.Threading;

namespace MultiplayerMod.Core.Patch;

public static class PatchControl {

    private static readonly ThreadLocal<bool> disabled = new(() => false);

    public static bool Disabled {
        private get => disabled.Value;
        set => disabled.Value = value;
    }

    public static void RunWithDisabledPatches(System.Action action) {
        Disabled = true;
        try {
            action();
        } finally {
            Disabled = false;
        }
    }

    public static void RunIfEnabled(System.Action action) {
        if (!Disabled)
            action();
    }

}
