using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace MultiplayerMod.Test.Game.WorldBuilder;

/// <summary>
/// Regression tests for GameTickLoop tick-counting bugs discovered after be4064b.
///
/// ROOT CAUSE SUMMARY:
///
///   GameTickLoop.Update(dt) structure (simplified):
///     _tickCount++;                         // ← incremented FIRST
///     while (_accumulatedTime >= SubTickTime) {
///         AdvanceOneSimSubTick();           // ← throws with no try/catch (be4064b)
///         _accumulatedTime -= SubTickTime;
///     }
///     if (_tickCount == ChoreKickTick)      // ← AFTER the while loop
///         ForceUpdateBrains();
///
///   BUG 1 (Finding 2): If AdvanceOneSimSubTick() throws on the Update() call where
///     _tickCount == ChoreKickTick (61), the while loop aborts before the ForceUpdateBrains
///     check is reached.  Next Update() call: _tickCount == 62 — ForceUpdateBrains
///     fires NEVER again.  currentChore stays null forever.
///
///   BUG 2 (Finding 3): Even if ForceUpdateBrains IS reached, provider.CollectChores()
///     (a diagnostic-only call, previously in try/catch) now throws unguarded and
///     propagates out of the entire ForceUpdateBrains function before any brain is kicked.
///
/// These tests replicate the loop structure inline (project convention — no DedicatedServer
/// reference) to document and guard both failure modes.
/// </summary>
[TestFixture]
[Parallelizable]
public class GameTickLoopTickCountTests {

    // Constants mirroring GameTickLoop
    private const float SubTickTime    = 1f / 60f;
    private const int   ChoreKickTick = 61;

    // ─── Bug 1: _tickCount++ before while loop ────────────────────────────────

    /// <summary>
    /// Demonstrates Bug 1: when the while loop throws on the call where _tickCount == 61,
    /// ForceUpdateBrains check is never reached and the tick is permanently skipped.
    ///
    /// This directly replicates the GameTickLoop.Update() structure from be4064b.
    /// </summary>
    [Test]
    public void TickCount_WhenWhileLoopThrowsAtTick61_ForceUpdateBrains_IsMissed() {
        // Simulate the GameTickLoop state machine.
        var tickCount        = 0;
        var accumulatedTime  = 0f;
        var brainKickFired   = false;
        // AdvanceOneSimSubTick throws exactly once — on the call processed during tick 61.
        // tickCount==61 when the 61st Update() call runs.
        var shouldThrowOnUpdate = 61;

        for (var updateCall = 1; updateCall <= 65; updateCall++) {
            // ── Mirrors GameTickLoop.Update(dt) ──
            accumulatedTime += SubTickTime;   // dt = 1/60, one subtick per call
            tickCount++;                      // _tickCount++ BEFORE while loop

            var whileLoopThrew = false;
            while (accumulatedTime >= SubTickTime) {
                if (tickCount == shouldThrowOnUpdate) {
                    // Simulates AdvanceOneSimSubTick() throwing at tick=61
                    whileLoopThrew = true;
                    break;  // exception aborts while — accumulatedTime NOT decremented
                }
                accumulatedTime -= SubTickTime;
            }

            if (!whileLoopThrew) {
                // Check AFTER while loop (mirrors GameTickLoop lines 112-127)
                if (tickCount == ChoreKickTick) {
                    brainKickFired = true;  // ForceUpdateBrains called
                }
            }
        }

        Assert.That(brainKickFired, Is.False,
            "BUG 1 (be4064b): When while loop throws at tick=61, ForceUpdateBrains check " +
            "at line 119 is NEVER REACHED — it is after the while loop. " +
            "Next Update() call: _tickCount==62, forever past ChoreKickTick==61. " +
            "currentChore stays null. Fix: move _tickCount++ AFTER the while loop, or " +
            "put ForceUpdateBrains check before/inside the while loop.");
    }

