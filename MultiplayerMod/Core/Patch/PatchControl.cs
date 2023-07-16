using System;
using System.Threading;

namespace MultiplayerMod.Core.Patch;

public static class PatchControl {

    private static readonly ThreadLocal<bool> disabled = new(() => false);

    public static void RunWithDisabledPatches(System.Action action) {
        RunWithDisabledPatches<object>(
            () => {
                action();
                return null;
            }
        );
    }

    public static T? RunWithDisabledPatches<T>(Func<T?> action) {
        disabled.Value = true;
        try {
            return action();
        } finally {
            disabled.Value = false;
        }
    }

    public static bool PatchesDisabled() => disabled.Value;

    public static void RunIfEnabled(System.Action action) {
        if (!disabled.Value)
            action();
    }

}
