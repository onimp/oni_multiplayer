using System;
using MultiplayerMod.Game.Events.Tools;

namespace MultiplayerMod.Multiplayer.Commands.Tools;

[Serializable]
public class Mop : AbstractDragToolCommand<MopTool> {
    public Mop(DragCompleteEventArgs @event) : base(@event) { }

    protected override void InitializeTool(MopTool tool) {
        base.InitializeTool(tool);
        tool.Placer = Assets.GetPrefab(new Tag("MopPlacer"));
    }
}
