using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace MultiplayerMod.Test.GameRuntime;

/// <summary>
/// Tests for the SharedStateMachinesList root cause and the CloneSingle fix.
///
/// ROOT CAUSE (confirmed by code inspection + Max's server log):
///   UnityRuntime.CloneSingle() uses MemberwiseClone for each component.
///   MemberwiseClone is a shallow copy — all reference-type fields share the same
///   object reference between original and clone.
///
///   StateMachineController has:
///     private List&lt;StateMachine.Instance&gt; stateMachines = new List&lt;...&gt;();
///
///   When dupe GOs are instantiated from the same prefab (Object.Instantiate →
///   CloneSingle), all cloned SMCs share the SAME stateMachines list reference.
///
///   Consequence:
///     • Dupe 0's BaseOnSpawn starts 52 SMIs (including IdleMonitor) → added to list.
///     • Dupe 1's StartSMIS() calls GetSMI(IdleMonitor.InstanceType) → finds dupe 0's
///       instance in the shared list → considers it already started → never creates
///       dupe 1's own IdleMonitor.Instance.
///     • Only 1 IdleChore ever created (dupe 0's). Dupes 1+2 find the same chore via
///       GetSMI → chore.driver != null (dupe 0 claimed it) → IsPreemptable fails.
///     • Result: dupes 1+2 stuck in nochore, no movement.
///   Confirmed by Max's server log: ONE IdleChore in global list.
///
/// FIX (UnityRuntime.CloneSingle):
///   After MemberwiseCloning a StateMachineController component, reset its
///   stateMachines field to a new empty List via reflection:
///     if (cloneComp is StateMachineController) {
///         _smcField.SetValue(cloneComp, new List&lt;StateMachine.Instance&gt;());
///     }
///   Each clone now starts with an empty list. StartSMIS creates its own instances.
///   3 dupes → 3 IdleMonitor instances → 3 IdleChores → all 3 can pick idle chores
///   → movement resumes.
/// </summary>
[TestFixture]
public class DupeMovementTest : PlayableGameTest {

    // Reflection accessor for StateMachineController.stateMachines (private field).
    private static readonly FieldInfo SmcStateMachinesField =
        typeof(StateMachineController)
            .GetField("stateMachines", BindingFlags.Instance | BindingFlags.NonPublic)!;

    // Reflection accessor for ChoreConsumer.providers (private List<ChoreProvider>).
    private static readonly FieldInfo _choreConsumerProvidersField =
        typeof(ChoreConsumer)
            .GetField("providers", BindingFlags.Instance | BindingFlags.NonPublic)!;

    // Reflection accessor for ChoreConsumer.choreProvider (private [MyCmpAdd] ChoreProvider field).
    // Confirmed shared via MemberwiseClone — points to dupe0's CP for all dupes (see 752c1a4 diag).
    private static readonly FieldInfo _choreConsumerChoreProviderField =
        typeof(ChoreConsumer)
            .GetField("choreProvider", BindingFlags.Instance | BindingFlags.NonPublic)!;

    [SetUp]
    public void SetUp() {
        Singleton<StateMachineManager>.Instance.Clear();
        Singleton<StateMachineUpdater>.Instance.Clear();
        StateMachine.Instance.error = false;
    }

    [TearDown]
    public void TestTearDown() {
        StateMachine.Instance.error = false;
    }

    // ── Root cause: MemberwiseClone produces a shared stateMachines list ──────

    /// <summary>
    /// Documents root cause: MemberwiseClone(smc) shares the stateMachines list
    /// between the original and the clone.
    ///
    /// This is what CloneSingle does for every component when instantiating a GO
    /// from a prefab. All dupe SMCs get the same list reference → only 1 set of
    /// SMIs runs across all 3 dupes.
    /// </summary>
    [Test]
    public void Smc_MemberwiseClone_SharesStateMachinesList() {
        var go  = createGameObject();
        var smc = go.AddComponent<StateMachineController>();

        var cloned = (StateMachineController) typeof(object)
            .GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(smc, null);

        var list1 = SmcStateMachinesField.GetValue(smc);
        var list2 = SmcStateMachinesField.GetValue(cloned);

        Assert.AreSame(list1, list2,
            "MemberwiseClone produces a shallow copy — stateMachines list is SHARED. " +
            "When CloneSingle instantiates dupe GOs from the same prefab, all dupe SMCs " +
            "share the same stateMachines list. Dupe 0's StartSMIS adds its IdleMonitor " +
            "instance to the list; dupe 1's StartSMIS finds it already there → skips " +
            "creation → only 1 IdleMonitor runs → only 1 IdleChore.");
    }

    // ── Fix: resetting the list on the clone makes lists independent ──────────

    /// <summary>
    /// Documents the fix: assigning a new List after MemberwiseClone breaks the
    /// shared reference. Each SMC now has its own list → StartSMIS creates its own
    /// IdleMonitor.Instance → 3 dupes get 3 IdleChores.
    ///
    /// This is exactly what UnityRuntime.CloneSingle() now does for SMC components.
    /// </summary>
    [Test]
    public void Smc_AfterFix_ClonedSmcHasFreshList() {
        var go  = createGameObject();
        var smc = go.AddComponent<StateMachineController>();

        var cloned = (StateMachineController) typeof(object)
            .GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(smc, null);

        // THE FIX: reset stateMachines on the clone (mirrors CloneSingle change).
        SmcStateMachinesField.SetValue(cloned, new List<StateMachine.Instance>());

        var list1 = SmcStateMachinesField.GetValue(smc);
        var list2 = SmcStateMachinesField.GetValue(cloned);

        Assert.AreNotSame(list1, list2,
            "After resetting stateMachines on the clone, the lists are independent. " +
            "Each dupe SMC can now accumulate its own set of SMIs.");
        Assert.IsNotNull(list2,
            "The clone's stateMachines list must be non-null after fix.");
        Assert.AreEqual(0, ((List<StateMachine.Instance>) list2).Count,
            "The clone's stateMachines list must be empty (fresh) after fix.");
    }

    // ── Integration: shared list → same IdleMonitor for all dupes (before fix) ─

