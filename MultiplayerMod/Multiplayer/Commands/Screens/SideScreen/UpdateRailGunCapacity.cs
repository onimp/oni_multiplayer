using System;
using static MultiplayerMod.Game.UI.SideScreens.RailGunSideScreenEvents;

namespace MultiplayerMod.Multiplayer.Commands.Screens.SideScreen;

[Serializable]
public class UpdateRailGunCapacity : IMultiplayerCommand {

    private readonly RailGunSideScreenEventArgs args;

    public UpdateRailGunCapacity(RailGunSideScreenEventArgs args) {
        this.args = args;
    }

    public void Execute() {
        var railGun = args.Target.GetComponent();
        railGun.launchMass = args.LaunchMass;
    }

}
