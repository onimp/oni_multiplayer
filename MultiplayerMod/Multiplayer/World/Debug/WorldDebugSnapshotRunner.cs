using System;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.World.Debug;

public class WorldDebugSnapshotRunner : KMonoBehaviour, IRenderEveryTick {

    private WorldDebugSnapshot? current;
    public static event Action<WorldDebugSnapshot>? SnapshotAvailable;

    private const float checkPeriod = 30.0f;
    private float lastTime;
    public static WorldDebugSnapshot? LastServerInfo { private get; set; }

    public static int ErrorsCount { get; private set; }

    public void RenderEveryTick(float dt) {
        if (GameClock.Instance.GetTime() - lastTime < checkPeriod)
            return;

        lastTime = GameClock.Instance.GetTime();
        try {
            CompareIfApplicable();
            current = WorldDebugSnapshot.Create();
            SnapshotAvailable?.Invoke(current);
            CompareIfApplicable();
        } catch (Exception) { }
    }

    private void CompareIfApplicable() {
        if (LastServerInfo == null || Mathf.Abs(LastServerInfo.worldTime - (current?.worldTime ?? 0f)) > 0.5f)
            return;

        ErrorsCount = current?.Compare(LastServerInfo, true) ?? 0;
        LastServerInfo = null;
    }

}
