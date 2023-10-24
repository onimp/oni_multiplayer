using MultiplayerMod.Multiplayer.Players;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Components;

public class AssignedMultiplayerPlayer : MonoBehaviour {

    public MultiplayerPlayer Player { get; set; } = null!;

}
