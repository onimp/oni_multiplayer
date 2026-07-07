using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Core.Unity;
using MultiplayerMod.Multiplayer.Players.Events;

namespace MultiplayerMod.Multiplayer.Components;

public class DestroyOnPlayerLeave : MultiplayerKMonoBehaviour {

    [InjectDependency]
    private readonly EventDispatcher events = null!;

    [MyCmpReq]
    private readonly AssignedMultiplayerPlayer playerComponent = null!;

    private EventSubscription subscription = null!;

    protected override void OnSpawn() {
        var player = playerComponent.Player;
        subscription = events.Subscribe<PlayerLeftEvent>(@event => {
            if (@event.Player == player)
                DestroyImmediate(gameObject);
        });
    }

    // OnForcedCleanUp runs from OnDestroy, which fires even when the component is torn down (app quit /
    // scene unload) before OnSpawn ran - so subscription can still be null. Guard it: an unconditional
    // .Cancel() NRE'd out of OnDestroy during shutdown.
    protected override void OnForcedCleanUp() => subscription?.Cancel();

}
