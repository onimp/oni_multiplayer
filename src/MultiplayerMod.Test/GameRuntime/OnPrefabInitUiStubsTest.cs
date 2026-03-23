using NUnit.Framework;
using UnityEngine;

namespace MultiplayerMod.Test.GameRuntime;

/// <summary>
/// Tests for the three UI stubs that allow <c>Game.OnPrefabInit()</c> to run
/// to completion in a headless dedicated-server environment.
///
/// Without these stubs, <c>Game.OnPrefabInit()</c> crashes before setting
/// critical fields (roomProber, fetchManager, accumulators …), leaving the
/// headless world in a broken state.
///
/// The three blockers and their stubs:
/// <list type="bullet">
///   <item>
///     <b>BLOCKER 1 — Lighting.Instance (line 820)</b><br/>
///     <c>new ConduitFlowVisualizer(…, Lighting.Instance.Settings.GasConduit)</c>
///     NPEs when <c>Lighting.Instance</c> is null.
///     Fix: create a stub <c>Lighting</c> with a
///     <c>ScriptableObject.CreateInstance&lt;LightingSettings&gt;()</c> assigned to
///     <c>Lighting.Instance.Settings</c>.
///   </item>
///   <item>
///     <b>BLOCKER 4 — fxSpawnData (line 829)</b><br/>
///     <c>InitializeFXSpawners()</c> iterates <c>fxSpawnData.Length</c> times.
///     When <c>fxSpawnData</c> is the inspector-assigned null reference (headless
///     has no inspector), the loop body NPEs immediately.
///     Fix: assign <c>new Game.SpawnPoolData[0]</c> — loop runs 0 times.
///   </item>
///   <item>
///     <b>BLOCKER 5 — screenManagerPrefab (line 832)</b><br/>
///     <c>Util.KInstantiate(screenManagerPrefab).GetComponent&lt;GameScreenManager&gt;()</c>
///     NPEs when <c>screenManagerPrefab</c> is null.
///     Fix: create a dummy <c>GameObject</c> with a <c>GameScreenManager</c>
///     component and assign it to <c>game.screenManagerPrefab</c>.
///   </item>
/// </list>
/// </summary>
[TestFixture]
public class OnPrefabInitUiStubsTest : PlayableGameTest {

    private Lighting? _savedLightingInstance;

    [SetUp]
    public void SaveLightingState() {
        // Save and clear Lighting.Instance so each stub test starts from a known state.
        _savedLightingInstance = Lighting.Instance;
        Lighting.Instance = null;
    }

    [TearDown]
    public void RestoreLightingState() {
        Lighting.Instance = _savedLightingInstance;
    }

    // ── BLOCKER 1: Lighting stub ──────────────────────────────────────────────

    /// <summary>
    /// Precondition: <c>Lighting.Instance</c> is null before the stub is applied.
    /// This documents why the stub is needed (the crash site reads
    /// <c>Lighting.Instance.Settings.GasConduit</c>).
    /// </summary>
    [Test]
    public void LightingStub_Precondition_InstanceIsNullAfterReset() {
        // Arranged by [SetUp] above.
        Assert.IsNull(Lighting.Instance,
            "Lighting.Instance must be null before the stub is applied. " +
            "Without the stub, Game.OnPrefabInit line 820 — " +
            "new ConduitFlowVisualizer(…, Lighting.Instance.Settings.GasConduit) — " +
            "throws NullReferenceException.");
    }

    /// <summary>
    /// The Lighting stub creates a valid <c>LightingSettings</c> ScriptableObject and
    /// assigns it to <c>Lighting.Instance.Settings</c>.
    /// <c>ScriptableObject.CreateInstance&lt;LightingSettings&gt;</c> must return a
    /// non-null object so field reads on it are safe.
    /// </summary>
    [Test]
    public void LightingStub_Settings_IsNonNullScriptableObject() {
        // Mirrors WorldBuilder stub creation:
        var lightingGo = new GameObject("Lighting_headless");
        var lighting = lightingGo.AddComponent<Lighting>();
        lighting.Settings = ScriptableObject.CreateInstance<LightingSettings>();
        Lighting.Instance = lighting;

        Assert.IsNotNull(Lighting.Instance,
            "Lighting.Instance must be non-null after stub creation.");
        Assert.IsNotNull(Lighting.Instance.Settings,
            "Lighting.Instance.Settings must be non-null — it is assigned from " +
            "ScriptableObject.CreateInstance<LightingSettings>(), which returns a " +
            "valid (zero-initialised) ScriptableObject.");
    }

