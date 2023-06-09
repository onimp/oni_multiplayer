using System;
using MultiplayerMod.Game.UI.Tools.Events;

namespace MultiplayerMod.Multiplayer.Commands.Tools;

[Serializable]
public class Sweep : AbstractDragToolCommand<ClearTool> {
    public Sweep(DragCompleteEventArgs arguments) : base(arguments) { }
}
