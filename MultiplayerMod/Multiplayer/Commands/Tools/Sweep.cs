using System;
using MultiplayerMod.Game.Events.Tools;

namespace MultiplayerMod.Multiplayer.Commands.Tools;

[Serializable]
public class Sweep : AbstractDragToolCommand<ClearTool> {
    public Sweep(DragCompleteEventArgs @event) : base(@event) { }
}