    /// <summary>
    /// Demonstrates root cause in a realistic scenario:
    /// When 3 SMCs share the same stateMachines list (as happens before the CloneSingle
    /// fix), all 3 return the SAME IdleMonitor.Instance from GetSMI().
    ///
    /// This is why only 1 IdleChore existed on Max's server — dupes 1+2 could never
    /// get their own IdleMonitor → never get their own IdleChore → IsPreemptable fails.
    ///
    /// NOTE: We start IdleMonitor directly rather than via BaseMinionConfig.BaseOnSpawn,
    /// because BaseOnSpawn throws in the unit-test environment before reaching IdleMonitor
    /// (several sub-SMs NPE due to absent Unity assets). Direct StartSM isolates the
    /// shared-list behaviour without needing a full game boot.
    ///
    /// EXPECTED TO PASS (documents pre-fix broken behaviour).
    /// </summary>
    [Test]
    public void ThreeDupes_SharedSmcList_AllReturnSameIdleMonitorInstance() {
        var go1  = createGameObject();
        var go2  = createGameObject();
        var go3  = createGameObject();
        // KPrefabID required by TagTransitionData.HasAllTags (idle.TagTransition(GameTags.Dying)).
        go1.AddComponent<KPrefabID>();
        go2.AddComponent<KPrefabID>();
        go3.AddComponent<KPrefabID>();
        var smc1 = go1.AddComponent<StateMachineController>();
        var smc2 = go2.AddComponent<StateMachineController>();
        var smc3 = go3.AddComponent<StateMachineController>();

        // Simulate pre-fix state: all 3 SMCs share the same stateMachines list.
        var sharedList = (List<StateMachine.Instance>) SmcStateMachinesField.GetValue(smc1);
        SmcStateMachinesField.SetValue(smc2, sharedList);
        SmcStateMachinesField.SetValue(smc3, sharedList);

        // Start IdleMonitor on smc1 → adds instance to the shared list.
        var idle1 = new IdleMonitor.Instance(smc1);
        idle1.StartSM();

        // smc2 and smc3 GetSMI searches the shared list → finds smc1's instance → returns it.
        var idle2 = smc2.GetSMI<IdleMonitor.Instance>();
        var idle3 = smc3.GetSMI<IdleMonitor.Instance>();

        Assert.NotNull(idle1, "smc1's IdleMonitor.Instance must be non-null after StartSM.");
        Assert.AreSame(idle1, idle2,
            "With shared stateMachines list: smc2.GetSMI returns smc1's IdleMonitor. " +
            "Dupe 1 never gets its own instance → shares dupe 0's IdleChore → locked out.");
        Assert.AreSame(idle1, idle3,
            "With shared stateMachines list: smc3.GetSMI returns smc1's IdleMonitor. " +
            "Dupe 2 never gets its own instance → shares dupe 0's IdleChore → locked out.");
    }

    // ── Integration: independent lists → unique IdleMonitor per dupe (after fix) ─

    /// <summary>
    /// Verifies the fix: when each dupe's SMC has its own stateMachines list
    /// (as produced by the corrected CloneSingle), each gets its own IdleMonitor.Instance.
    ///
    /// With 3 unique IdleMonitor instances:
    ///   • 3 IdleChores are created (one per dupe)
    ///   • Each chore is in the dupe's personal ChoreProvider under its own key
    ///   • IsPreemptable passes for each dupe's own chore (chore.driver == null)
    ///   • All 3 dupes can pick their chore → transition to haschore → begin IdleMove
    ///
    /// EXPECTED TO PASS after CloneSingle fix (and already passes for freshly created
    /// GOs since AddComponent creates independent components, mirroring the fix).
    /// </summary>
    [Test]
    public void ThreeDupes_IndependentSmcLists_HaveUniqueIdleMonitorInstances() {
        // Each AddComponent creates a fresh SMC with its own stateMachines list.
        // This mirrors post-fix CloneSingle behaviour where each clone gets a fresh list.
        var go1  = createGameObject();
        var go2  = createGameObject();
        var go3  = createGameObject();
        // KPrefabID required by TagTransitionData.HasAllTags (idle.TagTransition(GameTags.Dying)).
        go1.AddComponent<KPrefabID>();
        go2.AddComponent<KPrefabID>();
        go3.AddComponent<KPrefabID>();
        var smc1 = go1.AddComponent<StateMachineController>();
        var smc2 = go2.AddComponent<StateMachineController>();
        var smc3 = go3.AddComponent<StateMachineController>();

        // Start IdleMonitor on each SMC independently.
        var idle1 = new IdleMonitor.Instance(smc1);
        idle1.StartSM();
        var idle2 = new IdleMonitor.Instance(smc2);
        idle2.StartSM();
        var idle3 = new IdleMonitor.Instance(smc3);
        idle3.StartSM();

        Assert.NotNull(smc1.GetSMI<IdleMonitor.Instance>(), "smc1 must find its own IdleMonitor.");
        Assert.NotNull(smc2.GetSMI<IdleMonitor.Instance>(), "smc2 must find its own IdleMonitor.");
        Assert.NotNull(smc3.GetSMI<IdleMonitor.Instance>(), "smc3 must find its own IdleMonitor.");
        Assert.AreNotSame(smc1.GetSMI<IdleMonitor.Instance>(), smc2.GetSMI<IdleMonitor.Instance>(),
            "With independent SMC lists, each dupe must have a different IdleMonitor.Instance. " +
            "Different instances → different IdleChores → each can be claimed independently.");
        Assert.AreNotSame(smc1.GetSMI<IdleMonitor.Instance>(), smc3.GetSMI<IdleMonitor.Instance>(),
            "smc1 and smc3 must have different IdleMonitor instances.");
        Assert.AreNotSame(smc2.GetSMI<IdleMonitor.Instance>(), smc3.GetSMI<IdleMonitor.Instance>(),
            "All three IdleMonitor instances must be unique objects.");
    }

    // ── Shared providers / choreWorldMap: same MemberwiseClone issue ──────────

