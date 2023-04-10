using System;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Debug;

public class WorldDebugSnapshotRunner : KMonoBehaviour, IRenderEveryTick {

    private WorldDebugSnapshot current;
    public static event Action<WorldDebugSnapshot> SnapshotAvailable;

    private const float CheckPeriod = 5.0f;
    private float lastTime;
    public static WorldDebugSnapshot LastServerInfo { private get; set; }

    public static int ErrorsCount { get; private set; }

    public void RenderEveryTick(float dt) {
        if (GameClock.Instance.GetTime() - lastTime < CheckPeriod)
            return;

        lastTime = GameClock.Instance.GetTime();
        CompareIfApplicable();
        current = WorldDebugSnapshot.Create();
        SnapshotAvailable?.Invoke(current);
        CompareIfApplicable();
    }

    private void CompareIfApplicable() {
        if (LastServerInfo == null || Mathf.Abs(LastServerInfo.worldTime - (current?.worldTime ?? 0f)) > 0.5f)
            return;

        ErrorsCount = current?.Compare(LastServerInfo, true) ?? 0;
        LastServerInfo = null;
    }

}
