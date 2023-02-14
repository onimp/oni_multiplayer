using System;

namespace MultiplayerMod.steam
{
    [Serializable]
    public enum Command
    {
        // DEBUG
        WorldDebugDiff = 0,
        
        // Usual commands
        LoadWorld = 1,
        PlayersState = 2,
        UserAction = 3,
        MouseMove = 4,
        ChoreSet = 5
    }
}