using System;
using MultiplayerMod.Game.UI.Tools.Events;

namespace MultiplayerMod.Multiplayer.Commands.Tools;

[Serializable]
public class Dig : AbstractDragToolCommand<DigTool> {
    public Dig(DragCompleteEventArgs arguments) : base(arguments) { }
}
