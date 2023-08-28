using MultiplayerMod.Game.Context;

namespace MultiplayerMod.Game.UI.Tools.Context;

public class PrioritySettingsContext : IGameContext {

    private readonly PrioritySetting priority;

    public PrioritySettingsContext(PrioritySetting priority) {
        this.priority = priority;
    }

    public void Apply() {
        GameStaticContext.Override();
        if (GameStaticContext.Current.PriorityScreen != null)
            GameStaticContext.Current.PriorityScreen.lastSelectedPriority = priority;
    }

    public void Restore() {
        GameStaticContext.Restore();
    }

}
