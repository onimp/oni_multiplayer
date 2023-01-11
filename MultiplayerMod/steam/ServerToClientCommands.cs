using System;

namespace MultiplayerMod.steam
{
    [Serializable]
    public enum Command
    {
        Pause = 1,
        Unpause = 2,
        LoadWorld = 3,
        CursorMove = 4
    }
}