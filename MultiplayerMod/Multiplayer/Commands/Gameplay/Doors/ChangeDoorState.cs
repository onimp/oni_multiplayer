using System;
using MultiplayerMod.Game.Mechanics;
using MultiplayerMod.Multiplayer.Tools;

namespace MultiplayerMod.Multiplayer.Commands.Gameplay.Doors;

[Serializable]
public class ChangeDoorState : IMultiplayerCommand {

    private readonly DoorStateChangedEventArgs arguments;

    public ChangeDoorState(DoorStateChangedEventArgs arguments) {
        this.arguments = arguments;
    }

    public void Execute() {
        var door = arguments.Target.GetComponent<Door>();
        door.QueueStateChange(arguments.State);

        if (SideScreen<DoorToggleSideScreen>.TargetSelected(door, out var screen))
            screen.Refresh();
    }

}
