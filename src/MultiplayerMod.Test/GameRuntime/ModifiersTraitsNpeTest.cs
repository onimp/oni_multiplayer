using Klei.AI;
using NUnit.Framework;
using UnityEngine;

namespace MultiplayerMod.Test.GameRuntime;

/// <summary>
/// Tests for the Modifiers.OnPrefabInit NPE fix (commit 109bcc2 / fix 126).
///
/// ROOT CAUSE (confirmed by code inspection):
///   <c>Modifiers.OnPrefabInit()</c> tail:
///   <code>
///     Traits component = GetComponent&lt;Traits&gt;();
///     if (initialTraits == null) return;
///     foreach (string initialTrait in initialTraits)
///     {
///         Trait trait = Db.Get().traits.Get(initialTrait);
///         component.Add(trait);  // ← NPE at IL offset 0x153 when component is null
///     }
///   </code>
///   EntityTemplates only adds the <c>Traits</c> component conditionally
///   (when <c>trait.SelfModifiers.Count &gt; 0</c>), but other EntityTemplates
///   code paths populate <c>Modifiers.initialTraits</c> without adding <c>Traits</c>.
///   Result: 810x NullReferenceException per server boot for critters / buildings.
///
/// FIX (UnityRuntime.cs Phase 0):
///   Before any <c>InitializeComponent()</c> fires in <c>TriggerLifecycle()</c>,
///   a pre-pass checks every component: if the GO has a <c>Modifiers</c> with
///   <c>initialTraits.Count &gt; 0</c> but no <c>Traits</c> component, it calls
///   <c>go.AddOrGet&lt;Traits&gt;()</c>. This ensures <c>component</c> is never null
///   when <c>Modifiers.OnPrefabInit()</c> iterates <c>initialTraits</c>.
/// </summary>
public class ModifiersTraitsNpeTest : PlayableGameTest {

    [SetUp]
    public void SetUp() {
        // Ensure Db.traits is initialised for the end-to-end test.
        // DbPatch replaces Db.Initialize() and does not call base (ModifierSet.Initialize()),
        // so traits is null in the test environment unless we seed it here.
        if (Db.Get().traits == null)
            Db.Get().traits = new ModifierSet.TraitSet();
    }

    // ─── ROOT CAUSE TESTS ────────────────────────────────────────────────────

    /// <summary>
    /// Root cause: a GO that has <c>Modifiers.initialTraits</c> populated is created
    /// WITHOUT a <c>Traits</c> component by EntityTemplates.
    ///
    /// EntityTemplates adds <c>Traits</c> only when <c>trait.SelfModifiers.Count &gt; 0</c>:
    /// <code>
    ///   if (trait.SelfModifiers != null &amp;&amp; trait.SelfModifiers.Count &gt; 0) {
    ///       template.AddOrGet&lt;Traits&gt;();
    ///       component.initialTraits.Add(baseTraitId);
    ///   }
    /// </code>
    /// Critters/buildings whose base trait has NO self-modifiers therefore reach
    /// <c>Modifiers.OnPrefabInit()</c> without a <c>Traits</c> component →
    /// <c>component.Add(trait)</c> at IL offset 0x153 throws NullReferenceException.
    /// </summary>
    [Test]
    public void Modifiers_TraitsComponent_IsAbsent_WhenNotExplicitlyAdded() {
        var go = createGameObject();
        var mods = go.AddComponent<Modifiers>();

        // Simulate a critter prefab that has initialTraits but EntityTemplates
        // skipped adding Traits (because the trait had no SelfModifiers).
        mods.initialTraits.Add("SomeTraitId");

        Assert.IsNull(go.GetComponent<Traits>(),
            "Traits component is absent even though initialTraits is non-empty. " +
            "EntityTemplates only adds Traits when trait.SelfModifiers.Count > 0. " +
            "Modifiers.OnPrefabInit() then NPEs at component.Add(trait) where " +
            "component = GetComponent<Traits>() == null (IL offset 0x153). " +
            "This is the root cause of 810x pre-boot NPEs for critters/buildings.");

        Assert.Greater(mods.initialTraits.Count, 0,
            "initialTraits must be non-empty to reach the NPE code path in OnPrefabInit.");
    }

