using System;
using MultiplayerMod.Game.Events.Tools;

namespace MultiplayerMod.Multiplayer.Commands.Tools;

[Serializable]
public class Dig : AbstractDragToolCommand<DigTool> {
    public Dig(DragCompleteEventArgs @event) : base(@event) { }
}
