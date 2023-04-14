using System;
using MultiplayerMod.Game.Events.Tools;

namespace MultiplayerMod.Multiplayer.Commands.Tools;

[Serializable]
public class Prioritize : AbstractDragToolCommand<PrioritizeTool> {
    public Prioritize(DragCompleteEventArgs @event) : base(@event) { }
}
