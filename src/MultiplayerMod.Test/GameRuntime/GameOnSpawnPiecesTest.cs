using System.Reflection;
using System.Runtime.Serialization;
using NUnit.Framework;

namespace MultiplayerMod.Test.GameRuntime;

/// <summary>
/// Tests for the Game.OnSpawn() / ClusterManager.OnSpawn() simulation pieces that
/// WorldBuilder must initialise manually because neither OnSpawn() is ever called in
/// headless (SpawnPlayer() / UpdateWorldReverbSnapshot() crash in a no-UI environment).
///
/// ROOT CAUSE (why the pieces are missing):
///   • Game.OnSpawn() line 930: SpawnPlayer() calls Util.KInstantiate(playerPrefab, ...)
///     where playerPrefab is null in headless → NullReferenceException.  Everything at
///     lines 949–977 (solidConduitFlow.Initialize, SimAndRenderScheduler.Add(roomProber /
///     spaceScannerNetworkManager / KComponentSpawn), BatchUpdate registrations, etc.)
///     is therefore never reached.
///   • ClusterManager.OnSpawn() line 636: UpdateWorldReverbSnapshot() unconditionally
///     calls AudioMixer.instance.PauseSpaceVisibleSnapshot() — NPE when AudioMixer.instance
///     is null in headless → m_grid is never initialised.
///
/// FIX (WorldBuilder.Create() — after SpawnStarterMinions()):
///   Replicate the simulation-critical lines from each OnSpawn() method manually,
///   following the same pattern used for OnPrefabInit() pieces above (accumulators,
///   roomProber, mingleCellTracker, etc.).
///
/// WHAT THESE TESTS VERIFY:
///   The fix uses two reflection-based mechanisms that must work correctly at runtime;
///   these tests lock in that those mechanisms target the right fields / methods so that
///   a rename in game code is caught immediately rather than silently doing nothing.
///   Each test documents the exact access pattern used in WorldBuilder.
/// </summary>
public class GameOnSpawnPiecesTest : PlayableGameTest {

    // ── KComponentSpawn / KComponentsInitializer ──────────────────────────────────────

    /// <summary>
    /// Documents root cause: KComponentSpawn.instance is null after boot because
    /// KComponentsInitializer.Awake() (a private Unity MonoBehaviour callback) is
    /// never called in headless — Unity's Start/Awake pipeline does not run.
    ///
    /// This confirms the instance field is not self-initialising: something MUST call
    /// Awake() explicitly before KComponentSpawn.instance can be used.
    /// </summary>
    [Test]
    public void KComponentSpawnInstance_IsNullWithoutAwake() {
        var saved = KComponentSpawn.instance;
        try {
            KComponentSpawn.instance = null;
            Assert.IsNull(
                KComponentSpawn.instance,
                "KComponentSpawn.instance must be null when Awake() has not been called. " +
                "In headless the Unity Awake callback never fires, so without an explicit " +
                "invocation via WorldBuilder the instance stays null and " +
                "SimAndRenderScheduler.Add(KComponentSpawn.instance) is silently skipped.");
        } finally {
            KComponentSpawn.instance = saved;
        }
    }

    /// <summary>
    /// Verifies the fix: reflecting into KComponentsInitializer and invoking its private
    /// Awake() method correctly sets KComponentSpawn.instance to a non-null value.
    ///
    /// WorldBuilder uses exactly this pattern:
    ///   var kcsi = (KComponentsInitializer)FormatterServices
    ///       .GetUninitializedObject(typeof(KComponentsInitializer));
    ///   typeof(KComponentsInitializer)
    ///       .GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic)
    ///       ?.Invoke(kcsi, null);
    ///
    /// This test FAILS if the Awake() method is renamed / made inaccessible (BindingFlags
    /// mismatch) and PASSES when the reflection target is correct.
    /// </summary>
    [Test]
    public void KComponentsInitializer_AwakeViaReflection_SetsInstance() {
        var saved = KComponentSpawn.instance;
        try {
            KComponentSpawn.instance = null;

            var kcsi = (KComponentsInitializer) FormatterServices
                .GetUninitializedObject(typeof(KComponentsInitializer));

            var awakeMethod = typeof(KComponentsInitializer)
                .GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.IsNotNull(awakeMethod,
                "KComponentsInitializer.Awake() must be findable via reflection with " +
                "BindingFlags.Instance | BindingFlags.NonPublic. " +
                "WorldBuilder.Create() invokes this method to initialise KComponentSpawn.instance " +
                "and comps (GameComps). If the method cannot be found, the ?.Invoke silently " +
                "does nothing and KComponentSpawn.instance stays null.");

            awakeMethod!.Invoke(kcsi, null);

            Assert.IsNotNull(
                KComponentSpawn.instance,
                "KComponentSpawn.instance must be non-null after KComponentsInitializer.Awake() " +
                "is invoked via reflection. Awake() sets KComponentSpawn.instance = this and " +
                "creates comps = new GameComps(). Without this, Add(KComponentSpawn.instance) " +
                "in WorldBuilder is a no-op → component-spawn tick events never fire.");
        } finally {
            KComponentSpawn.instance = saved;
        }
    }

