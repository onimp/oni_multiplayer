using UnityEngine;

namespace DedicatedServer.Game;

/// <summary>
/// Drives game simulation — mirrors Game.SimEveryTick() for headless use.
///
/// Real game tick lifecycle (per Game.SimEveryTick):
///   simSubTick cycles 0-11 (12 subticks = 200ms of sim-time at 60fps)
///   Every 12th subtick: SimMessages.NewGameFrame → StepTheSim (SimDLL physics)
///   Every subtick: StateMachineUpdater.AdvanceOneSimSubTick (state machines, AI, sensors)
///
/// We mirror that exactly — one call to Update(dt) per frame advances both SimDLL and state machines.
/// </summary>
public class GameTickLoop {

    private const float SubTickTime = 1f / 60f;   // 16.67ms per subtick
    private const int SimFrameSubTicks = 12;       // SimDLL advances every 12 subticks (200ms)

    private float _accumulatedTime;
    private int _simSubTick;
    private readonly System.Action _tickSimDll;

    /// <param name="tickSimDll">Called every 200ms to advance SimDLL (pass WorldBuilder.TickSimulation)</param>
    public GameTickLoop(System.Action tickSimDll) {
        _tickSimDll = tickSimDll;
    }

    /// <summary>Advance simulation by dt seconds. Call from main loop at ~60fps or any rate.</summary>
    public void Update(float dt) {
        var clampedDt = Mathf.Min(dt, 0.2f);
        _accumulatedTime += clampedDt;

        while (_accumulatedTime >= SubTickTime) {
            _simSubTick = (_simSubTick + 1) % SimFrameSubTicks;
            if (_simSubTick == 0) {
                _tickSimDll();
                // After each 200ms SimDLL step, process nav dirty cells. Mirrors Game.UnsafeSim200ms()
                // which calls Pathfinding.Instance.UpdateNavGrids() after world.UpdateCellInfo().
                // Without this, tile changes from the sim never propagate to the nav graph.
                Pathfinding.Instance?.UpdateNavGrids();
            }
            // Advances all SIM_EVERY_TICK / SIM_33ms / SIM_200ms / SIM_1000ms / SIM_4000ms buckets
            Singleton<StateMachineUpdater>.Instance.AdvanceOneSimSubTick();
            _accumulatedTime -= SubTickTime;
        }

        // Drive RENDER_EVERY_TICK bucket — covers BrainScheduler (Dupe/Creature AI)
        Singleton<StateMachineUpdater>.Instance.RenderEveryTick(clampedDt);

        // Dispatch async path probe work orders to the background thread and apply completed
        // PathGrid results back to navigators. Mirrors Game.LateUpdate() where TickFrame()
        // is called every Unity frame. Without this, PathGrid costs are never populated
        // → Navigator.GetNavigationCost() always returns -1 → path-cost-based chore selection fails.
        AsyncPathProber.Instance?.TickFrame();
    }
}
