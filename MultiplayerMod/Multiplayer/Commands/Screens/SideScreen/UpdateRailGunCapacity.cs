using System;
using MultiplayerMod.Game.UI.SideScreens;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Multiplayer.Commands.Screens.SideScreen;

[Serializable]
public class UpdateRailGunCapacity : IMultiplayerCommand {
    private readonly ComponentReference target;
    private readonly RailGunSideScreenEvents.RailGunSideScreenEventArgs eventArgs;

    public UpdateRailGunCapacity(
        ComponentReference target,
        RailGunSideScreenEvents.RailGunSideScreenEventArgs eventArgs
    ) {
        this.target = target;
        this.eventArgs = eventArgs;
    }

    public void Execute() {
        var railGun = (RailGun) target.GetComponent();
        railGun.launchMass = eventArgs.LaunchMass;
    }
}
