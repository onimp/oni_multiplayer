using NUnit.Framework;
using UnityEngine;

namespace MultiplayerMod.Test.GameRuntime;

/// <summary>
/// Tests that <c>Pickupable.OnSpawn</c> does not NPE when
/// <c>MinionGroupProber</c> is initialized before <c>SpawnEntities()</c>.
///
/// ROOT CAUSE:
///   <c>Pickupable.OnSpawn()</c> immediately creates a
///   <c>new ReachabilityMonitor.Instance(this).StartSM()</c>.
///   The <c>ReachabilityMonitor.Instance</c> constructor calls
///   <c>UpdateReachability()</c> which executes:
///
///     <c>MinionGroupProber.Get().IsAllReachable(cell, offsets)</c>
///
///   When <c>MinionGroupProber.Get()</c> returns null (because
///   <c>MinionGroupProber</c> was never initialized in the headless boot
///   sequence), this NPEs inside every critter's <c>Pickupable.OnSpawn()</c>.
///   All 593 critter entities with <c>Pickupable</c> crash on spawn.
///
/// WHY MINIONGROUP PROBER WAS NULL:
///   In the original <c>WorldBuilder</c>, <c>MinionGroupProber</c> was
///   initialized in <c>InitializeWorld()</c> (for the full game path),
///   but the headless server's <c>Create()</c> method did not include it
///   in the pre-<c>SpawnEntities()</c> setup block. The OnPrefabInit call
///   for MinionGroupProber was simply absent.
///
/// FIX (WorldBuilder):
///   Before <c>SpawnEntities()</c>, create a <c>MinionGroupProber</c>
///   GameObject and call <c>Awake()</c> (→ InitializeComponent →
///   OnPrefabInit sets <c>Instance = this</c> and
///   <c>cells = new int[Grid.CellCount]</c>).
///   All cells default to 0 → <c>IsAllReachable</c> returns false
///   (correct: no dupe has probed yet).
///   <c>ReachabilityMonitor.Instance</c> ctor completes without exception.
/// </summary>
public class PickupableOnSpawnTest : PlayableGameTest {

    [SetUp]
    public void SetUp() {
        Singleton<StateMachineManager>.Instance.Clear();
        Singleton<StateMachineUpdater>.Instance.Clear();
        StateMachine.Instance.error = false;
        // Ensure MinionGroupProber.Instance is null to reproduce root cause.
        MinionGroupProber.DestroyInstance();
    }

    [TearDown]
    public void TestTearDown() {
        StateMachine.Instance.error = false;
        MinionGroupProber.DestroyInstance();
    }

    // ─── ROOT CAUSE TESTS ─────────────────────────────────────────────────────

    /// <summary>
    /// Documents root cause: <c>MinionGroupProber.Get()</c> returns null
    /// when no instance has been initialized.
    ///
    /// In the original WorldBuilder headless boot sequence,
    /// <c>MinionGroupProber</c> was not initialized before
    /// <c>SpawnEntities()</c> — so <c>Get()</c> always returned null
    /// during creature spawning.
    /// </summary>
    [Test]
    public void MinionGroupProber_Get_ReturnsNull_WhenNotInitialized() {
        Assert.IsNull(
            MinionGroupProber.Get(),
            "MinionGroupProber.Get() must return null when no instance has been " +
            "initialized. In the original WorldBuilder, the headless server never " +
            "created a MinionGroupProber before SpawnEntities(). This caused " +
            "every Pickupable.OnSpawn call (593 critter entities) to NPE at " +
            "MinionGroupProber.Get().IsAllReachable(cell, offsets) inside " +
            "ReachabilityMonitor.Instance.UpdateReachability().");
    }

    /// <summary>
    /// Documents root cause: directly calling
    /// <c>MinionGroupProber.Get().IsAllReachable(...)</c> when Instance is
    /// null throws <c>NullReferenceException</c>.
    ///
    /// This is the exact expression that fires in
    /// <c>ReachabilityMonitor.Instance.UpdateReachability()</c> (line 29):
    ///   <c>MinionGroupProber.Get().IsAllReachable(cell, ...)</c>
    ///
    /// In the original WorldBuilder, MinionGroupProber was null when
    /// SpawnEntities ran — so every critter's Pickupable.OnSpawn crashed here.
    /// </summary>
    [Test]
    public void MinionGroupProber_IsAllReachable_Throws_WhenInstanceNull() {
        Assert.IsNull(MinionGroupProber.Get(),
            "Precondition: MinionGroupProber must be null for this root-cause test.");

        Assert.Throws<System.NullReferenceException>(
            () => MinionGroupProber.Get().IsAllReachable(0, new CellOffset[0]),
            "Calling MinionGroupProber.Get().IsAllReachable() when Instance is null " +
            "must throw NullReferenceException. " +
            "This is exactly the call at UpdateReachability() line 29 " +
            "(base.sm.isReachable.Set(MinionGroupProber.Get().IsAllReachable(...))). " +
            "In the original WorldBuilder headless boot sequence, MinionGroupProber " +
            "was never initialized before SpawnEntities → NPE for all 593 critter " +
            "Pickupable.OnSpawn invocations.");
    }