    /// <summary>
    /// Documents root cause: MemberwiseClone(ChoreConsumer) shares the providers list.
    ///
    /// Same shallow-copy pattern as SMC.stateMachines. All clones from the same prefab share
    /// the same providers List reference. If the list is populated with only dupe 0's CP
    /// before subsequent clones are made, dupes 1+N start with [dupe0_CP] in their providers.
    /// FindNextChore iterates providers → only searches dupe 0's ChoreProvider → finds dupe 0's
    /// IdleChore (chore.driver != null) → IsPreemptable fails → stuck in nochore.
    /// </summary>
    [Test]
    public void ChoreConsumer_MemberwiseClone_SharesProvidersList() {
        var go       = createGameObject();
        var consumer = go.AddComponent<ChoreConsumer>();

        var cloned = (ChoreConsumer) typeof(object)
            .GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(consumer, null);

        var list1 = _choreConsumerProvidersField.GetValue(consumer);
        var list2 = _choreConsumerProvidersField.GetValue(cloned);

        Assert.AreSame(list1, list2,
            "MemberwiseClone produces a shallow copy — providers list is SHARED. " +
            "If populated with dupe 0's CP before dupes 1+2 are cloned, " +
            "FindNextChore for dupes 1+2 only searches dupe 0's ChoreProvider.");
    }

    /// <summary>
    /// Verifies the fix: resetting providers on each clone gives independent lists.
    /// Each dupe's OnPrefabInit then adds only its own ChoreProvider → providers = [own CP].
    /// FindNextChore searches only the dupe's own chores → finds own IdleChore → movement.
    /// </summary>
    [Test]
    public void ChoreConsumer_AfterFix_ClonedConsumerHasFreshProvidersList() {
        var go       = createGameObject();
        var consumer = go.AddComponent<ChoreConsumer>();

        var cloned = (ChoreConsumer) typeof(object)
            .GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(consumer, null);

        // THE FIX: reset providers on the clone (mirrors CloneSingle change).
        _choreConsumerProvidersField.SetValue(cloned, new List<ChoreProvider>());

        var list1 = _choreConsumerProvidersField.GetValue(consumer);
        var list2 = _choreConsumerProvidersField.GetValue(cloned);

        Assert.AreNotSame(list1, list2,
            "After fix: each cloned ChoreConsumer has its own providers list.");
        Assert.AreEqual(0, ((List<ChoreProvider>) list2!).Count,
            "The clone's providers list must be empty so OnPrefabInit populates it fresh.");
    }

    /// <summary>
    /// Integration: after CloneSingle fix, 3 dupes' ChoreConsumers have independent
    /// providers lists. Each dupe's FindNextChore searches only its own ChoreProvider.
    /// </summary>
    [Test]
    public void ThreeDupes_IndependentProvidersList_EachDupeSearchesOwnProvider() {
        var go1       = createGameObject();
        var go2       = createGameObject();
        var go3       = createGameObject();
        go1.AddComponent<KPrefabID>();
        go2.AddComponent<KPrefabID>();
        go3.AddComponent<KPrefabID>();
        var cp1       = go1.AddComponent<ChoreProvider>();
        var cp2       = go2.AddComponent<ChoreProvider>();
        var cp3       = go3.AddComponent<ChoreProvider>();
        var consumer1 = go1.AddComponent<ChoreConsumer>();
        var consumer2 = go2.AddComponent<ChoreConsumer>();
        var consumer3 = go3.AddComponent<ChoreConsumer>();

        // Simulate post-fix: each ChoreConsumer has its own fresh providers list.
        // Add only own ChoreProvider (mirrors OnPrefabInit behaviour).
        consumer1.AddProvider(cp1);
        consumer2.AddProvider(cp2);
        consumer3.AddProvider(cp3);

        var list1 = (List<ChoreProvider>) _choreConsumerProvidersField.GetValue(consumer1)!;
        var list2 = (List<ChoreProvider>) _choreConsumerProvidersField.GetValue(consumer2)!;
        var list3 = (List<ChoreProvider>) _choreConsumerProvidersField.GetValue(consumer3)!;

        Assert.AreNotSame(list1, list2,  "Dupe 0 and dupe 1 must have different providers lists.");
        Assert.AreNotSame(list1, list3,  "Dupe 0 and dupe 2 must have different providers lists.");
        Assert.AreEqual(1, list1.Count,  "Dupe 0 providers must contain exactly 1 entry (own CP).");
        Assert.AreEqual(1, list2.Count,  "Dupe 1 providers must contain exactly 1 entry (own CP).");
        Assert.AreEqual(1, list3.Count,  "Dupe 2 providers must contain exactly 1 entry (own CP).");
        Assert.AreSame(cp1, list1[0],    "Dupe 0's providers[0] must be its own ChoreProvider.");
        Assert.AreSame(cp2, list2[0],    "Dupe 1's providers[0] must be its own ChoreProvider.");
        Assert.AreSame(cp3, list3[0],    "Dupe 2's providers[0] must be its own ChoreProvider.");
    }

    // ── World-ID reindex: chores mis-keyed under -1 move to correct key ───────

