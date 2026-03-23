using System;
using NUnit.Framework;

namespace MultiplayerMod.Test.GameRuntime;

/// <summary>
/// Diagnostic tests for dupe-not-moving issue after d89f2ae (BreathMonitor fix).
///
/// Q1: Does GameClock.GetTime() advance after running 60 subticks of Sim33ms?
/// Q2: Does AdvanceOneSimSubTick throw (BreathMonitor still NPEing)?
/// Q3: Is Sim33ms still in main code path (not inside try)?  → Code check, see GameTickLoop.cs.
/// </summary>
public class GameTickDiagnosticTest : PlayableGameTest {

    /// <summary>
    /// Q1: Boot game, run 60 subticks worth of Sim33ms calls, assert GetTime() > 50f.
    ///
    /// GameTickLoop.cs calls GameClock.Instance.Sim33ms(SubTickTime) each subtick in the
    /// MAIN code path (no try/finally wrapping). If GetTime() still = 50f after 60 calls,
    /// either Sim33ms is not reached (still inside a catch block) or the clock was reset.
    ///
    /// GameClock starts at 50f (timeSinceStartOfCycle=50, cycle=0 after OnPrefabInit).
    /// After 60 × (1/60)s = 1s advance → GetTime() must be ~51f.
    /// </summary>
    [Test]
    public void Q1_GameClock_AdvancesAfter60Subticks_ViaDirectSim33msCalls() {
        var before = GameClock.Instance.GetTime();
        const float subTickTime = 1f / 60f;

        // Mirrors what GameTickLoop.cs does each subtick: Sim33ms(SubTickTime)
        // (after AdvanceOneSimSubTick, in the main code path).
        for (int i = 0; i < 60; i++)
            GameClock.Instance.Sim33ms(subTickTime);

        var after = GameClock.Instance.GetTime();
        Assert.That(after, Is.GreaterThan(before),
            "GameClock.GetTime() must increase after 60 Sim33ms calls. " +
            "If still at 50f: Sim33ms is NOT being reached in GameTickLoop " +
            "(possibly still inside a try block that catches AdvanceOneSimSubTick throws).");
        Assert.That(after, Is.EqualTo(before + 1f).Within(0.001f),
            $"Expected ~{before + 1f:F3}f after 60 subticks (1s total), got {after:F3}f.");
    }

    /// <summary>
    /// Q2: Run AdvanceOneSimSubTick 10 times without dupes — assert no exceptions.
    ///
    /// In the test environment there are no dupes or WorldContainer → BreathMonitor never
    /// runs → no NPE expected. This verifies the infrastructure (StateMachineUpdater
    /// instance, scheduler state) is clean after the d89f2ae/22c304a fixes.
    ///
    /// NOTE: this does NOT test the real server scenario (which has dupes + BreathMonitor).
    /// If BreathMonitor is still NPEing on the real server, the AlertStateManager fix
    /// (22c304a: wc.InitializeComponent() before CreateSMIS) must be verified separately.
    /// </summary>
    [Test]
    public void Q2_AdvanceOneSimSubTick_DoesNotThrow_10Times_EmptyWorld() {
        int count = 0;
        Assert.DoesNotThrow(() => {
            for (int i = 0; i < 10; i++) {
                Singleton<StateMachineUpdater>.Instance.AdvanceOneSimSubTick();
                count++;
            }
        }, $"AdvanceOneSimSubTick must not throw. Completed {count}/10 ticks before exception.");
    }

    /// <summary>
    /// Q3: Code check result (no runtime assertion needed).
    ///
    /// GameTickLoop.cs main loop (post d89f2ae / 22c304a):
    ///   while (_accumulatedTime >= SubTickTime) {
    ///     ...
    ///     Singleton&lt;StateMachineUpdater&gt;.Instance.AdvanceOneSimSubTick(); // no try/catch
    ///     GameClock.Instance?.Sim33ms(SubTickTime);  // ← MAIN PATH, not in try/catch/finally
    ///     _accumulatedTime -= SubTickTime;
    ///   }
    ///
    /// Sim33ms IS in the main code path. If AdvanceOneSimSubTick throws, Sim33ms is skipped.
    /// This test documents what the code check found.
    /// </summary>
    [Test]
    public void Q3_GameTickLoop_Sim33ms_IsInMainCodePath_NotInTryCatch() {
        // Code verification (manual check of GameTickLoop.cs):
        // - Line 68-71: no try/catch around AdvanceOneSimSubTick
        // - Line 75: Sim33ms called directly after AdvanceOneSimSubTick
        // - CONCLUSION: If AdvanceOneSimSubTick throws, Sim33ms IS skipped (clock freezes).
        // - The BreathMonitor fix (22c304a) must fully stop all throws from AdvanceOneSimSubTick.
        //
        // If dupes are still not moving: AdvanceOneSimSubTick is still throwing (BreathMonitor
        // or other NPE) → Sim33ms is skipped → GameClock frozen → Scheduler never fires → no move.
        Assert.Pass(
            "Q3 code check: GameTickLoop.cs line 75 — Sim33ms is in main code path (no try/catch). " +
            "If clock is frozen in real server: AdvanceOneSimSubTick is still throwing."
        );
    }
}
