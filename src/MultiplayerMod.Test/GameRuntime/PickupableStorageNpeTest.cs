using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DedicatedServer.Game;
using NUnit.Framework;
using UnityEngine;

namespace MultiplayerMod.Test.GameRuntime;

/// <summary>
/// Tests for the Pickupable.UpdateCachedCell Storage NPE fix.
///
/// ROOT CAUSE:
///   <c>Pickupable.UpdateCachedCell(int cell)</c> contains:
///   <code>
///     if (KPrefabID.HasTag(GameTags.PickupableStorage))
///         GetComponent&lt;Storage&gt;().UpdateStoredItemCachedCells();
///   </code>
///
///   <c>PickupableStorage</c> tag is set exclusively by
///   <c>EntityTemplates.ExtendEntityToDehydratedFoodPackage()</c>,
///   which also calls <c>template.AddComponent&lt;Storage&gt;()</c>.
///   In headless, <c>CloneSingle</c> (MemberwiseClone-based) shares the
///   <c>KPrefabID.tags</c> HashSet between the prefab and all its clones.
///   As a result, clones correctly return <c>HasTag(PickupableStorage) = true</c>,
///   but <c>GetComponent&lt;Storage&gt;()</c> may return null due to
///   component-tracking edge cases in the headless runtime.
///
///   <c>null.UpdateStoredItemCachedCells()</c> throws
///   <c>NullReferenceException</c> at IL offset 0x0053.  This fires in
///   BOTH <c>OnPrefabInit</c> AND <c>OnSpawn</c> (each calls
///   <c>UpdateCachedCell</c>), generating 2 errors per entity.
///   With 593 affected entities: 1186 errors total.
///   Each exception inside a state-machine Enter/GoTo delegate sets
///   <c>StateMachine.Instance.error = true</c>, halting all SMs globally.
///
/// FIX (UnityRuntime.TriggerLifecycle — Phase 0.2):
///   Before Phase 1 (InitializeComponent on all components), check:
///   if the GO has PickupableStorage tag AND is missing Storage,
///   add Storage via <c>go.AddComponent&lt;Storage&gt;()</c>.
///   Because the GO is already in <c>SpawnedObjects</c> at Phase 0.2,
///   <c>AddComponent</c> calls <c>InitializeComponent</c> on the new
///   Storage immediately.  When <c>Pickupable.OnPrefabInit</c> fires
///   in Phase 1, <c>GetComponent&lt;Storage&gt;()</c> finds the
///   component and <c>UpdateStoredItemCachedCells()</c> runs safely
///   on an empty list (no NPE, no globalSMError).
/// </summary>
public class PickupableStorageNpeTest : PlayableGameTest {

    [SetUp]
    public void SetUp() {
        StateMachine.Instance.error = false;
    }

    [TearDown]
    public void TestTearDown() {
        StateMachine.Instance.error = false;
    }

    // ─── ROOT CAUSE TESTS ─────────────────────────────────────────────────────

    /// <summary>
    /// Documents root cause: <c>KPrefabID.HasTag(PickupableStorage)</c> returns
    /// <c>true</c> for an entity whose KPrefabID has the PickupableStorage tag —
    /// exactly as set by <c>ExtendEntityToDehydratedFoodPackage()</c>.
    ///
    /// This confirms the branch in <c>UpdateCachedCell</c> is entered and
    /// <c>GetComponent&lt;Storage&gt;()</c> is reached.
    /// </summary>
    [Test]
    public void KPrefabID_HasTag_ReturnsTrue_WhenPickupableStorageAdded() {
        var go = new GameObject("PickupableStorage_TagCheck");
        var kpid = go.AddComponent<KPrefabID>();
        kpid.AddTag(GameTags.PickupableStorage);

        Assert.IsTrue(kpid.HasTag(GameTags.PickupableStorage),
            "KPrefabID.HasTag(PickupableStorage) must return true when the tag was added. " +
            "ExtendEntityToDehydratedFoodPackage() calls component.AddTag(GameTags.PickupableStorage) " +
            "on the prefab KPrefabID. MemberwiseClone shares the tags HashSet with all clones, " +
            "so HasTag returns true for all clones — causing the Storage-null branch to be reached.");
    }

