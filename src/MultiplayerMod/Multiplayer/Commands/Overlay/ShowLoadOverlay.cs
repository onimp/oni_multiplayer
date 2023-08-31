using System;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.Multiplayer.UI;

namespace MultiplayerMod.Multiplayer.Commands.Overlay;

[Serializable]
public class ShowLoadOverlay : MultiplayerCommand {
    public override void Execute(Runtime runtime) => LoadOverlay.Show();
}
