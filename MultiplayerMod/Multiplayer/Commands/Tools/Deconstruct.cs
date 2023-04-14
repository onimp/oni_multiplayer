using System;
using MultiplayerMod.Game.Events.Tools;

namespace MultiplayerMod.Multiplayer.Commands.Tools;

[Serializable]
public class Deconstruct : AbstractDragToolCommand<DeconstructTool> {
    public Deconstruct(DragCompleteEventArgs @event) : base(@event) { }
}
