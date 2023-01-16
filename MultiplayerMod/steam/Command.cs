using System;

namespace MultiplayerMod.steam
{
    [Serializable]
    public enum Command
    {
        // TODO are those special or user actions as well?
        Pause = 1,
        Unpause = 2,
        LoadWorld = 3,
        PlayersState = 4,
        UserAction = 5
    }
}