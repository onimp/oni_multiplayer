using System;
using MultiplayerMod.Game.Context;
using MultiplayerMod.Game.Effects;
using MultiplayerMod.Game.UI.Tools.Events;

namespace MultiplayerMod.Multiplayer.Commands.Tools;

[Serializable]
public class Prioritize : AbstractDragToolCommand<PrioritizeTool> {

    public Prioritize(DragCompleteEventArgs arguments) : base(arguments) { }

    protected override IGameContext CreateContext() => new GameContextComposite(
        base.CreateContext(),
        new DisablePriorityConfirmSound()
    );

}
