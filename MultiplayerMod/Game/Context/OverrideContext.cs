using MultiplayerMod.Game.Debug;

namespace MultiplayerMod.Game.Context;

public class OverrideContext {
    public PrioritySetting? Priority { get; set; }
    public ModifyParameters ModifyParameters { get; set; }
}