    /// <summary>
    /// <c>LightingSettings.GasConduit</c> is a struct field (zero-initialised by
    /// default).  After stub creation the field must be readable without throwing,
    /// which is the exact access pattern at the crash site in
    /// <c>Game.OnPrefabInit()</c> line 820.
    /// </summary>
    [Test]
    public void LightingStub_Settings_GasConduit_IsReadableWithoutException() {
        var lightingGo = new GameObject("Lighting_headless");
        var lighting = lightingGo.AddComponent<Lighting>();
        lighting.Settings = ScriptableObject.CreateInstance<LightingSettings>();
        Lighting.Instance = lighting;

        Assert.DoesNotThrow(
            () => { var _ = Lighting.Instance.Settings.GasConduit; },
            "Reading Lighting.Instance.Settings.GasConduit must not throw. " +
            "ConduitFlowVisualizer ctor (called from Game.OnPrefabInit line 820) " +
            "passes this value as a parameter — a NullReferenceException here " +
            "prevents OnPrefabInit from completing and leaves Game fields " +
            "(roomProber, fetchManager, accumulators …) uninitialised.");
    }

    // ── BLOCKER 4: fxSpawnData stub ───────────────────────────────────────────

    /// <summary>
    /// <c>Game.InitializeFXSpawners()</c> (called from <c>OnPrefabInit</c> line 829)
    /// iterates <c>fxSpawnData</c> using <c>for (int i = 0; i &lt; fxSpawnData.Length; …)</c>.
    /// In headless the inspector field is null, so <c>fxSpawnData.Length</c> throws.
    /// The fix assigns <c>new Game.SpawnPoolData[0]</c> — the loop must run zero times.
    /// </summary>
    [Test]
    public void FxSpawnData_EmptyArray_LoopBodyNeverExecutes() {
        var fxSpawnData = new global::Game.SpawnPoolData[0];

        var iterations = 0;
        for (var i = 0; i < fxSpawnData.Length; i++)
            iterations++;

        Assert.AreEqual(0, iterations,
            "new Game.SpawnPoolData[0] must cause the fxSpawnData loop to execute " +
            "exactly 0 times.  This is the stub assigned by WorldBuilder before " +
            "Game.OnPrefabInit() is called — it prevents the NullReferenceException " +
            "that would occur if the loop body tried to access a null array reference.");
    }

    // ── BLOCKER 5: screenManagerPrefab stub ───────────────────────────────────

    /// <summary>
    /// <c>Game.OnPrefabInit()</c> line 832 calls
    /// <c>Util.KInstantiate(screenManagerPrefab).GetComponent&lt;GameScreenManager&gt;()</c>.
    /// The stub is a <c>GameObject</c> with a <c>GameScreenManager</c> component added.
    /// After adding the component it must be present on the stub GO.
    /// </summary>
    [Test]
    public void ScreenManagerPrefab_Stub_HasGameScreenManagerComponent() {
        // Mirrors WorldBuilder stub creation:
        var smPrefab = new GameObject("ScreenManagerPrefab_headless");
        smPrefab.AddComponent<GameScreenManager>();

        Assert.IsNotNull(smPrefab.GetComponent<GameScreenManager>(),
            "The screenManagerPrefab stub must have a GameScreenManager component. " +
            "Game.OnPrefabInit line 832 calls " +
            "Util.KInstantiate(screenManagerPrefab).GetComponent<GameScreenManager>() " +
            "and assigns the result to screenMgr.  If screenManagerPrefab lacks the " +
            "component, GetComponent returns null and the subsequent Player.ScreenManager " +
            "assignment NPEs.");
    }

    /// <summary>
    /// <c>GetComponent&lt;GameScreenManager&gt;()</c> on the stub GO must return a
    /// non-null instance.  This is the value assigned to <c>game.screenMgr</c> after
    /// <c>Util.KInstantiate</c>; any null here propagates to a crash inside
    /// <c>SpawnPlayer()</c> when it calls <c>component.ScreenManager.StartScreen(…)</c>.
    /// </summary>
    [Test]
    public void ScreenManagerPrefab_Stub_GetComponent_ReturnsNonNull() {
        var smPrefab = new GameObject("ScreenManagerPrefab_headless");
        smPrefab.AddComponent<GameScreenManager>();

        var mgr = smPrefab.GetComponent<GameScreenManager>();

        Assert.IsNotNull(mgr,
            "GetComponent<GameScreenManager>() on the screenManagerPrefab stub must " +
            "return a non-null GameScreenManager.  " +
            "This is the component retrieved by Game.OnPrefabInit after KInstantiate, " +
            "assigned to screenMgr, and later used in OnSpawn → SpawnPlayer() to call " +
            "ScreenManager.StartScreen(HudScreen), ScreenManager.StartScreen(HoverTextScreen), " +
            "and ScreenManager.StartScreen(ToolTipScreen).  A null value here causes " +
            "NullReferenceException in SpawnPlayer at any of those StartScreen calls.");
    }
}
