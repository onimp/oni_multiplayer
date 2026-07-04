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

    private static WorldDebugSnapshot? lastServerInfo;

    public static WorldDebugSnapshot? LastServerInfo {
        private get => lastServerInfo;
        set {
            lastServerInfo = value;
            if (value != null)
                LastHostSnapshot = value;
        }
    }

    // Persistent copy of the most recent host snapshot for the duplicant sync inspector.
    // Unlike LastServerInfo (which is nulled after each aggregate comparison), this is kept so the
    // DevTool always has the latest host state to diff local duplicants against.
    public static WorldDebugSnapshot? LastHostSnapshot { get; private set; }

    [InjectDependency]
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
