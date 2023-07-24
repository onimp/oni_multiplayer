using System;

namespace MultiplayerMod.Multiplayer;

public static class MultiplayerEvents {

    public static Action<IPlayer>? PlayerWorldSpawned;

    [Serializable]
    public class PlayerWorldSpawnedEvent : IMultiplayerCommand {

        private IPlayer player;

        public PlayerWorldSpawnedEvent(IPlayer player) {
            this.player = player;
        }

        public void Execute() {
            PlayerWorldSpawned?.Invoke(player);
        }

    }
}