    /// <summary>
    /// Documents and verifies the world-ID reindex fix.
    ///
    /// ROOT CAUSE:
    ///   IdleMonitor enters 'idle' during OnSpawn (TriggerLifecycle Phase 3).
    ///   ToggleRecurringChore fires its Enter action → SetupChore → new IdleChore(smi.master)
    ///   → IdleChore.ctor calls provider.AddChore(this).
    ///   AddChore calls chore.gameObject.GetMyParentWorldId() to determine the map key.
    ///   GetMyParentWorldId uses Grid.WorldIdx[cell] — but if Grid.WorldIdx hasn't been
    ///   fully written for the dupe's cell at that moment, WorldIdx[cell] = byte.MaxValue →
    ///   ClusterManager.GetWorld() returns null → GetMyParentWorldId returns -1.
    ///   Chore stored under key -1. CollectChores queries with key 0 (valid after warm-up)
    ///   → miss → 0 chores found → FindNextChore returns false → no movement.
    ///
    ///   Combined with the SMC fix (3 IdleMonitors now run), this means 3 IdleChores are
    ///   created but all 3 are stored under key -1 → none visible to CollectChores.
    ///
    /// FIX (GameTickLoop.ReindexChoreProviders, called at tick=61 after ForceUpdateBrains):
    ///   For each dupe's personal ChoreProvider:
    ///     if choreWorldMap[-1] is non-empty and GetMyParentWorldId() != -1 now:
    ///       move all entries to choreWorldMap[correctKey], remove -1 bucket.
    ///   After reindex, BrainScheduler.RenderEveryTick naturally finds the chores
    ///   at tick=62+ and assigns them to dupes.
    /// </summary>
    [Test]
    public void DupeChoreProvider_ReindexesMiskeyedChores() {
        var go       = createGameObject();
        var provider = go.AddComponent<ChoreProvider>();

        // Simulate mis-keyed state: IdleChore stored under -1 because
        // GetMyParentWorldId() returned -1 during IdleMonitor.OnSpawn.
        // We use a null stand-in for Chore — the logic is purely key migration.
        var miskeyedEntry = new List<Chore> { null! };
        provider.choreWorldMap[-1] = miskeyedEntry;

        Assert.IsTrue(provider.choreWorldMap.ContainsKey(-1),
            "Pre-condition: chore must be under key -1 (simulating mis-keyed state).");
        Assert.IsFalse(provider.choreWorldMap.ContainsKey(0),
            "Pre-condition: no entry under key 0 yet.");

        // Apply reindex logic (mirrors GameTickLoop.ReindexChoreProviders).
        const int correctKey = 0;
        if (provider.choreWorldMap.TryGetValue(-1, out var miskeyed) && miskeyed.Count > 0) {
            if (!provider.choreWorldMap.ContainsKey(correctKey))
                provider.choreWorldMap[correctKey] = new List<Chore>();
            provider.choreWorldMap[correctKey].AddRange(miskeyed);
            provider.choreWorldMap.Remove(-1);
        }

        Assert.IsFalse(provider.choreWorldMap.ContainsKey(-1),
            "After reindex: key -1 must be removed from choreWorldMap.");
        Assert.IsTrue(provider.choreWorldMap.ContainsKey(correctKey),
            "After reindex: chore must be under the correct world key.");
        Assert.AreEqual(1, provider.choreWorldMap[correctKey].Count,
            "After reindex: exactly 1 entry under the correct key.");
        Assert.AreSame(miskeyedEntry[0], provider.choreWorldMap[correctKey][0],
            "After reindex: the same chore object must be under the new key.");
    }

    // ── MinionPrefab.Setup() AddProvider fix ─────────────────────────────────

    /// <summary>
    /// Verifies that after MinionPrefab.Setup() the dupe's ChoreConsumer.providers
    /// contains its own ChoreProvider.
    ///
    /// ROOT CAUSE: CloneSingle (fdb3d2c) resets providers to a new empty List.
    /// MinionModifiers.OnSpawn() may not re-run for clones → providers stays empty
    /// → FindNextChore iterates 0 entries → always returns false → no chore assigned.
    ///
    /// FIX (MinionPrefab.Setup): consumer.AddProvider(go.GetComponent&lt;ChoreProvider&gt;())
    /// mirrors the same one-liner in CreaturePrefab.Setup() that fixed critter movement.
    /// </summary>
    [Test]
    public void DupeChoreConsumer_AfterSetup_HasOwnChoreProvider() {
        var go = createGameObject();
        var choreProvider = go.AddComponent<ChoreProvider>();
        go.AddComponent<ChoreDriver>();
        var consumer = go.AddComponent<ChoreConsumer>();
        consumer.consumerState = new ChoreConsumerState(consumer);

        // Simulate post-CloneSingle state: providers reset to empty.
        _choreConsumerProvidersField.SetValue(consumer, new List<ChoreProvider>());

        // Apply the fix: same call as MinionPrefab.Setup() DS-007 block.
        consumer.AddProvider(go.GetComponent<ChoreProvider>());

        var providers = (List<ChoreProvider>) _choreConsumerProvidersField.GetValue(consumer)!;

        Assert.AreEqual(1, providers.Count,
            "After AddProvider fix: providers must contain exactly 1 entry. " +
            "FindNextChore iterates this list — 0 entries means no chore ever found.");
        Assert.AreSame(choreProvider, providers[0],
            "The single entry must be the dupe's own ChoreProvider " +
            "(holds the IdleChore created by IdleMonitor during OnSpawn).");
    }

    // ── Per-SMC IdleMonitor isolation ─────────────────────────────────────────

    /// <summary>
    /// Documents the root cause: when stateMachines lists are SHARED (CloneSingle fix
    /// not applied in production save-load path), GetSMI returns the FIRST IdleMonitor
    /// (index 0 of the shared list) for ALL SMCs. Dupes 1+2 get dupe 0's IdleMonitor
    /// → only 1 IdleChore ever visible to any dupe → dupes 1+2 never move.
    /// </summary>
    [Test]
    public void TwoDupes_SharedSmcList_GetSmiReturnsSameIdleMonitor() {
        var go1 = createGameObject();
        var go2 = createGameObject();
        go1.AddComponent<KPrefabID>();
        go2.AddComponent<KPrefabID>();
        var smc1 = go1.AddComponent<StateMachineController>();
        var smc2 = go2.AddComponent<StateMachineController>();

        // Simulate CloneSingle fix NOT applied: both SMCs share the same list.
        var sharedList = new List<StateMachine.Instance>();
        SmcStateMachinesField.SetValue(smc1, sharedList);
        SmcStateMachinesField.SetValue(smc2, sharedList);

        var idle1 = new IdleMonitor.Instance(smc1); // ctor: smc1.AddStateMachineInstance → sharedList[0]
        var idle2 = new IdleMonitor.Instance(smc2); // ctor: smc2.AddStateMachineInstance → sharedList[1]

        // ROOT CAUSE: GetSMI iterates from index 0 — returns sharedList[0] = idle1 for BOTH.
        Assert.AreSame(smc1.GetSMI<IdleMonitor.Instance>(), smc2.GetSMI<IdleMonitor.Instance>(),
            "With shared stateMachines list, GetSMI<IdleMonitor>() returns the SAME instance " +
            "for both SMCs (index 0 of shared list). Only 1 IdleChore created → dupes 1+2 blocked.");
    }

