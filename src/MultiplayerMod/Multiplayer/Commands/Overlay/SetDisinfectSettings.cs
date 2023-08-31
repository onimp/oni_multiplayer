using System;
using MultiplayerMod.ModRuntime;

namespace MultiplayerMod.Multiplayer.Commands.Overlay;

[Serializable]
public class SetDisinfectSettings : MultiplayerCommand {
    private readonly int minGerm;
    private readonly bool enableAutoDisinfect;

    public SetDisinfectSettings(int minGerm, bool enableAutoDisinfect) {
        this.minGerm = minGerm;
        this.enableAutoDisinfect = enableAutoDisinfect;
    }

    public override void Execute(Runtime runtime) {
        SaveGame.Instance.enableAutoDisinfect = enableAutoDisinfect;
        SaveGame.Instance.minGermCountForDisinfect = minGerm;
    }
}
