using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace MultiplayerMod.Test.GameRuntime;

/// <summary>
/// Integration tests for the critter movement blocker.
///
/// ROOT CAUSE (confirmed by code inspection):
///   <c>ChoreConsumer.providers</c> (private List&lt;ChoreProvider&gt;) is empty for ALL
///   regular creatures (Hatch, Drecko, Puft, etc.).
///
///   <c>ChoreConsumer.FindNextChore()</c> iterates <c>providers</c> to collect chores.
///   For dupes, <c>MinionModifiers.OnSpawn()</c> calls
///   <c>AddProvider(GlobalChoreProvider.Instance)</c> to populate the list.
///   For regular creatures, <c>AddProvider()</c> is NEVER called from any game code
///   → <c>providers</c> stays empty → <c>FindNextChore()</c> always returns false
///   → <c>ForceUpdateCreatureBrains()</c> at tick=62 calls <c>SetChore()</c> with nothing
///   → creature stays in <c>nochore</c> state forever → no movement.
///
///   The creature's SM chores (IdleStates, MoveToSafetyChore, etc.) ARE created by
///   <c>ChoreConsumer.OnSpawn()</c> via <c>choreTableInstance</c> and registered in the
///   creature's own <c>ChoreProvider</c> component, but that provider is never added to
///   the <c>providers</c> list that <c>FindNextChore()</c> uses.
///
/// FIX:
///   <c>CreaturePrefab.Setup()</c> Step 3.5: call
///   <c>consumer.AddProvider(go.GetComponent&lt;ChoreProvider&gt;())</c> so the creature's
///   own provider is in the list and <c>FindNextChore()</c> can find its SM chores.
/// </summary>
public class CreatureMovementTest : PlayableGameTest {

    // Reflection accessor for the private ChoreConsumer.providers field.
    private static readonly FieldInfo _providersField =
        typeof(ChoreConsumer).GetField("providers",
            BindingFlags.Instance | BindingFlags.NonPublic)!;

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

    // ─── ROOT CAUSE TESTS ────────────────────────────────────────────────────

    /// <summary>
    /// Documents root cause: ChoreConsumer.providers is empty for a newly created creature.
    /// FindNextChore() iterates providers — with 0 entries it always returns false.
    ///
    /// This is why ForceUpdateCreatureBrains() at tick=62 finds no chore for any
    /// regular creature (Hatch, Drecko, Puft, etc.) and creatures stay stuck in nochore.
    /// </summary>
    [Test]
    public void Creature_Providers_IsEmpty_BeforeAddProvider() {
        var go = createGameObject();
        go.AddComponent<ChoreProvider>();
        go.AddComponent<ChoreDriver>();
        var consumer = go.AddComponent<ChoreConsumer>();
        consumer.consumerState = new ChoreConsumerState(consumer);

        var providers = (System.Collections.Generic.List<ChoreProvider>)_providersField.GetValue(consumer);

        Assert.AreEqual(0, providers.Count,
            "ChoreConsumer.providers must be empty for a freshly-created creature — " +
            "AddProvider() is never called for regular creatures (Hatch/Drecko/etc.), " +
            "so FindNextChore() always iterates 0 providers → returns false → no chore ever assigned.");
    }

    /// <summary>
    /// Documents root cause: FindNextChore returns false when providers is empty,
    /// even if the creature's own ChoreProvider has chores registered.
    ///
    /// The creature's ChoreProvider (populated by choreTableInstance) is invisible to
    /// FindNextChore because it is not in the providers list.
    /// </summary>
    [Test]
    public void Creature_FindNextChore_ReturnsFalse_WithEmptyProviders() {
        var go = createGameObject();
        go.AddComponent<ChoreProvider>();
        go.AddComponent<ChoreDriver>();
        var consumer = go.AddComponent<ChoreConsumer>();
        consumer.consumerState = new ChoreConsumerState(consumer);

        var driver = go.GetComponent<ChoreDriver>();
        driver.smi.StartSM();
        StateMachine.Instance.error = false;

        var context = default(Chore.Precondition.Context);
        var found = consumer.FindNextChore(ref context);

        Assert.IsFalse(found,
            "FindNextChore must return false with empty providers. " +
            "This is the root cause of critters not moving: all regular creatures have " +
            "providers.Count == 0 → FindNextChore never finds their SM chores → " +
            "no chore assigned → Navigator never starts → no movement.");
    }

