using System;
using System.Collections.Generic;
using MultiplayerMod.Game.UI.Screens.Events;

namespace MultiplayerMod.Multiplayer.Commands.Screens.UserMenu;

[Serializable]
public class InteractWithSliderSideScreen : AbstractInteractWithSideScreen {
    private readonly int sliderIndex;
    private readonly float sliderValue;

    public InteractWithSliderSideScreen(
        SideScreenEvents.SideScreenFieldArgs sideScreenFieldArgs,
        int sliderIndex,
        float sliderValue
    ) : base(sideScreenFieldArgs) {
        this.sliderIndex = sliderIndex;
        this.sliderValue = sliderValue;
    }

    protected override void HandleValue(object fieldValue) {
        var sliders = (List<SliderSet>) fieldValue;
        var numberInput = sliders[sliderIndex].numberInput;
        numberInput.currentValue = sliderValue;
        numberInput.StopEditing();
    }
}
