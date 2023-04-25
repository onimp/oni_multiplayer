using System.Diagnostics.CodeAnalysis;

namespace MultiplayerMod.Game.Context;

public static class GameContext {

    [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
    public static void Override(IGameContext context, System.Action action) {
        try {
            context.Apply();
            action();
        } finally {
            context.Restore();
        }
    }

}