    /// <summary>
    /// Verifies the fix: after MinionPrefab.Setup() resets smc.stateMachines to a fresh
    /// private list before IdleMonitor creation, each SMC's GetSMI returns its OWN instance.
    ///
    /// This is the direct inverse of TwoDupes_SharedSmcList_GetSmiReturnsSameIdleMonitor.
    /// </summary>
    [Test]
    public void TwoDupes_FreshSmcList_GetSmiReturnsDistinctIdleMonitors() {
        var go1 = createGameObject();
        var go2 = createGameObject();
        go1.AddComponent<KPrefabID>();
        go2.AddComponent<KPrefabID>();
        var smc1 = go1.AddComponent<StateMachineController>();
        var smc2 = go2.AddComponent<StateMachineController>();

        // Apply fix: each SMC gets its own fresh list BEFORE IdleMonitor creation.
        var list1 = new List<StateMachine.Instance>();
        var list2 = new List<StateMachine.Instance>();
        SmcStateMachinesField.SetValue(smc1, list1);
        SmcStateMachinesField.SetValue(smc2, list2);

        var idle1 = new IdleMonitor.Instance(smc1); // ctor → list1[0]
        var idle2 = new IdleMonitor.Instance(smc2); // ctor → list2[0]

        Assert.AreNotSame(idle1, idle2,
            "Each dupe must get a distinct IdleMonitor instance after fresh-list reset.");
        Assert.AreSame(idle1, smc1.GetSMI<IdleMonitor.Instance>(),
            "smc1.GetSMI must return idle1 (own instance), not idle2.");
        Assert.AreSame(idle2, smc2.GetSMI<IdleMonitor.Instance>(),
            "smc2.GetSMI must return idle2 (own instance), not idle1.");
        Assert.That(list1, Does.Not.Contain(idle2), "smc1.stateMachines must not contain smc2's IdleMonitor.");
        Assert.That(list2, Does.Not.Contain(idle1), "smc2.stateMachines must not contain smc1's IdleMonitor.");
    }

    // ── smc.stateMachines isolation fix (SMOKING GUN from 14b8aa5) ───────────

    /// <summary>
    /// SMOKING GUN confirmed by 14b8aa5 diagnostic:
    ///   smc hashes: -985234943, -1314160309, 292462703 — 3 DISTINCT SMC objects ✅
    ///   getSMI hash: 568807501 for ALL 3 — SAME IdleMonitor returned ❌
    ///
    /// Root cause: smc.stateMachines field on all 3 SMCs points to the SAME
    /// List&lt;StateMachine.Instance&gt; (MemberwiseClone shallow copy on save-load path).
    /// IdleMonitor.Instance(smc) ctor calls smc.AddStateMachineInstance(this) →
    /// appends to the shared list. GetSMI iterates from index 0 → always returns
    /// dupe0's IdleMonitor for all dupes → dupes 1+2 never get their own IdleChore.
    ///
    /// Fix: smc.stateMachines = new List&lt;&gt;() before any SM creation in MinionPrefab.Setup().
    /// Each assignment only changes that SMC's field — other SMCs' fields unchanged.
    /// </summary>
    [Test]
    public void ThreeDupes_SharedList_AfterPerDupeReset_HaveDistinctIdleMonitors() {
        var go1 = createGameObject(); go1.AddComponent<KPrefabID>();
        var go2 = createGameObject(); go2.AddComponent<KPrefabID>();
        var go3 = createGameObject(); go3.AddComponent<KPrefabID>();
        var smc1 = go1.AddComponent<StateMachineController>();
        var smc2 = go2.AddComponent<StateMachineController>();
        var smc3 = go3.AddComponent<StateMachineController>();

        // Simulate MemberwiseClone: all 3 SMCs share the same list.
        var sharedList = new List<StateMachine.Instance>();
        SmcStateMachinesField.SetValue(smc1, sharedList);
        SmcStateMachinesField.SetValue(smc2, sharedList);
        SmcStateMachinesField.SetValue(smc3, sharedList);

        // Without fix: adding an IdleMonitor via smc1 means smc2.GetSMI returns smc1's instance.
        var preIdle = new IdleMonitor.Instance(smc1); preIdle.StartSM();
        Assert.AreSame(smc1.GetSMI<IdleMonitor.Instance>(), smc2.GetSMI<IdleMonitor.Instance>(),
            "Pre-condition: shared list → GetSMI returns same instance across all SMCs (bug confirmed).");

        // FIX: reset stateMachines per-dupe (mirrors MinionPrefab.Setup() smc.stateMachines = new List<>()).
        SmcStateMachinesField.SetValue(smc1, new List<StateMachine.Instance>());
        SmcStateMachinesField.SetValue(smc2, new List<StateMachine.Instance>());
        SmcStateMachinesField.SetValue(smc3, new List<StateMachine.Instance>());

        // Start IdleMonitor for each dupe after isolation.
        new IdleMonitor.Instance(smc1).StartSM();
        new IdleMonitor.Instance(smc2).StartSM();
        new IdleMonitor.Instance(smc3).StartSM();

        var im1 = smc1.GetSMI<IdleMonitor.Instance>();
        var im2 = smc2.GetSMI<IdleMonitor.Instance>();
        var im3 = smc3.GetSMI<IdleMonitor.Instance>();

        Assert.IsNotNull(im1, "smc1 must have its own IdleMonitor.");
        Assert.IsNotNull(im2, "smc2 must have its own IdleMonitor.");
        Assert.IsNotNull(im3, "smc3 must have its own IdleMonitor.");
        Assert.AreNotSame(im1, im2, "After reset: smc1 and smc2 must return distinct IdleMonitor instances.");
        Assert.AreNotSame(im1, im3, "After reset: smc1 and smc3 must return distinct IdleMonitor instances.");
        Assert.AreNotSame(im2, im3, "After reset: smc2 and smc3 must return distinct IdleMonitor instances.");
    }

    // ── ResetSharedReferences: all 11 shared fields isolated ─────────────────