    /// <summary>
    /// Documents root cause: <c>GetComponent&lt;Storage&gt;()</c> returns
    /// <c>null</c> when no <c>Storage</c> component is present on the GO.
    ///
    /// In headless, component-tracking edge cases in <c>CloneSingle</c> can
    /// cause Storage to be absent on a clone even though the prefab had it.
    /// </summary>
    [Test]
    public void GetComponent_Storage_ReturnsNull_WhenStorageAbsent() {
        var go = new GameObject("PickupableStorage_NoStorage");
        go.AddComponent<KPrefabID>().AddTag(GameTags.PickupableStorage);

        Assert.IsNull(go.GetComponent<Storage>(),
            "GetComponent<Storage>() must return null when no Storage component is present. " +
            "In the headless clone path (CloneSingle/MemberwiseClone), Storage may be absent " +
            "from the clone GO's component list despite the prefab having it. " +
            "This is the null reference that causes the NPE in UpdateCachedCell at IL_0053.");
    }

    /// <summary>
    /// Documents root cause: dereferencing a null <c>Storage</c> reference in
    /// <c>UpdateStoredItemCachedCells()</c> throws <c>NullReferenceException</c>.
    ///
    /// This is the exact call site: <c>GetComponent&lt;Storage&gt;().UpdateStoredItemCachedCells()</c>
    /// at IL offset 0x0053 in <c>Pickupable.UpdateCachedCell</c>.
    /// </summary>
    [Test]
    public void NullStorage_UpdateStoredItemCachedCells_ThrowsNullReferenceException() {
        Storage nullStorage = null!;

        Assert.Throws<NullReferenceException>(
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            () => nullStorage!.UpdateStoredItemCachedCells(),
            "Calling UpdateStoredItemCachedCells() on a null Storage reference must throw " +
            "NullReferenceException. This is the exact call at IL_0053 in UpdateCachedCell: " +
            "  GetComponent<Storage>().UpdateStoredItemCachedCells() " +
            "when GetComponent returns null. " +
            "The exception propagates through OnPrefabInit → TriggerLifecycle catch → logged, " +
            "but also propagates through OnSpawn and any SM Enter() delegate → " +
            "StateMachine.Instance.error = true → ALL state machines halt globally.");
    }

    // ─── FIX TESTS ────────────────────────────────────────────────────────────
    //
    // NOTE ON TEST INFRASTRUCTURE:
    //   The test environment uses two separate component-tracking systems:
    //   • UnityTestRuntime.companionData — tracks GOs/components for Harmony-patched
    //     Unity InternalCalls (AddComponent, GetComponent, SetActive, etc.)
    //   • UnityRuntime.GameObjectComponents — tracks GOs/components for the dedicated
    //     server production path (TriggerLifecycle reads/writes this dictionary)
    //
    //   In the test environment:
    //   • SetActive is patched to a NO-OP (TriggerLifecycle is never called from test code)
    //   • go.AddComponent<T>() routes to UnityTestRuntime, NOT UnityRuntime.AddComponent
    //
    //   To test TriggerLifecycle.Phase 0.2 we must:
    //   1. Set up the GO in UnityRuntime.GameObjectComponents via reflection (bridging the gap)
    //   2. Call UnityRuntime.TriggerLifecycle(go) directly (bypasses SetActive no-op)
    //   3. Phase 0.2 uses UnityRuntime.AddComponent(go, typeof(Storage)) — static direct call —
    //      which writes to GameObjectComponents regardless of Harmony routing
    //   4. Assert via GameObjectComponents (not go.GetComponent which goes through companionData)
    //
    //   The fix uses AddComponent(go, typeof(Storage)) (static) NOT go.AddComponent<Storage>()
    //   (InternalCall) precisely so this test infrastructure alignment works.

    // Access UnityRuntime private state via reflection (GameObjectComponents is private static).
    // DedicatedServer is now a project reference of this test project.
    private static readonly FieldInfo _gameObjectComponentsField =
        typeof(UnityRuntime).GetField(
            "GameObjectComponents", BindingFlags.Static | BindingFlags.NonPublic)!;

    private static readonly FieldInfo _spawnedObjectsField =
        typeof(UnityRuntime).GetField(
            "SpawnedObjects", BindingFlags.Static | BindingFlags.NonPublic)!;

