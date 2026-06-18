using System;
using MultiplayerMod.Game.World;
using MultiplayerMod.Multiplayer.Objects.Extensions;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Multiplayer.Commands.Tools;

[Serializable]
public class MoveToLocation(Navigator? navigator, Movable? movable, int cell) : MultiplayerCommand {

    private readonly ComponentReference<Navigator>? navigatorReference = navigator?.GetReference();
    private readonly ComponentReference<Movable>? movableReference = movable?.GetReference();
    private readonly int? worldId = WorldIdentity.GetCellWorldId(cell);

    public override void Execute(MultiplayerCommandContext context) {
        WorldCommandScope.Execute(
            worldId,
            nameof(MoveToLocation),
            () => {
                var navigator = navigatorReference?.Resolve();
                var movable = movableReference?.Resolve();

                if (navigator != null)
                    navigator.GetSMI<MoveToLocationMonitor.Instance>()?.MoveToLocation(cell);
                else if (movable != null)
                    movable.MoveToLocation(cell);
            }
        );
    }

}
