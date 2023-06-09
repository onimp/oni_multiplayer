using System;
using MultiplayerMod.Game.UI.Tools.Events;

namespace MultiplayerMod.Multiplayer.Commands.Tools;

[Serializable]
public class Prioritize : AbstractDragToolCommand<PrioritizeTool> {
    public Prioritize(DragCompleteEventArgs arguments) : base(arguments) { }
}
