using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Network;
using UnityEngine;

namespace MultiplayerMod.Platform.Base.Network.Components;

public class ServerComponent : MonoBehaviour {
    private BaseServer server = null!;
    private void Awake() => server = (BaseServer) Container.Get<IMultiplayerServer>();
    private void Update() => server.Tick();
}