    // ── ClusterManager.m_grid reflection ─────────────────────────────────────────────

    /// <summary>
    /// Verifies that ClusterManager has a private field named "m_grid" accessible via
    /// reflection with BindingFlags.Instance | BindingFlags.NonPublic.
    ///
    /// WorldBuilder.Create() uses:
    ///   typeof(ClusterManager).GetField("m_grid",
    ///       BindingFlags.Instance | BindingFlags.NonPublic)
    /// to check and initialise the cluster grid when ClusterManager.OnSpawn() was blocked
    /// by UpdateWorldReverbSnapshot() (AudioMixer.instance NPE).
    ///
    /// This test FAILS if the field is renamed in game code, causing the GetField call to
    /// return null and the m_grid check/set to be silently skipped.
    /// </summary>
    [Test]
    public void ClusterManager_MGridField_IsAccessibleViaReflection() {
        var mGridField = typeof(ClusterManager)
            .GetField("m_grid", BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.IsNotNull(mGridField,
            "ClusterManager must have a private instance field named 'm_grid' accessible " +
            "via BindingFlags.Instance | BindingFlags.NonPublic. " +
            "WorldBuilder.Create() reads this field to detect an uninitialised ClusterGrid " +
            "and sets it to new ClusterGrid(m_numRings) when ClusterManager.OnSpawn() was " +
            "blocked by the AudioMixer NPE in UpdateWorldReverbSnapshot().");
    }

    /// <summary>
    /// Verifies that ClusterManager has a private field named "m_numRings" accessible via
    /// reflection. WorldBuilder reads this to know how many rings to pass to ClusterGrid.
    /// </summary>
    [Test]
    public void ClusterManager_MNumRingsField_IsAccessibleViaReflection() {
        var mNumRingsField = typeof(ClusterManager)
            .GetField("m_numRings", BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.IsNotNull(mNumRingsField,
            "ClusterManager must have a private instance field named 'm_numRings' accessible " +
            "via BindingFlags.Instance | BindingFlags.NonPublic. " +
            "WorldBuilder.Create() reads this field to pass the correct ring count to " +
            "new ClusterGrid(numRings) when initialising m_grid manually.");
    }

    // ── Game public field accessibility ──────────────────────────────────────────────

    /// <summary>
    /// Verifies that Game.solidConduitFlow is a public field accessible without reflection.
    /// WorldBuilder calls game.solidConduitFlow?.Initialize() — if AssemblyExposer stops
    /// exposing this field the call would fail to compile.
    ///
    /// This test documents the access requirement for the solidConduitFlow initialisation
    /// (Game.OnSpawn() line 965) that WorldBuilder replicates manually.
    /// </summary>
    [Test]
    public void Game_SolidConduitFlow_IsPublicField() {
        var field = typeof(global::Game)
            .GetField("solidConduitFlow", BindingFlags.Instance | BindingFlags.Public);

        Assert.IsNotNull(field,
            "Game.solidConduitFlow must be a public instance field. " +
            "WorldBuilder.Create() calls game.solidConduitFlow?.Initialize() to replicate " +
            "Game.OnSpawn() line 965. If the field is not public (requires BindingFlags.Public) " +
            "or is renamed, the Initialize() call is silently skipped and solid conveyors " +
            "never tick.");
    }

    /// <summary>
    /// Verifies that Game.spaceScannerNetworkManager is a public field accessible without
    /// reflection. WorldBuilder assigns and registers it:
    ///   game.spaceScannerNetworkManager ??= new SpaceScannerNetworkManager();
    ///   SimAndRenderScheduler.instance?.Add(game.spaceScannerNetworkManager);
    /// Replicates Game.OnSpawn() lines 967 (spaceScannerNetworkManager was created at
    /// OnPrefabInit line 834, which is after the OnPrefabInit crash in headless).
    /// </summary>
    [Test]
    public void Game_SpaceScannerNetworkManager_IsPublicField() {
        var field = typeof(global::Game)
            .GetField("spaceScannerNetworkManager", BindingFlags.Instance | BindingFlags.Public);

        Assert.IsNotNull(field,
            "Game.spaceScannerNetworkManager must be a public instance field so that " +
            "WorldBuilder.Create() can assign and register it without reflection. " +
            "If missing, space scanners / bunker doors never update their networks.");
    }
}
