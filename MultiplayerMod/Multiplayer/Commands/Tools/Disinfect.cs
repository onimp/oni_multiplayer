using System;
using MultiplayerMod.Game.Tools.Events;

namespace MultiplayerMod.Multiplayer.Commands.Tools;

[Serializable]
public class Disinfect : AbstractDragToolCommand<DisinfectTool> {
    public Disinfect(DragCompleteEventArgs arguments) : base(arguments) { }
}
