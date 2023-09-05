using System;
using MultiplayerMod.Multiplayer.UI;

namespace MultiplayerMod.Multiplayer.Commands.Overlay;

[Serializable]
[MultiplayerCommand(Type = MultiplayerCommandType.System)]
public class ShowLoadOverlay : MultiplayerCommand {
    public override void Execute(MultiplayerCommandContext context) => LoadOverlay.Show();
}
