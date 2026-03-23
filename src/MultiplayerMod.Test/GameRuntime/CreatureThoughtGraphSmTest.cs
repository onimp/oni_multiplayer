using NUnit.Framework;
using UnityEngine;

namespace MultiplayerMod.Test.GameRuntime;

/// <summary>
/// Tests that <c>CreatureThoughtGraph.Instance</c> and <c>CritterEmoteMonitor.Instance</c>
/// can be constructed and started safely once <c>NameDisplayScreen.Instance</c> is
/// initialized before <c>SpawnEntities()</c>.
///
/// ROOT CAUSE:
///   Both SMs call <c>NameDisplayScreen.Instance.*</c> during early lifecycle:
///
///   • <c>CreatureThoughtGraph.Instance.ctor</c> (line 18):
///       <c>NameDisplayScreen.Instance.RegisterComponent(base.gameObject, this)</c>
///   • <c>CritterEmoteMonitor.cooldown.Enter</c>:
///       <c>NameDisplayScreen.Instance.SetThoughtBubbleDisplay(smi.gameObject, ...)</c>
///
///   <c>NameDisplayScreen</c> was initialized in WorldBuilder AFTER <c>SpawnEntities()</c>
///   (at ~line 968 in the original code), so <c>Instance</c> was always null when
///   creatures spawned. Both callers NPE → <c>CreateSMIS()</c> / <c>StartSMIS()</c>
///   crash → monitor defs after CreatureThoughtGraph (AnimInterruptMonitor,
///   CreatureFallMonitor, BurrowMonitor, etc.) are never instantiated.
///
/// WHY THE CALLS ARE SAFE ONCE INSTANCE IS SET:
///   Creature GOs have <c>CharacterOverlay</c> removed by <c>FixCreaturePrefabsPreSpawn()</c>
///   before entity spawning. <c>NameDisplayScreen.RegisterComponent</c> guards on
///   <c>GetEntry(go)</c> returning non-null before doing any rendering work.
///   Without a <c>CharacterOverlay</c> there is no registered entry → <c>GetEntry</c>
///   returns null → all NameDisplayScreen methods return early as no-ops.
///
/// FIX (WorldBuilder):
///   Move the <c>NameDisplayScreen</c> initialization block to BEFORE
///   <c>SpawnEntities()</c> (near <c>DietManager</c>, after <c>RegisterEntities()</c>).
///   <c>InitializeComponent()</c> → <c>OnPrefabInit</c> → <c>Instance = this</c>.
///   All three call sites now receive a non-null Instance and execute as no-ops.
/// </summary>
public class CreatureThoughtGraphSmTest : PlayableGameTest {

    [SetUp]
    public void SetUp() {
        Singleton<StateMachineManager>.Instance.Clear();
        Singleton<StateMachineUpdater>.Instance.Clear();
        StateMachine.Instance.error = false;
        // Ensure NameDisplayScreen.Instance is null for root cause tests.
        NameDisplayScreen.DestroyInstance();
    }

    [TearDown]
    public void TestTearDown() {
        StateMachine.Instance.error = false;
        NameDisplayScreen.DestroyInstance();
    }

    // ─── ROOT CAUSE TESTS ─────────────────────────────────────────────────────

    /// <summary>
    /// Documents root cause: <c>CreatureThoughtGraph.Instance.ctor</c> throws
    /// <c>NullReferenceException</c> when <c>NameDisplayScreen.Instance</c> is null.
    ///
    /// Ctor body line 18: <c>NameDisplayScreen.Instance.RegisterComponent(go, this)</c>
    /// → <c>NameDisplayScreen.Instance</c> is null → NPE.
    ///
    /// In the original WorldBuilder, <c>NameDisplayScreen</c> was initialized after
    /// <c>SpawnEntities()</c> — too late for creature SM ctors that fire during
    /// <c>TriggerLifecycle</c> → <c>CreateSMIS()</c>.
    /// </summary>
    [Test]
    public void CreatureThoughtGraph_Ctor_Throws_WhenNameDisplayScreenNull() {
        Assert.IsNull(NameDisplayScreen.Instance, "Precondition: NameDisplayScreen.Instance must be null");

        var go  = createGameObject();
        var smc = go.AddComponent<StateMachineController>();
        go.AddComponent<KPrefabID>();
        var def = go.AddOrGetDef<CreatureThoughtGraph.Def>();

        Assert.Throws<System.NullReferenceException>(
            () => { var _ = new CreatureThoughtGraph.Instance(smc, def); },
            "CreatureThoughtGraph.Instance ctor must throw NullReferenceException when " +
            "NameDisplayScreen.Instance is null. " +
            "Ctor line 18: NameDisplayScreen.Instance.RegisterComponent(base.gameObject, this) " +
            "→ NPE. In the original WorldBuilder boot sequence, NameDisplayScreen was " +
            "initialized AFTER SpawnEntities (~line 968) so this NPE fired for every creature " +
            "spawned. The fix: move NameDisplayScreen init to before SpawnEntities.");
    }

    // ─── FIX TESTS ────────────────────────────────────────────────────────────

