using System;
using MultiplayerMod.Game.Mechanics;
using MultiplayerMod.Multiplayer.Objects;

namespace MultiplayerMod.Multiplayer.Commands.Gameplay.SliderControl;

[Serializable]
public class SetSliderValue : IMultiplayerCommand {
    private readonly MultiplayerReference reference;
    private readonly float value;
    private readonly int index;

    public SetSliderValue(SliderEvents.SliderEventArgs sliderEventArgs) {
        reference = sliderEventArgs.Target;
        value = sliderEventArgs.Value;
        index = sliderEventArgs.Index;
    }

    public void Execute() {
        var sliderControl = reference.GetComponent<ISliderControl>();
        sliderControl?.SetSliderValue(value, index);
    }
}
