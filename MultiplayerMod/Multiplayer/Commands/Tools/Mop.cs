using System;
using MultiplayerMod.Game.Tools.Events;

namespace MultiplayerMod.Multiplayer.Commands.Tools;

[Serializable]
public class Mop : AbstractDragToolCommand<MopTool> {
    public Mop(DragCompleteEventArgs arguments) : base(arguments) { }

    protected override void InitializeTool(MopTool tool) {
        base.InitializeTool(tool);
        tool.Placer = Assets.GetPrefab(new Tag("MopPlacer"));
    }
}
