using System;

namespace MultiplayerMod.Multiplayer.Commands;

[Serializable]
public class ShowOverlay : IMultiplayerCommand {
    public void Execute() => LoadingOverlay.Load(() => { });
}
