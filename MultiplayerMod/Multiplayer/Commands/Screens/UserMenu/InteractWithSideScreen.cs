using System;
using MultiplayerMod.Game.UI.Screens.Events;

namespace MultiplayerMod.Multiplayer.Commands.Screens.UserMenu;

[Serializable]
public class InteractWithSideScreen : AbstractInteractWithSideScreen {

    public InteractWithSideScreen(SideScreenEvents.SideScreenFieldArgs sideScreenFieldArgs) :
        base(sideScreenFieldArgs) { }

    protected override void HandleValue(object fieldValue) {
        switch (fieldValue) {
            case KToggle toggle:
                toggle.Click();
                break;
            case KButton button:
                button.SignalClick(KKeyCode.Mouse2);
                break;

            // TODO handle other cases.
        }
    }
}
