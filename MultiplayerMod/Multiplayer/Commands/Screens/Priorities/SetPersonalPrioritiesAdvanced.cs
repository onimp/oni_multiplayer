using System;

namespace MultiplayerMod.Multiplayer.Commands.Screens.Priorities;

[Serializable]
public class SetPersonalPrioritiesAdvanced : IMultiplayerCommand {
    private readonly bool value;

    public SetPersonalPrioritiesAdvanced(bool value) {
        this.value = value;
    }

    public void Execute() {
        global::Game.Instance.advancedPersonalPriorities = value;
        ManagementMenu.Instance.jobsScreen.toggleAdvancedModeButton.fgImage.gameObject.SetActive(value);
    }
}
