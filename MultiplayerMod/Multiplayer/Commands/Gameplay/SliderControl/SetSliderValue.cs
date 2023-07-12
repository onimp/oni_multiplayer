using System;
using MultiplayerMod.Multiplayer.Objects;

namespace MultiplayerMod.Multiplayer.Commands.Gameplay.SliderControl;

[Serializable]
public class SetSliderValue : IMultiplayerCommand {
    private readonly MultiplayerReference reference;
    private readonly float percent;
    private readonly int index;

    public SetSliderValue(MultiplayerReference reference, float percent, int index) {
        this.reference = reference;
        this.percent = percent;
        this.index = index;
    }

    public void Execute() {
        var sliderControl = reference.GetComponent<ISliderControl>();
        sliderControl?.SetSliderValue(percent, index);
    }
}
