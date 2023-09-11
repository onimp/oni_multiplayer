using System;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Core.Unity;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.World.Debug;

public class WorldDebugSnapshotRunner : MultiplayerKMonoBehaviour, IRenderEveryTick {

    private WorldDebugSnapshot? current;

    private const float checkPeriod = 30.0f;
    private float lastTime;
    public static WorldDebugSnapshot? LastServerInfo { private get; set; }

    [Dependency]
    private readonly EventDispatcher eventDispatcher = null!;

    public static int ErrorsCount { get; private set; }

    public void RenderEveryTick(float dt) {
        if (GameClock.Instance.GetTime() - lastTime < checkPeriod)
            return;

        lastTime = GameClock.Instance.GetTime();
        try {
            CompareIfApplicable();
            current = WorldDebugSnapshot.Create();
            eventDispatcher.Dispatch(new DebugSnapshotAvailableEvent(current));
            CompareIfApplicable();
        } catch (Exception) { }
    }

    private void CompareIfApplicable() {
        if (LastServerInfo == null || Mathf.Abs(LastServerInfo.WorldTime - (current?.WorldTime ?? 0f)) > 0.5f)
            return;

        ErrorsCount = current != null ? WorldDebugSnapshotComparator.Compare(current, LastServerInfo, true) : 0;
        LastServerInfo = null;
    }

}