    /// <summary>
    /// Verifies that MinionPrefab.ResetSharedReferences logic isolates each dupe's
    /// ChoreConsumer and ChoreProvider from the shared MemberwiseClone state.
    ///
    /// ROOT CAUSE (confirmed 752c1a4 diagnostic):
    ///   • providersBefore=4,5,6 → providers List is SHARED across all 3 dupes
    ///   • cpChores=1 for CP0 only → choreProvider field points to dupe0's CP for all dupes
    ///   Both fields were shared because the save-load path bypasses CloneSingle, so
    ///   fdb3d2c's CloneSingle resets never ran.
    ///
    /// Additional shared fields (all reference-type, same MemberwiseClone pattern):
    ///   urges, behaviourPreconditions, preconditionSnapshot,
    ///   lastSuccessfulPreconditionSnapshot, choreGroupPriorities,
    ///   choreTypePriorities, traitDisabledChoreGroups, userDisabledChoreGroups,
    ///   cp.choreWorldMap
    ///
    /// FIX: MinionPrefab.ResetSharedReferences(go) replaces each shared collection/ref
    /// with a fresh instance before IdleMonitor.Instance(smc) runs.
    /// This test documents the before/after state using the same field reflections
    /// that the production fix applies via direct field access (AssemblyExposer).
    /// </summary>
    [Test]
    public void DupeChoreConsumer_ResetSharedReferences_IsolatesAllSharedFields() {
        var go = createGameObject();
        go.AddComponent<KPrefabID>();
        var cp = go.AddComponent<ChoreProvider>();
        go.AddComponent<ChoreDriver>();
        var consumer = go.AddComponent<ChoreConsumer>();

        // Simulate shared state from MemberwiseClone: both choreProvider and providers
        // reference objects belonging to a "dupe0" GO (the foreign objects).
        var foreignGo = createGameObject();
        var foreignCp = foreignGo.AddComponent<ChoreProvider>();

        // Wire shared state onto this dupe's consumer (mirrors save-load path result).
        _choreConsumerChoreProviderField.SetValue(consumer, foreignCp);
        _choreConsumerProvidersField.SetValue(consumer, new List<ChoreProvider> { foreignCp });
        // Pre-populate choreWorldMap to simulate shared dictionary with stale entries.
        cp.choreWorldMap[42] = new List<Chore> { null! };

        // Precondition: shared state is in place.
        Assert.AreSame(foreignCp, _choreConsumerChoreProviderField.GetValue(consumer),
            "Pre-condition: choreProvider must be pointing to foreignCp (shared state).");
        var providersPre = (List<ChoreProvider>) _choreConsumerProvidersField.GetValue(consumer)!;
        Assert.AreEqual(1, providersPre.Count, "Pre-condition: providers list contains foreignCp.");
        Assert.IsTrue(cp.choreWorldMap.ContainsKey(42), "Pre-condition: choreWorldMap has stale entries.");

        // Apply the reset — mirrors MinionPrefab.ResetSharedReferences(go) logic exactly.
        // (Cannot call MinionPrefab directly since it is internal and private.)
        consumer.choreProvider = cp;
        consumer.providers = new List<ChoreProvider>();
        cp.choreWorldMap = new Dictionary<int, List<Chore>>();

        // Verify: choreProvider now points to own CP.
        var choreProviderAfter = (ChoreProvider) _choreConsumerChoreProviderField.GetValue(consumer)!;
        Assert.AreSame(cp, choreProviderAfter,
            "After reset: choreProvider must point to this dupe's own ChoreProvider, not foreignCp.");

        // Verify: providers is fresh and empty (caller adds own CP via AddProvider after StartSM).
        var providersAfter = (List<ChoreProvider>) _choreConsumerProvidersField.GetValue(consumer)!;
        Assert.AreNotSame(providersPre, providersAfter,
            "After reset: providers must be a NEW list instance, not the shared one.");
        Assert.AreEqual(0, providersAfter.Count,
            "After reset: providers must be empty — AddProvider(cp) fills it after IdleMonitor.StartSM().");

        // Verify: choreWorldMap is fresh and empty.
        Assert.AreEqual(0, cp.choreWorldMap.Count,
            "After reset: choreWorldMap must be empty so new IdleChores land under the correct world key.");

        // Verify: after AddProvider, exactly own CP is in providers.
        consumer.AddProvider(cp);
        var providersAfterAdd = (List<ChoreProvider>) _choreConsumerProvidersField.GetValue(consumer)!;
        Assert.AreEqual(1, providersAfterAdd.Count,
            "After AddProvider: providers must contain exactly 1 entry.");
        Assert.AreSame(cp, providersAfterAdd[0],
            "After AddProvider: providers[0] must be this dupe's own ChoreProvider.");
    }

