using System;
using MultiplayerMod.Game.Tools.Events;

namespace MultiplayerMod.Multiplayer.Commands.Tools;

[Serializable]
public class Deconstruct : AbstractDragToolCommand<DeconstructTool> {
    public Deconstruct(DragCompleteEventArgs arguments) : base(arguments) { }
}
