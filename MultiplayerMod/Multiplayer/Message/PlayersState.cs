using System;
using System.Collections.Generic;
using Steamworks;

namespace MultiplayerMod.Multiplayer.Message
{
    [Serializable]
    public class PlayersState
    {
        public Dictionary<CSteamID, PlayerState> PlayerStates = new Dictionary<CSteamID, PlayerState>();

        public void UpdateMousePos(CSteamID userId, Pair<float, float> mousePosition)
        {
            if (!PlayerStates.ContainsKey(userId)) PlayerStates[userId] = new PlayerState();
            PlayerStates[userId].MousePosition = mousePosition;
        }

        [Serializable]
        public class PlayerState
        {
            public Pair<float, float> MousePosition;
        }
    }
}
