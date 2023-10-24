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

    protected override void OnForcedCleanUp() => subscription.Cancel();

}
