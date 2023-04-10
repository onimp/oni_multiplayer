using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Network;
using UnityEngine;

namespace MultiplayerMod.Platform.Steam.Network.Components;

public class SteamServerComponent : MonoBehaviour {
    private SteamServer server;
    private void Awake() => server = (SteamServer) Container.Get<IMultiplayerServer>();
    private void Update() => server.Tick();
}
