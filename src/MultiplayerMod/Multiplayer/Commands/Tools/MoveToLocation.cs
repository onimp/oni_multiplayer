using System;
using MultiplayerMod.Multiplayer.Objects.Extensions;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Multiplayer.Commands.Tools;

[Serializable]
public class MoveToLocation(Navigator? navigator, Movable? movable, int cell) : MultiplayerCommand {

    private readonly ComponentReference<Navigator>? navigatorReference = navigator?.GetReference();
    private readonly ComponentReference<Movable>? movableReference = movable?.GetReference();

    public override void Execute(MultiplayerCommandContext context) {
        var navigator = navigatorReference?.Resolve();
        var movable = movableReference?.Resolve();

        if (navigator != null)
            navigator.GetSMI<MoveToLocationMonitor.Instance>()?.MoveToLocation(cell);
        else if (movable != null)
            movable.MoveToLocation(cell);
    }

}
