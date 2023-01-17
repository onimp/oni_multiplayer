using System;

namespace MultiplayerMod.steam
{
    [Serializable]
    public enum Command
    {
        LoadWorld = 1,
        PlayersState = 2,
        UserAction = 3,
        MouseMove = 4
    }
}