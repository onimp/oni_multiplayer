using System;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Multiplayer.Players.Events;

namespace MultiplayerMod.Multiplayer.Commands.Speed;

[Serializable]
public class ResumeGame : MultiplayerCommand {

    public override void Execute(MultiplayerCommandContext context) {
        if (TryResume(context.Multiplayer))
            return;
        context.EventDispatcher.Subscribe<PlayersReadyEvent>(OnPlayersReady);
    }

    private void OnPlayersReady(PlayersReadyEvent @event, EventSubscription subscription) {
        Resume();
        subscription.Cancel();
    }

    private bool TryResume(MultiplayerGame multiplayer) {
        if (!multiplayer.Players.Ready)
            return false;
        Resume();
        return true;
    }

    private void Resume() {
        if (SpeedControlScreen.Instance.IsPaused)
            SpeedControlScreen.Instance.Unpause();
    }

}
