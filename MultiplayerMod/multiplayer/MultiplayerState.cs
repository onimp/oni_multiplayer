namespace MultiplayerMod.multiplayer
{
    /// <summary>
    /// A singleton showing current multiplayer game.
    ///
    /// Created after multiple places requiring to be executed only if in multiplayer game (or in MP and being a client/host).
    /// </summary>
    public static class MultiplayerState
    {
        public static bool IsConnected => CurrentState is State.Connecting or State.Spawned;

        public static bool IsSpawned => CurrentState is State.Spawned;

        private static State CurrentState { get; set; }

        public static Role MultiplayerRole { get; private set; }

        public static void MainMenu()
        {
            CurrentState = State.NotConnected;
            MultiplayerRole = Role.None;
        }

        public static void SetRoleToHost()
        {
            MultiplayerRole = Role.Server;
        }

        public static void SetRoleToClient()
        {
            MultiplayerRole = Role.Client;
        }
        
        public static void WorldLoaded()
        {
            CurrentState = State.Spawned;
        }

        public static void ConnectToServer()
        {
            if (MultiplayerRole == Role.None) MultiplayerRole = Role.Client;
            CurrentState = State.Connecting;
        }

        private enum State
        {
            NotConnected,
            Connecting,
            Spawned
        }

        public enum Role
        {
            None,
            Server,
            Client
        }
    }
}