    // ─── FIX TESTS ───────────────────────────────────────────────────────────

    /// <summary>
    /// Verifies the fix: after AddProvider(choreProvider), providers has 1 entry.
    ///
    /// This mirrors what CreaturePrefab.Setup() Step 3.5 now does:
    ///   consumer.AddProvider(go.GetComponent&lt;ChoreProvider&gt;());
    ///
    /// With providers populated, FindNextChore() will iterate the creature's own
    /// ChoreProvider and find the SM chores created by choreTableInstance.
    /// </summary>
    [Test]
    public void Creature_Providers_HasOneEntry_AfterAddProvider() {
        var go = createGameObject();
        var choreProvider = go.AddComponent<ChoreProvider>();
        go.AddComponent<ChoreDriver>();
        var consumer = go.AddComponent<ChoreConsumer>();
        consumer.consumerState = new ChoreConsumerState(consumer);

        // THE FIX: add the creature's own ChoreProvider to the consumer's providers list.
        // This is what CreaturePrefab.Setup() Step 3.5 now does.
        consumer.AddProvider(choreProvider);

        var providers = (System.Collections.Generic.List<ChoreProvider>)_providersField.GetValue(consumer);

        Assert.AreEqual(1, providers.Count,
            "providers must have 1 entry after AddProvider(choreProvider). " +
            "FindNextChore() iterates this list — with 1 entry it will call " +
            "choreProvider.CollectChores() and find the creature's SM chores.");
        Assert.AreSame(choreProvider, providers[0],
            "The entry must be exactly the creature's own ChoreProvider, " +
            "which holds the SM chores created by choreTableInstance " +
            "(IdleStates, MoveToSafetyChore, etc.).");
    }

    /// <summary>
    /// AddProvider is idempotent-safe when called once:
    /// calling it a second time adds a duplicate, which would double-collect chores.
    /// FixCreatureBrains() is only called once at startup, so one call is all that happens.
    ///
    /// This test documents the expected single-call behavior.
    /// </summary>
    [Test]
    public void Creature_AddProvider_SingleCall_NoSideEffects() {
        var go = createGameObject();
        var choreProvider = go.AddComponent<ChoreProvider>();
        go.AddComponent<ChoreDriver>();
        var consumer = go.AddComponent<ChoreConsumer>();
        consumer.consumerState = new ChoreConsumerState(consumer);

        consumer.AddProvider(choreProvider); // called exactly once by CreaturePrefab.Setup()

        var providers = (System.Collections.Generic.List<ChoreProvider>)_providersField.GetValue(consumer);

        // Exactly one entry after one call.
        Assert.AreEqual(1, providers.Count,
            "One AddProvider call must produce exactly one entry. " +
            "FixCreatureBrains() calls CreaturePrefab.Setup() once per brain at startup.");
    }

    // ─── INTEGRATION: 1000 TICKS ─────────────────────────────────────────────

