using System;

namespace MultiplayerMod.Multiplayer;

public static class MultiplayerEvents {

    public static Action<IPlayerIdentity>? PlayerWorldSpawned;

    [Serializable]
    public class PlayerWorldSpawnedEvent : MultiplayerCommand {

        private IPlayerIdentity player;

        public PlayerWorldSpawnedEvent(IPlayerIdentity player) {
            this.player = player;
        }

        public override void Execute() {
            PlayerWorldSpawned?.Invoke(player);
        }

    }

}