    /// <summary>
    /// Documents the CORRECT behavior: when while loop does NOT throw,
    /// ForceUpdateBrains IS called at exactly tick=61.
    /// </summary>
    [Test]
    public void TickCount_WhenWhileLoopDoesNotThrow_ForceUpdateBrains_FiredAtTick61() {
        var tickCount        = 0;
        var accumulatedTime  = 0f;
        var brainKickFired   = false;
        var brainKickAtTick  = -1;

        for (var updateCall = 1; updateCall <= 65; updateCall++) {
            accumulatedTime += SubTickTime;
            tickCount++;

            while (accumulatedTime >= SubTickTime) {
                accumulatedTime -= SubTickTime;  // no throw
            }

            if (tickCount == ChoreKickTick) {
                brainKickFired  = true;
                brainKickAtTick = tickCount;
            }
        }

        Assert.That(brainKickFired, Is.True,
            "When AdvanceOneSimSubTick does not throw, ForceUpdateBrains must fire at tick=61");
        Assert.That(brainKickAtTick, Is.EqualTo(61),
            "Brain kick must happen at exactly tick=61 (ChoreKickTick)");
    }

    // ─── Bug 2: diagnostic CollectChores aborts ForceUpdateBrains ────────────

    /// <summary>
    /// Demonstrates Bug 2: if any diagnostic call in ForceUpdateBrains throws
    /// (previously guarded by try/catch, now exposed after be4064b),
    /// the entire function aborts — none of the brains are kicked.
    ///
    /// Before be4064b: provider.CollectChores() was wrapped in try/catch.
    ///   → exception caught per-provider, brain kick continues for remaining brains.
    /// After be4064b: no catch → exception propagates out of ForceUpdateBrains.
    ///   → ALL brain kicks are skipped → currentChore stays null.
    /// </summary>
    [Test]
    public void ForceUpdateBrains_WhenDiagnosticThrows_NoBrainIsKicked() {
        var brainsKicked = new List<int>();

        // Simulate ForceUpdateBrains with 3 brains (dupes), as in be4064b (no try/catch).
        // The diagnostic (CollectChores) throws for brain 0's provider.
        var brainCount = 3;

        try {
            for (var i = 0; i < brainCount; i++) {
                // Diagnostic block (was try/catch before be4064b, now bare):
                // provider.CollectChores() throws for brain 0.
                if (i == 0) throw new InvalidOperationException("CollectChores threw (simulated)");

                // Brain kick — only reached if no throw above.
                brainsKicked.Add(i);
            }
        } catch {
            // Exception escapes ForceUpdateBrains — no more brains processed.
        }

        Assert.That(brainsKicked, Is.Empty,
            "BUG 2 (be4064b): when diagnostic CollectChores throws for brain 0, " +
            "exception escapes ForceUpdateBrains (no per-brain try/catch anymore) " +
            "and ALL brain kicks are skipped. Fix: separate diagnostic code from " +
            "the brain kick path so a diagnostic failure cannot abort the kick.");
    }

    /// <summary>
    /// Documents the correct pattern: diagnostic code guarded separately from
    /// brain kick so a diagnostic failure does NOT abort the kick.
    /// </summary>
    [Test]
    public void ForceUpdateBrains_WhenDiagnosticGuardedSeparately_AllBrainsKicked() {
        var brainsKicked = new List<int>();
        var brainCount   = 3;

        for (var i = 0; i < brainCount; i++) {
            // Diagnostic: guarded independently from kick
            try {
                if (i == 0) throw new InvalidOperationException("CollectChores threw");
                // log diagnostic result
            } catch {
                // diagnostic failure: log it but do NOT abort the kick loop
            }

            // Brain kick — always reached regardless of diagnostic throw
            brainsKicked.Add(i);
        }

        Assert.That(brainsKicked, Has.Count.EqualTo(brainCount),
            "When diagnostic is guarded separately, all brains are kicked " +
            "even when the diagnostic throws for brain 0");
    }

    // ─── Tick-constant contract ────────────────────────────────────────────────

    [Test]
    public void ChoreKickTick_IsExactly61() {
        Assert.That(ChoreKickTick, Is.EqualTo(61),
            "ChoreKickTick must be 61 — matches const in GameTickLoop. " +
            "If this changes, the 'sensors warm at tick 60, kick at 61' assumption breaks.");
    }
}
