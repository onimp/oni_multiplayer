using System;

namespace MultiplayerMod.Multiplayer.Commands.Overlay;

[Serializable]
public class SetDisinfectSettings : IMultiplayerCommand {
    private readonly int minGerm;
    private readonly bool enableAutoDisinfect;

    public SetDisinfectSettings(int minGerm, bool enableAutoDisinfect) {
        this.minGerm = minGerm;
        this.enableAutoDisinfect = enableAutoDisinfect;
    }

    public void Execute() {
        SaveGame.Instance.enableAutoDisinfect = enableAutoDisinfect;
        SaveGame.Instance.minGermCountForDisinfect = minGerm;
    }
}
