using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Core.Extensions;
using MultiplayerMod.Core.Unity;
using MultiplayerMod.Multiplayer.Players;
using MultiplayerMod.Multiplayer.Players.Events;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Components;

public class CursorManager : MultiplayerKMonoBehaviour {

    [InjectDependency]
    private readonly EventDispatcher events = null!;

    [InjectDependency]
    private readonly MultiplayerGame multiplayer = null!;

    private EventSubscription subscription = null!;

    protected override void OnSpawn() {
        subscription = events.Subscribe<PlayerJoinedEvent>(OnPlayerJoined);
        multiplayer.Players.ForEach(CreatePlayerCursor);
    }

    private void OnPlayerJoined(PlayerJoinedEvent @event) => CreatePlayerCursor(@event.Player);

    protected override void OnForcedCleanUp() => subscription.Cancel();

    private void CreatePlayerCursor(MultiplayerPlayer player) {
        if (player == multiplayer.Players.Current)
            return;
        var canvas = GameScreenManager.Instance.GetParent(GameScreenManager.UIRenderTarget.ScreenSpaceOverlay);
        var cursorName = $"{player.Profile.PlayerName}'s cursor";
        var cursor = new GameObject(cursorName) { transform = { parent = canvas.transform } };
        cursor.AddComponent<AssignedMultiplayerPlayer>().Player = player;
        cursor.AddComponent<CursorComponent>();
        cursor.AddComponent<DestroyOnPlayerLeave>();
    }

}