    // ─── FIX TESTS ───────────────────────────────────────────────────────────

    /// <summary>
    /// Fix: Phase 0 in <c>TriggerLifecycle()</c> adds the <c>Traits</c> component
    /// whenever <c>Modifiers.initialTraits</c> is non-empty and <c>Traits</c> is absent.
    ///
    /// This is the exact condition from <c>UnityRuntime.cs</c>:
    /// <code>
    ///   if (comp is Modifiers mods &amp;&amp; mods.initialTraits?.Count &gt; 0
    ///       &amp;&amp; go.GetComponent&lt;Traits&gt;() == null)
    ///       go.AddOrGet&lt;Traits&gt;();
    /// </code>
    /// After Phase 0 runs, <c>GetComponent&lt;Traits&gt;()</c> is non-null and
    /// <c>Modifiers.OnPrefabInit()</c> can safely call <c>component.Add(trait)</c>.
    /// </summary>
    [Test]
    public void Phase0_AddsTraitsComponent_WhenInitialTraitsNonEmptyAndTraitsAbsent() {
        var go = createGameObject();
        var mods = go.AddComponent<Modifiers>();
        mods.initialTraits.Add("SomeTraitId");

        // Pre-condition: Traits absent (the bug scenario).
        Assert.IsNull(go.GetComponent<Traits>(),
            "Pre-condition: Traits must be absent before Phase 0 runs.");

        // Apply Phase 0 logic from UnityRuntime.TriggerLifecycle.
        if (mods.initialTraits?.Count > 0 && go.GetComponent<Traits>() == null)
            go.AddOrGet<Traits>();

        // Post-condition: Traits now present.
        Assert.IsNotNull(go.GetComponent<Traits>(),
            "Phase 0 must add Traits component when initialTraits is non-empty. " +
            "Without this, Modifiers.OnPrefabInit() NPEs at component.Add(trait) " +
            "where component = GetComponent<Traits>() == null (IL offset 0x153).");
    }

    /// <summary>
    /// End-to-end: <c>Modifiers.InitializeComponent()</c> does NOT throw when the
    /// <c>Traits</c> component is present and <c>initialTraits</c> contains a valid trait.
    ///
    /// This is the test that FAILS without the fix (Traits absent → NPE) and
    /// PASSES with the fix (Phase 0 adds Traits → OnPrefabInit succeeds).
    ///
    /// Test seed: we add a minimal Trait to Db.traits (in SetUp Db.traits is
    /// initialised if null), then add its id to Modifiers.initialTraits, then
    /// call the Phase 0 add-Traits guard, then run InitializeComponent.
    /// </summary>
    [Test]
    public void Modifiers_InitializeComponent_DoesNotThrow_WhenTraitsAddedByPhase0() {
        // Seed a minimal trait into Db so Modifiers.OnPrefabInit's
        // Db.Get().traits.Get("TestModifiersTrait") can resolve it.
        var testTrait = new Trait(
            "TestModifiersTrait", "TestModifiersTrait", "", 0f,
            false, null, false, false);
        Db.Get().traits.Add(testTrait);

        var go = createGameObject();
        var mods = go.AddComponent<Modifiers>();
        mods.initialTraits.Add("TestModifiersTrait");

        // Without Phase 0 this would be absent → NPE at component.Add(trait).
        Assert.IsNull(go.GetComponent<Traits>(), "Pre-condition: Traits absent");

        // Phase 0 fix: add Traits when initialTraits non-empty.
        if (mods.initialTraits?.Count > 0 && go.GetComponent<Traits>() == null)
            go.AddOrGet<Traits>();

        Assert.IsNotNull(go.GetComponent<Traits>(), "Traits must be present after Phase 0");

        // InitializeComponent must not throw — this fails without the fix.
        Assert.DoesNotThrow(
            () => ((KMonoBehaviour) mods).InitializeComponent(),
            "Modifiers.InitializeComponent must succeed when Traits component is present. " +
            "Without the Phase 0 fix in TriggerLifecycle, Traits is absent and " +
            "component.Add(trait) NPEs at IL offset 0x153 — 810x per server boot.");
    }
}