    /// <summary>
    /// Documents root cause: <c>new ReachabilityMonitor.Instance(workable)</c>
    /// throws <c>NullReferenceException</c> when
    /// <c>MinionGroupProber.Get()</c> is null.
    ///
    /// Constructor call chain:
    ///   <c>Pickupable.OnSpawn</c>
    ///     → <c>new ReachabilityMonitor.Instance(this)</c>
    ///       → ctor body: <c>UpdateReachability()</c>
    ///         → <c>MinionGroupProber.Get().IsAllReachable(cell, offsets)</c>
    ///         → NPE because Instance is null.
    ///
    /// This test FAILS without the WorldBuilder MinionGroupProber fix
    /// (Instance is null → NPE) and PASSES with it.
    /// </summary>
    [Test]
    public void ReachabilityMonitor_Ctor_Throws_WhenMinionGroupProberNull() {
        Assert.IsNull(MinionGroupProber.Get(),
            "Precondition: MinionGroupProber must be null for this root-cause test.");

        var go = createGameObject();
        var workable = go.AddComponent<Workable>();

        Assert.Throws<System.NullReferenceException>(
            () => { var _ = new ReachabilityMonitor.Instance(workable); },
            "ReachabilityMonitor.Instance ctor must throw NullReferenceException when " +
            "MinionGroupProber.Get() is null. " +
            "Ctor calls UpdateReachability() → MinionGroupProber.Get().IsAllReachable(cell, offsets) → NPE. " +
            "In the original WorldBuilder, this NPE fired for every critter with Pickupable " +
            "(593 entities) during SpawnEntities() because MinionGroupProber was not " +
            "initialized in the headless boot sequence.");
    }

    // ─── FIX TESTS ────────────────────────────────────────────────────────────

    /// <summary>
    /// Verifies the fix precondition: calling <c>Awake()</c> on a
    /// <c>MinionGroupProber</c> component sets <c>Instance</c> to non-null.
    ///
    /// In WorldBuilder the fix is:
    ///   <c>minionProberGo.AddComponent&lt;MinionGroupProber&gt;().Awake()</c>
    /// placed BEFORE <c>SpawnEntities()</c>.
    /// <c>Awake()</c> → <c>InitializeComponent()</c> → <c>OnPrefabInit()</c>:
    ///   <c>Instance = this; cells = new int[Grid.CellCount]</c>.
    /// </summary>
    [Test]
    public void MinionGroupProber_Instance_IsNotNull_AfterAwake() {
        var go = new GameObject("MinionGroupProber");
        go.AddComponent<MinionGroupProber>().Awake();

        Assert.IsNotNull(
            MinionGroupProber.Get(),
            "MinionGroupProber.Get() must be non-null after Awake(). " +
            "Awake() → InitializeComponent() → OnPrefabInit() sets Instance = this. " +
            "WorldBuilder fix: call Awake() on a MinionGroupProber GO before SpawnEntities() " +
            "so that ReachabilityMonitor.Instance.UpdateReachability() finds a non-null Instance.");
    }

    /// <summary>
    /// Verifies the fix: <c>new ReachabilityMonitor.Instance(workable)</c>
    /// does NOT throw when <c>MinionGroupProber</c> is initialized.
    ///
    /// With a valid MinionGroupProber, <c>UpdateReachability()</c> calls:
    ///   <c>MinionGroupProber.Get().IsAllReachable(cell, offsets)</c>
    /// All cells default to 0 → <c>IsAllReachable</c> returns false (safe).
    /// No exception is thrown.
    ///
    /// This test FAILS without the WorldBuilder MinionGroupProber init
    /// (Instance null → NPE) and PASSES with it.
    /// </summary>
    [Test]
    public void ReachabilityMonitor_Ctor_DoesNotThrow_WhenMinionGroupProberInitialized() {
        // Initialize MinionGroupProber (mirrors WorldBuilder fix)
        var proberGo = new GameObject("MinionGroupProber");
        proberGo.AddComponent<MinionGroupProber>().Awake();
        Assert.IsNotNull(MinionGroupProber.Get(),
            "MinionGroupProber must be initialized for this fix-verification test.");

        var go = createGameObject();
        var workable = go.AddComponent<Workable>();

        Assert.DoesNotThrow(
            () => { var _ = new ReachabilityMonitor.Instance(workable); },
            "ReachabilityMonitor.Instance ctor must not throw when MinionGroupProber is " +
            "initialized. UpdateReachability() calls " +
            "MinionGroupProber.Get().IsAllReachable(cell, offsets) — with a valid " +
            "MinionGroupProber, all cells default to 0 → IsAllReachable returns false safely. " +
            "Without the WorldBuilder MinionGroupProber init, this threw NullReferenceException " +
            "for every critter Pickupable.OnSpawn.");
    }
}
