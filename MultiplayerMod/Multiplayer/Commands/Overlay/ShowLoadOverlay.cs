using System;
using MultiplayerMod.Game.UI.Overlay;

namespace MultiplayerMod.Multiplayer.Commands.Overlay;

[Serializable]
public class ShowLoadOverlay : IMultiplayerCommand {
    public void Execute() => LoadOverlay.Show();
}
