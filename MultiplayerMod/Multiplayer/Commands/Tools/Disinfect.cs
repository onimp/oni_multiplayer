using System;
using MultiplayerMod.Game.Events.Tools;

namespace MultiplayerMod.Multiplayer.Commands.Tools;

[Serializable]
public class Disinfect : AbstractDragToolCommand<DisinfectTool> {
    public Disinfect(DragCompleteEventArgs @event) : base(@event) { }
}
