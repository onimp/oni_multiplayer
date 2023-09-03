using System;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer;

public static class MultiplayerEvents {

    public static Action<IMultiplayerClientId>? PlayerWorldSpawned;

    [Serializable]
    public class PlayerWorldSpawnedEvent : MultiplayerCommand {

        private IMultiplayerClientId player;

        public PlayerWorldSpawnedEvent(IMultiplayerClientId player) {
            this.player = player;
        }

        public override void Execute(Runtime runtime) {
            PlayerWorldSpawned?.Invoke(player);
        }

    }

}