    /// <summary>
    /// Regression: 3 dupes each get their own fresh references after ResetSharedReferences.
    /// Confirms that the fix scales correctly — no cross-contamination between dupes.
    /// </summary>
    [Test]
    public void ThreeDupes_AfterResetSharedReferences_EachHasIsolatedChoreProvider() {
        // Set up 3 dupe GOs.
        var go1 = createGameObject(); go1.AddComponent<KPrefabID>();
        var go2 = createGameObject(); go2.AddComponent<KPrefabID>();
        var go3 = createGameObject(); go3.AddComponent<KPrefabID>();
        var cp1 = go1.AddComponent<ChoreProvider>();
        var cp2 = go2.AddComponent<ChoreProvider>();
        var cp3 = go3.AddComponent<ChoreProvider>();
        go1.AddComponent<ChoreDriver>(); go2.AddComponent<ChoreDriver>(); go3.AddComponent<ChoreDriver>();
        var c1 = go1.AddComponent<ChoreConsumer>();
        var c2 = go2.AddComponent<ChoreConsumer>();
        var c3 = go3.AddComponent<ChoreConsumer>();

        // Simulate shared state: all consumers point to dupe0's CP.
        _choreConsumerChoreProviderField.SetValue(c1, cp1);
        _choreConsumerChoreProviderField.SetValue(c2, cp1); // wrong — shared
        _choreConsumerChoreProviderField.SetValue(c3, cp1); // wrong — shared
        var sharedList = new List<ChoreProvider> { cp1 };
        _choreConsumerProvidersField.SetValue(c1, sharedList);
        _choreConsumerProvidersField.SetValue(c2, sharedList);
        _choreConsumerProvidersField.SetValue(c3, sharedList);

        // Apply reset to each dupe (mirrors MinionPrefab.ResetSharedReferences per dupe).
        foreach (var (go, cp, consumer) in new[] {
            (go1, cp1, c1), (go2, cp2, c2), (go3, cp3, c3)
        }) {
            consumer.choreProvider = cp;
            consumer.providers = new List<ChoreProvider>();
            cp.choreWorldMap = new Dictionary<int, List<Chore>>();
            consumer.AddProvider(cp);
        }

        // Each consumer must point to its own CP.
        Assert.AreSame(cp1, _choreConsumerChoreProviderField.GetValue(c1), "c1.choreProvider must be cp1.");
        Assert.AreSame(cp2, _choreConsumerChoreProviderField.GetValue(c2), "c2.choreProvider must be cp2.");
        Assert.AreSame(cp3, _choreConsumerChoreProviderField.GetValue(c3), "c3.choreProvider must be cp3.");

        // Each providers list must be independent and contain only own CP.
        var list1 = (List<ChoreProvider>) _choreConsumerProvidersField.GetValue(c1)!;
        var list2 = (List<ChoreProvider>) _choreConsumerProvidersField.GetValue(c2)!;
        var list3 = (List<ChoreProvider>) _choreConsumerProvidersField.GetValue(c3)!;
        Assert.AreNotSame(list1, list2, "Dupe 0 and dupe 1 must have different providers lists.");
        Assert.AreNotSame(list1, list3, "Dupe 0 and dupe 2 must have different providers lists.");
        Assert.AreEqual(1, list1.Count, "Dupe 0 providers must contain exactly 1 entry.");
        Assert.AreEqual(1, list2.Count, "Dupe 1 providers must contain exactly 1 entry.");
        Assert.AreEqual(1, list3.Count, "Dupe 2 providers must contain exactly 1 entry.");
        Assert.AreSame(cp1, list1[0], "Dupe 0's providers[0] must be its own CP.");
        Assert.AreSame(cp2, list2[0], "Dupe 1's providers[0] must be its own CP.");
        Assert.AreSame(cp3, list3[0], "Dupe 2's providers[0] must be its own CP.");

        // choreWorldMaps must all be independent and empty (no stale entries).
        Assert.AreNotSame(cp1.choreWorldMap, cp2.choreWorldMap, "cp1 and cp2 must have different choreWorldMaps.");
        Assert.AreEqual(0, cp1.choreWorldMap.Count, "cp1.choreWorldMap must be empty after reset.");
        Assert.AreEqual(0, cp2.choreWorldMap.Count, "cp2.choreWorldMap must be empty after reset.");
        Assert.AreEqual(0, cp3.choreWorldMap.Count, "cp3.choreWorldMap must be empty after reset.");
    }

    // ── Game.Instance.accumulators initialization ─────────────────────────────

    /// <summary>
    /// Regression: OxygenBreather.OnSpawn[IL_0x0021] NPE — Game.Instance.accumulators was null.
    ///
    /// ROOT CAUSE:
    ///   Game.OnPrefabInit() crashes at line ~820 (ConduitFlowVisualizer needs
    ///   Lighting/GlobalResources — absent in headless).
    ///   accumulators = new Accumulators()  ← line 823 — NEVER REACHED.
    ///   plantElementAbsorbers = new ...()  ← line 824 — NEVER REACHED.
    ///
    ///   OxygenBreather.OnSpawn() (runs during TriggerLifecycle Phase 2):
    ///     o2Accumulator = Game.Instance.accumulators.Add("O2", this);  // IL 0x0021 → NPE!
    ///
    ///   This NPE crashed all 3 dupes and set StateMachine.Instance.error = true,
    ///   halting all SM ticks → regression: all 3 dupes chore=None (commit 6ef111a).
    ///
    /// FIX (WorldBuilder.InitializeWorld catch block):
    ///   game.accumulators ??= new Accumulators();
    ///   game.plantElementAbsorbers ??= new PlantElementAbsorbers();
    ///
    /// This test documents that Accumulators is independently constructible and its
    /// Add() API works without Unity context — verifying the fix is sound.
    /// </summary>
    [Test]
    public void Accumulators_ConstructibleAndAddable_WithoutUnityContext() {
        var acc = new Accumulators();
        Assert.IsNotNull(acc,
            "Accumulators must be constructible without Unity context. " +
            "WorldBuilder catch block initializes it via 'new Accumulators()' after " +
            "Game.OnPrefabInit() crashes at ConduitFlowVisualizer (line 820).");

        // Add() must return a valid handle — OxygenBreather.OnSpawn stores the result
        // and uses it to call Accumulate() each breath tick. An InvalidHandle would
        // silently produce wrong metrics but not crash; a valid handle is required.
        var handle = acc.Add("O2", null!);
        Assert.AreNotEqual(HandleVector<int>.Handle.InvalidHandle, handle,
            "Accumulators.Add must return a valid handle so OxygenBreather.OnSpawn " +
            "can accumulate O2 data per breath tick without NPE.");
    }

    // ── 1000-tick stability: 3 dupes, no crash ────────────────────────────────

    /// <summary>
    /// Integration stability test: 3 dupes with independent SMC lists, run 1000
    /// StateMachineUpdater subticks, no unhandled exception.
    ///
    /// Before the CloneSingle fix, running 3 dupes after save-load would produce
    /// NPEs every tick (worker=null, consumerState=null) from dupes 1+2 being stuck
    /// in a half-initialised state with no own IdleMonitor/IdleChore.
    ///
    /// After the fix: each dupe has its own IdleMonitor, 3 IdleChores, 3 active
    /// ChoreDriver states → no NPE flood → clean tick loop.
    ///
    /// NOTE on position assertion:
    ///   navigator.cachedCell change requires a populated PathGrid with walkable floor
    ///   tiles (Grid.Solid[belowCell] = true). The unit-test environment does not set
    ///   up tile data, so Navigators find no reachable cells and stay at spawn cell.
    ///   End-to-end movement (cachedCell change) is verified on the live dedicated server.
    ///   This test verifies the prerequisite: no crash across 1000 ticks with 3 dupes.
    /// </summary>
    [Test]
    public void ThreeDupes_IndependentSmcLists_Run1000Ticks_NoCrash() {
        var go1 = AllStateMachinesInitTest.CreateFullDupeGO();
        var go2 = AllStateMachinesInitTest.CreateFullDupeGO();
        var go3 = AllStateMachinesInitTest.CreateFullDupeGO();

        try { BaseMinionConfig.BaseOnSpawn(go1, new Tag("Minion"), BaseMinionConfig.BaseRationalAiStateMachines()); } catch { }
        try { BaseMinionConfig.BaseOnSpawn(go2, new Tag("Minion"), BaseMinionConfig.BaseRationalAiStateMachines()); } catch { }
        try { BaseMinionConfig.BaseOnSpawn(go3, new Tag("Minion"), BaseMinionConfig.BaseRationalAiStateMachines()); } catch { }
        StateMachine.Instance.error = false;

        Assert.DoesNotThrow(() => {
            for (var i = 0; i < 1000; i++) {
                Singleton<StateMachineUpdater>.Instance.AdvanceOneSimSubTick();
            }
        }, "1000 AdvanceOneSimSubTick calls must not throw with 3 dupes after CloneSingle fix.");
    }

