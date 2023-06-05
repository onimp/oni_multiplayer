using System;
using MultiplayerMod.Game.UI.Tools.Events;

namespace MultiplayerMod.Multiplayer.Commands.Tools;

[Serializable]
public class Deconstruct : AbstractDragToolCommand<DeconstructTool> {
    public Deconstruct(DragCompleteEventArgs arguments) : base(arguments) { }
}
