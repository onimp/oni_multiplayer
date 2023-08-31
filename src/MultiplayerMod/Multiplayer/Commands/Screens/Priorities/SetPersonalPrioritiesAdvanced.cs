using System;
using MultiplayerMod.ModRuntime;

namespace MultiplayerMod.Multiplayer.Commands.Screens.Priorities;

[Serializable]
public class SetPersonalPrioritiesAdvanced : MultiplayerCommand {
    private readonly bool value;

    public SetPersonalPrioritiesAdvanced(bool value) {
        this.value = value;
    }

    public override void Execute(Runtime runtime) {
        global::Game.Instance.advancedPersonalPriorities = value;
        ManagementMenu.Instance.jobsScreen.toggleAdvancedModeButton.fgImage.gameObject.SetActive(value);
    }
}
