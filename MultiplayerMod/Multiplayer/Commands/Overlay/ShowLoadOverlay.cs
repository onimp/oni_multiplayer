using System;
using MultiplayerMod.Game.UI.Overlay;

namespace MultiplayerMod.Multiplayer.Commands.Overlay;

[Serializable]
public class ShowLoadOverlay : MultiplayerCommand {
    public override void Execute() => LoadOverlay.Show();
}
