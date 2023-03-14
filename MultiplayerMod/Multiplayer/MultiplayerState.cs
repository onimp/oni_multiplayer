namespace MultiplayerMod.Multiplayer
{

    /// <summary>
    ///     A singleton showing current multiplayer game.
    ///     Created after multiple places requiring to be executed only if in multiplayer game (or in MP and being a
    ///     client/host).
    /// </summary>
    public static class MultiplayerState
    {

        public enum Role
        {
            None,
            Server,
            Client
        }

        public static bool IsConnected => CurrentState is State.Connected;

        public static bool IsSpawned { get; private set; }

        private static State CurrentState { get; set; }

        public static Role MultiplayerRole { get; private set; }

        public static void MainMenu()
        {
            CurrentState = State.NotConnected;
            MultiplayerRole = Role.None;
            IsSpawned = false;
        }

        public static void SetRoleToHost()
        {
            MultiplayerRole = Role.Server;
        }

        public static void Connected()
        {
            CurrentState = State.Connected;
        }

        public static void WorldLoaded()
        {
            IsSpawned = true;
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
            Connected
        }
    }

}