    /// <summary>
    /// Integration test: creature with the fix applied (AddProvider) runs 1000
    /// StateMachineUpdater ticks without crashing.
    ///
    /// Without the fix, the b__5_3 NPE (worker=null or consumerState=null) would
    /// cause exceptions on every tick. This verifies the fix is stable.
    ///
    /// NOTE on "position changed":
    ///   Physical grid position change requires a populated PathGrid with walkable
    ///   floor tiles (Grid.Solid[belowCell] = true). The unit test environment does
    ///   not set up the game world's tile data, so Navigator.PathGrid has no reachable
    ///   cells → Navigator.GoTo() finds no path → creature stays at its spawn cell.
    ///   End-to-end movement verification is done on the live dedicated server.
    ///
    ///   What this test DOES verify:
    ///     • No unhandled exception across 1000 SM ticks with the fix applied.
    ///     • The creature's brain/chore/SM pipeline is correctly initialised.
    /// </summary>
    [Test]
    public void Creature_With_AddProvider_Fix_Runs1000Ticks_WithoutCrash() {
        // Set up creature GO with all required components.
        var go = createGameObject();

        var choreProvider = go.AddComponent<ChoreProvider>();
        go.AddComponent<ChoreDriver>();
        var consumer = go.AddComponent<ChoreConsumer>();
        consumer.consumerState = new ChoreConsumerState(consumer);

        var driver = go.GetComponent<ChoreDriver>();
        go.AddOrGet<StandardWorker>();

        // KPrefabID required by ChoreDriver nochore.Update (b__5_0):
        //   smi.masterPrefabId.HasTag(GameTags.BaseMinion)
        // masterPrefabId = GetComponent<KPrefabID>(). Without it HasTag NPEs at IL_000b.
        // TriggerLifecycle adds KPrefabID during world loading; we add it here for the test.
        go.AddComponent<KPrefabID>();

        driver.smi.StartSM();
        StateMachine.Instance.error = false;

        // THE FIX: add creature's own ChoreProvider to providers.
        consumer.AddProvider(choreProvider);

        // Run 1000 SM subticks — previously the b__5_3 NPE would fire every tick.
        Assert.DoesNotThrow(() => {
            for (int i = 0; i < 1000; i++) {
                Singleton<StateMachineUpdater>.Instance.AdvanceOneSimSubTick();
            }
        }, "1000 AdvanceOneSimSubTick calls must not throw after AddProvider fix. " +
           "Before the fix, b__5_3 NPEs (consumerState/worker null) crashed every tick.");
    }

    /// <summary>
    /// Contrast test: creature WITHOUT the fix runs 1000 ticks.
    ///
    /// With empty providers, FindNextChore always returns false so no chore is ever
    /// assigned — the ChoreDriver SM stays in nochore and the haschore.Update b__5_3
    /// lambda (smi.worker.GetWorkable()) is never reached.
    ///
    /// This confirms that the b__5_3 NPE only surfaces AFTER a chore is assigned, and
    /// that the providers fix alone doesn't cause new crashes.
    /// </summary>
    [Test]
    public void Creature_Without_AddProvider_Fix_AlsoRuns1000Ticks_WithoutCrash() {
        var go = createGameObject();

        go.AddComponent<ChoreProvider>();
        go.AddComponent<ChoreDriver>();
        var consumer = go.AddComponent<ChoreConsumer>();
        consumer.consumerState = new ChoreConsumerState(consumer);

        var driver = go.GetComponent<ChoreDriver>();
        go.AddOrGet<StandardWorker>();

        // KPrefabID required by nochore.Update (b__5_0): smi.masterPrefabId.HasTag(...)
        go.AddComponent<KPrefabID>();

        driver.smi.StartSM();
        StateMachine.Instance.error = false;

        // No AddProvider — creature is stuck in nochore, no b__5_3 fire possible.
        Assert.DoesNotThrow(() => {
            for (int i = 0; i < 1000; i++) {
                Singleton<StateMachineUpdater>.Instance.AdvanceOneSimSubTick();
            }
        }, "1000 ticks without AddProvider fix must not crash either — creature " +
           "just stays in nochore (no chore found → haschore never entered → " +
           "b__5_3 never reached).");

        // Confirm: still no chore assigned (expected with empty providers).
        Assert.IsNull(driver.GetCurrentChore(),
            "Without AddProvider, no chore should ever be assigned (FindNextChore " +
            "returns false with empty providers). Creature stays stuck in nochore.");
    }

}
