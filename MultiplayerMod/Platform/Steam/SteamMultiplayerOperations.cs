using MultiplayerMod.Multiplayer;
using Steamworks;

namespace MultiplayerMod.Platform.Steam;

public class SteamMultiplayerOperations : IMultiplayerOperations {

    public void Join() {
        SteamFriends.ActivateGameOverlay("friends");
    }

}
