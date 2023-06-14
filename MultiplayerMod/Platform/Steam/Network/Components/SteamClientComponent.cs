using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Network;
using UnityEngine;

namespace MultiplayerMod.Platform.Steam.Network.Components;

public class SteamClientComponent : MonoBehaviour {
    private BaseClient client;
    private void Awake() => client = (BaseClient) Container.Get<IMultiplayerClient>();
    private void Update() => client.Tick();
}
