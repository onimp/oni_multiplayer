using System;
using MultiplayerMod.Game.UI.Tools.Events;

namespace MultiplayerMod.Multiplayer.Commands.Tools;

[Serializable]
public class Disinfect : AbstractDragToolCommand<DisinfectTool> {
    public Disinfect(DragCompleteEventArgs arguments) : base(arguments) { }
}