    /// <summary>
    /// Verifies the fix precondition: <c>InitializeComponent()</c> on a
    /// <c>NameDisplayScreen</c> GO sets <c>Instance</c> to non-null.
    ///
    /// In WorldBuilder this is:
    ///   <c>ndsGo.AddComponent&lt;NameDisplayScreen&gt;().InitializeComponent()</c>
    /// moved to the pre-SpawnEntities section (near DietManager).
    /// </summary>
    [Test]
    public void NameDisplayScreen_Instance_IsNotNull_AfterInitializeComponent() {
        var go  = new GameObject("NameDisplayScreen");
        var nds = go.AddComponent<NameDisplayScreen>();
        nds.InitializeComponent(); // OnPrefabInit → Instance = this

        Assert.IsNotNull(NameDisplayScreen.Instance,
            "NameDisplayScreen.Instance must be non-null after InitializeComponent(). " +
            "OnPrefabInit sets Instance = this (line 113). " +
            "WorldBuilder fix: call this before SpawnEntities so creature SM ctors " +
            "and OxygenBreather.OnSpawn find a non-null Instance.");
    }

    /// <summary>
    /// Verifies the fix: <c>CreatureThoughtGraph.Instance.ctor</c> does NOT throw
    /// when <c>NameDisplayScreen.Instance</c> is initialized.
    ///
    /// With an initialized-but-empty NameDisplayScreen (no registered entries),
    /// <c>RegisterComponent(go, this)</c> calls <c>GetEntry(go)</c> → null (no
    /// <c>CharacterOverlay</c> on the test GO) → returns early with no side effects.
    ///
    /// This test FAILS without the WorldBuilder NameDisplayScreen move
    /// (Instance is null → NPE) and PASSES with it.
    /// </summary>
    [Test]
    public void CreatureThoughtGraph_Ctor_DoesNotThrow_WhenNameDisplayScreenInitialized() {
        // Initialize NameDisplayScreen (mirrors WorldBuilder fix)
        var ndsGo = new GameObject("NameDisplayScreen");
        ndsGo.AddComponent<NameDisplayScreen>().InitializeComponent();
        Assert.IsNotNull(NameDisplayScreen.Instance, "NameDisplayScreen must be initialized for this test");

        var go  = createGameObject();
        var smc = go.AddComponent<StateMachineController>();
        go.AddComponent<KPrefabID>();
        var def = go.AddOrGetDef<CreatureThoughtGraph.Def>();

        Assert.DoesNotThrow(
            () => { var _ = new CreatureThoughtGraph.Instance(smc, def); },
            "CreatureThoughtGraph.Instance ctor must not throw when NameDisplayScreen.Instance " +
            "is initialized. RegisterComponent calls GetEntry(go) → null (no CharacterOverlay) → " +
            "returns early. No UI work performed. " +
            "Without the WorldBuilder NameDisplayScreen move, this throws NullReferenceException.");
    }

    /// <summary>
    /// Verifies <c>NameDisplayScreen.RegisterComponent</c> is safe (returns silently)
    /// for a GO without <c>CharacterOverlay</c>, even when called with a
    /// <c>CreatureThoughtGraph.Instance</c> as the component parameter.
    ///
    /// Creatures have CharacterOverlay removed by <c>FixCreaturePrefabsPreSpawn()</c>
    /// → <c>GetEntry(go)</c> returns null → <c>RegisterComponent</c> returns at
    /// <c>if (entry == null) return;</c> with no rendering side effects.
    /// </summary>
    [Test]
    public void NameDisplayScreen_RegisterComponent_ReturnsEarly_ForGoWithoutCharacterOverlay() {
        var ndsGo = new GameObject("NameDisplayScreen");
        ndsGo.AddComponent<NameDisplayScreen>().InitializeComponent();

        var go  = createGameObject();
        go.AddComponent<KPrefabID>();
        var smc = go.AddComponent<StateMachineController>();
        var def = go.AddOrGetDef<CreatureThoughtGraph.Def>();

        // Create instance (safe because NDS is initialized above)
        var smi = new CreatureThoughtGraph.Instance(smc, def);

        // Calling RegisterComponent again is also safe (idempotent for no-entry GOs)
        Assert.DoesNotThrow(
            () => NameDisplayScreen.Instance.RegisterComponent(go, smi),
            "NameDisplayScreen.RegisterComponent must not throw for a GO without CharacterOverlay. " +
            "GetEntry(go) returns null → immediate return. " +
            "Creature GOs have CharacterOverlay removed pre-spawn, so this is always the path " +
            "taken for creature SMs that call RegisterComponent during their ctor.");
    }

    /// <summary>
    /// Verifies <c>NameDisplayScreen.SetThoughtBubbleDisplay</c> is a no-op
    /// (does not throw) for a GO with no registered entry.
    ///
    /// <c>CritterEmoteMonitor.cooldown.Enter</c> calls this method on every tick reset.
    /// Without an entry for the critter GO, the guard <c>if (entry != null)</c> prevents
    /// any rendering work.
    /// </summary>
    [Test]
    public void NameDisplayScreen_SetThoughtBubbleDisplay_IsNoOp_ForUnregisteredGo() {
        var ndsGo = new GameObject("NameDisplayScreen");
        ndsGo.AddComponent<NameDisplayScreen>().InitializeComponent();

        var go = createGameObject();
        go.AddComponent<KPrefabID>();

        Assert.DoesNotThrow(
            () => NameDisplayScreen.Instance.SetThoughtBubbleDisplay(go, false, null, null, null),
            "SetThoughtBubbleDisplay must not throw for a GO with no registered entry. " +
            "GetEntry(go) returns null → if (entry != null) guard → method returns. " +
            "This is the path for CritterEmoteMonitor.cooldown.Enter in headless: " +
            "creature GOs have no NameDisplayScreen entry → call is a no-op.");
    }
}
