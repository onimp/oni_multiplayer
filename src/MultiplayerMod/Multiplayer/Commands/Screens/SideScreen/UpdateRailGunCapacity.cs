using System;
using MultiplayerMod.ModRuntime;
using static MultiplayerMod.Game.UI.SideScreens.RailGunSideScreenEvents;

namespace MultiplayerMod.Multiplayer.Commands.Screens.SideScreen;

[Serializable]
public class UpdateRailGunCapacity : MultiplayerCommand {

    private readonly RailGunSideScreenEventArgs args;

    public UpdateRailGunCapacity(RailGunSideScreenEventArgs args) {
        this.args = args;
    }

    public override void Execute(Runtime runtime) {
        var railGun = args.Target.GetComponent();
        railGun.launchMass = args.LaunchMass;
    }

}
