using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Network;
using UnityEngine;

namespace MultiplayerMod.Platform.Base.Network.Components;

public class ClientComponent : MonoBehaviour {
    private BaseClient client;
    private void Awake() => client = (BaseClient) Container.Get<IMultiplayerClient>();
    private void Update() => client.Tick();
}
