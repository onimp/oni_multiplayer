using System;

namespace MultiplayerMod.Game.Context;

public abstract class GameContextManager {

    private static readonly GameContextOverride contextOverride = new();

    public static void Override(OverrideContext context, System.Action action) {
        Override<object>(
            context,
            () => {
                action();
                return null;
            }
        );
    }

    public static T Override<T>(OverrideContext context, Func<T> action) {
        var originalContext = GameContext.GetCurrent();
        try {
            contextOverride.Restore();
            if (context.Priority != null)
                contextOverride.PriorityScreen.lastSelectedPriority = context.Priority.Value;
            return action();
        } finally {
            originalContext.Restore();
        }
    }

}