    /// <summary>
    /// Registers a GO directly in <c>UnityRuntime.GameObjectComponents</c> so that
    /// <c>TriggerLifecycle</c> can find it (bypasses the UnityTestRuntime companionData gap).
    /// Returns the component list for assertions after <c>TriggerLifecycle</c>.
    /// </summary>
    private static List<Component> RegisterInUnityRuntime(GameObject go, params Component[] initial) {
        var goc = (Dictionary<IntPtr, List<Component>>)_gameObjectComponentsField.GetValue(null)!;
        var list = new List<Component>(initial);
        goc[go.m_CachedPtr] = list;
        return list;
    }

    private static void UnregisterFromUnityRuntime(GameObject go) {
        var goc = (Dictionary<IntPtr, List<Component>>)_gameObjectComponentsField.GetValue(null)!;
        var spo = (HashSet<IntPtr>)_spawnedObjectsField.GetValue(null)!;
        goc.Remove(go.m_CachedPtr);
        spo.Remove(go.m_CachedPtr);
    }

    /// <summary>
    /// Verifies the fix: <c>TriggerLifecycle</c> Phase 0.2 adds <c>Storage</c> to
    /// any GO that has <c>PickupableStorage</c> tag but is missing the component.
    ///
    /// Precondition: GO has KPrefabID with PickupableStorage tag, but NO Storage.
    /// After <c>TriggerLifecycle</c> runs, Storage must be present in
    /// <c>UnityRuntime.GameObjectComponents</c> for this GO.
    ///
    /// This test FAILS without the Phase 0.2 guard (Storage stays null → NPE when
    /// Pickupable.OnPrefabInit calls UpdateCachedCell) and PASSES with it.
    /// </summary>
    [Test]
    public void TriggerLifecycle_AddsStorage_ForPickupableStorage_WhenStorageAbsent() {
        var go = new GameObject("DehydratedFoodPackage_MissingStorage");
        var kpid = go.AddComponent<KPrefabID>();
        kpid.AddTag(GameTags.PickupableStorage);  // tag present (from ExtendEntityToDehydratedFoodPackage)
        // Storage deliberately absent — reproduces the headless clone-tracking bug

        // Bridge UnityTestRuntime → UnityRuntime: register go in GameObjectComponents so
        // TriggerLifecycle can find it (in the test env, new GameObject() only registers in companionData)
        var components = RegisterInUnityRuntime(go, kpid);

        Assert.IsFalse(components.Any(c => c is Storage),
            "Precondition: Storage must be absent in GameObjectComponents before TriggerLifecycle.");

        UnityRuntime.TriggerLifecycle(go);  // Phase 0.2: finds kpid, sees missing Storage, calls AddComponent

        Assert.IsTrue(components.Any(c => c is Storage),
            "Storage must be added to GameObjectComponents by TriggerLifecycle Phase 0.2 when " +
            "PickupableStorage tag is present but Storage component is absent. " +
            "Fix: Phase 0.2 calls UnityRuntime.AddComponent(go, typeof(Storage)) (static direct " +
            "call) which writes to GameObjectComponents regardless of InternalCall routing. " +
            "This ensures Pickupable.UpdateCachedCell (called from both OnPrefabInit and OnSpawn) " +
            "finds a non-null Storage and UpdateStoredItemCachedCells() completes safely.");

        UnregisterFromUnityRuntime(go);  // clean up
    }

    /// <summary>
    /// Verifies the fix is a no-op when Storage is already present.
    ///
    /// If Storage is correctly cloned onto the GO (no component-tracking bug),
    /// Phase 0.2 must not add a duplicate Storage.
    /// </summary>
    [Test]
    public void TriggerLifecycle_DoesNotDuplicateStorage_WhenAlreadyPresent() {
        var go = new GameObject("DehydratedFoodPackage_WithStorage");
        var kpid = go.AddComponent<KPrefabID>();
        kpid.AddTag(GameTags.PickupableStorage);
        var storage = go.AddComponent<Storage>();  // Storage already present (normal clone path)

        // Bridge UnityTestRuntime → UnityRuntime with both kpid and storage already present
        var components = RegisterInUnityRuntime(go, kpid, storage);

        UnityRuntime.TriggerLifecycle(go);  // Phase 0.2 should skip AddComponent (Storage present)

        var storageCount = components.Count(c => c is Storage);
        Assert.AreEqual(1, storageCount,
            "TriggerLifecycle Phase 0.2 must NOT add a second Storage when one is already " +
            "present. AddComponent(go, typeof(Storage)) is only called when " +
            "GetComponent(go, typeof(Storage)) returns null — the guard is an AddOrGet-style check.");

        UnregisterFromUnityRuntime(go);  // clean up
    }
}
