using System;
using MultiplayerMod.Game.Tools.Events;

namespace MultiplayerMod.Multiplayer.Commands.Tools;

[Serializable]
public class Dig : AbstractDragToolCommand<DigTool> {
    public Dig(DragCompleteEventArgs arguments) : base(arguments) { }
}