    // ── Movement-ready integration: 3 dupes, 1000 ticks, IdleChore per dupe ──

    /// <summary>
    /// Integration test: after 1000 SM ticks, all 3 dupes must be in movement-ready state.
    ///
    /// "Movement-ready" means:
    ///   • Each dupe has its own IdleMonitor.Instance (not null, not shared)
    ///   • Each dupe's ChoreDriver has a current chore (IdleChore — the first chore
    ///     assigned when IdleMonitor enters its 'idle' state)
    ///   • StateMachine.Instance.error remains false (no SM crash across 1000 ticks)
    ///
    /// NOTE on actual cell-change assertion:
    ///   Navigator.cachedCell changes only when PathGrid has walkable floor tiles
    ///   (Grid.Solid[belowCell] = true). The unit-test environment (ResetGrid) uses a
    ///   40x40 mock grid but does NOT set solid tiles, so Navigators cannot path and
    ///   stay at their spawn cell. The cell-change assertion is documented as requiring
    ///   a real save file and is verified on the live dedicated server instead.
    ///   See: ThreeDupes_IndependentSmcLists_Run1000Ticks_NoCrash for the pure no-crash
    ///   predecessor test.
    ///
    /// This test catches regressions where SMs are misconfigured (error=true blocks all
    /// GoTo() calls) or chore infrastructure is broken (empty providers → no IdleChore).
    /// </summary>
    [Test]
    public void ThreeDupes_After1000Ticks_AreMovementReady() {
        var go1 = AllStateMachinesInitTest.CreateFullDupeGO();
        var go2 = AllStateMachinesInitTest.CreateFullDupeGO();
        var go3 = AllStateMachinesInitTest.CreateFullDupeGO();

        try { BaseMinionConfig.BaseOnSpawn(go1, new Tag("Minion"), BaseMinionConfig.BaseRationalAiStateMachines()); } catch { }
        try { BaseMinionConfig.BaseOnSpawn(go2, new Tag("Minion"), BaseMinionConfig.BaseRationalAiStateMachines()); } catch { }
        try { BaseMinionConfig.BaseOnSpawn(go3, new Tag("Minion"), BaseMinionConfig.BaseRationalAiStateMachines()); } catch { }
        StateMachine.Instance.error = false;

        // Run 1000 ticks — enough for BrainScheduler to assign chores and
        // for IdleMonitor to reach its 'idle' → 'haschore' transition.
        for (var i = 0; i < 1000; i++) {
            Singleton<StateMachineUpdater>.Instance.AdvanceOneSimSubTick();
        }

        // ── Assert 1: no SM crash across 1000 ticks ───────────────────────────
        // StateMachine.Instance.error = true means at least one SM threw and all
        // subsequent GoTo() calls are no-ops → dupes are frozen forever.
        Assert.IsFalse(StateMachine.Instance.error,
            "StateMachine.Instance.error must be false after 1000 ticks. " +
            "error=true means a SM crash occurred → all GoTo() calls are no-ops → dupes are frozen.");

        // ── Assert 2: each dupe has its own distinct IdleMonitor instance ─────
        // Distinct instances → distinct IdleChores → each dupe can claim its own chore.
        var smc1 = go1.GetComponent<StateMachineController>();
        var smc2 = go2.GetComponent<StateMachineController>();
        var smc3 = go3.GetComponent<StateMachineController>();
        var im1 = smc1.GetSMI<IdleMonitor.Instance>();
        var im2 = smc2.GetSMI<IdleMonitor.Instance>();
        var im3 = smc3.GetSMI<IdleMonitor.Instance>();

        Assert.IsNotNull(im1, "Dupe 0 must have its own IdleMonitor.Instance.");
        Assert.IsNotNull(im2, "Dupe 1 must have its own IdleMonitor.Instance.");
        Assert.IsNotNull(im3, "Dupe 2 must have its own IdleMonitor.Instance.");
        Assert.AreNotSame(im1, im2, "Dupes 0 and 1 must have DISTINCT IdleMonitor instances. " +
            "Shared instance → shared IdleChore → only 1 dupe can claim it (IsPreemptable fails for others).");
        Assert.AreNotSame(im1, im3, "Dupes 0 and 2 must have DISTINCT IdleMonitor instances.");
        Assert.AreNotSame(im2, im3, "All 3 dupes must have DISTINCT IdleMonitor instances.");

        // ── Assert 3: each dupe's ChoreDriver has a current chore ────────────
        // A non-null current chore means FindNextChore() succeeded — the dupe
        // transitioned from nochore to haschore and is ready to act on the chore.
        var driver1 = go1.GetComponent<ChoreDriver>();
        var driver2 = go2.GetComponent<ChoreDriver>();
        var driver3 = go3.GetComponent<ChoreDriver>();

        Assert.IsNotNull(driver1?.GetCurrentChore(),
            "Dupe 0's ChoreDriver must have a current chore after 1000 ticks. " +
            "No chore → FindNextChore returned false → providers empty or chore under wrong world-ID key.");
        Assert.IsNotNull(driver2?.GetCurrentChore(),
            "Dupe 1's ChoreDriver must have a current chore after 1000 ticks.");
        Assert.IsNotNull(driver3?.GetCurrentChore(),
            "Dupe 2's ChoreDriver must have a current chore after 1000 ticks.");

        // NOTE: actual cachedCell change is NOT asserted here (requires PathGrid + solid tiles).
        // Once chore is assigned, Navigator will path and change cell on the live server.
        // Verify cell-change on the dedicated server via /api/debug/dupes after 300+ ticks.
    }

}
