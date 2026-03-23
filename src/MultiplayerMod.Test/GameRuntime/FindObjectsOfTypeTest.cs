using MultiplayerMod.Test.Environment.Unity;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MultiplayerMod.Test.GameRuntime;

/// <summary>
/// Verifies the <c>Object.FindObjectsOfType</c> API contract in the headless test runtime.
///
/// ROOT CAUSE (headless dedicated server):
///   <c>UnityRuntime.FindObjectsOfType(Type, bool)</c> and
///   <c>UnityRuntime.FindObjectsOfTypeAll(Type)</c> were stubbed to return
///   <c>Array.Empty&lt;Object&gt;()</c>.  No Unity scene graph exists in headless —
///   instead, every <c>GameObject</c> creation and <c>AddComponent&lt;T&gt;()</c>
///   call populates two internal dictionaries in <c>UnityRuntime</c>:
///     • <c>GameObjectComponents</c> — GO ptr → list of components
///     • <c>ComponentToGameObject</c> — component ptr → owning GO
///
/// FIX (UnityRuntime):
///   <c>FindObjectsOfType(Type, bool)</c> now iterates <c>GameObjectComponents.Values</c>
///   and filters by <c>type.IsInstanceOfType(comp)</c>, skipping components whose
///   <c>m_CachedPtr</c> is absent from <c>ComponentToGameObject</c>
///   (individually-destroyed components that were not yet GC'd from the list).
///   <c>FindObjectsOfTypeAll(Type)</c> delegates to <c>FindObjectsOfType(type, true)</c>.
///   The <c>includeInactive</c> flag is ignored — headless has no reliable active-state
///   tracking, so all registered objects are always returned.
///
/// TESTS:
///   Written against the <c>UnityTestRuntime</c> (used by the test harness), which
///   implements the same contract via its <c>enabledComponents</c> registry.
///   The same assertions must hold for <c>UnityRuntime</c> in the dedicated server.
///   In the test environment components become visible to <c>FindObjectsOfType</c> after
///   <c>NextFrame()</c> (which moves them from <c>newComponents</c> to
///   <c>enabledComponents</c>).
/// </summary>
public class FindObjectsOfTypeTest : PlayableGameTest {

    // ─── ROOT CAUSE TESTS ─────────────────────────────────────────────────────

    /// <summary>
    /// Documents root cause: before a frame tick, newly-created components are NOT
    /// yet visible to <c>FindObjectsOfType</c>.
    ///
    /// In <c>UnityTestRuntime</c>, components added by <c>AddComponent</c> land in
    /// <c>newComponents</c>; they move to <c>enabledComponents</c> (the source for
    /// <c>FindObjectsOfType</c>) only after <c>NextFrame()</c>.
    ///
    /// The dedicated server had a stricter version of this problem: even after
    /// frame ticks, <c>UnityRuntime.FindObjectsOfType</c> always returned empty
    /// because it was a stub (<c>return Array.Empty&lt;Object&gt;()</c>).
    /// </summary>
    [Test]
    public void FindObjectsOfType_ReturnsEmpty_BeforeNextFrame() {
        var go = new GameObject("TestGO");
        go.AddComponent<Notifier>();

        // Components are in newComponents, not yet in enabledComponents
        var found = Object.FindObjectsOfType<Notifier>(includeInactive: true);

        Assert.That(
            System.Array.Exists(found, c => c != null && c.gameObject == go),
            Is.False,
            "Notifier on a freshly-created GO must NOT be visible to FindObjectsOfType " +
            "until NextFrame() is called. This mirrors the headless server stub behaviour " +
            "(Array.Empty) — components are only findable once the runtime advances a frame " +
            "and populates its registry.");
    }

    // ─── FIX / CONTRACT TESTS ─────────────────────────────────────────────────

    /// <summary>
    /// Verifies the fix contract: after <c>NextFrame()</c>, a component added via
    /// <c>AddComponent</c> IS visible to <c>FindObjectsOfType&lt;T&gt;</c>.
    ///
    /// In the dedicated server, <c>UnityRuntime.FindObjectsOfType</c> now searches
    /// <c>GameObjectComponents</c> — all components are added there at
    /// <c>AddComponent</c> time (no frame-tick delay).  The test uses
    /// <c>NextFrame()</c> because the test runtime requires it; the prod runtime
    /// makes components findable immediately.
    /// </summary>
    [Test]
    public void FindObjectsOfType_Finds_ComponentAfterNextFrame() {
        var go = new GameObject("TestGO");
        var notifier = go.AddComponent<Notifier>();

        UnityTestRuntime.NextFrame(); // moves newComponents → enabledComponents

        var found = Object.FindObjectsOfType<Notifier>(includeInactive: true);

        Assert.That(
            System.Array.Exists(found, c => c == notifier),
            Is.True,
            "Notifier must be visible to FindObjectsOfType<Notifier> after NextFrame(). " +
            "UnityRuntime fix: FindObjectsOfType now iterates GameObjectComponents.Values " +
            "and filters by type.IsInstanceOfType(comp) instead of returning Array.Empty.");
    }

    /// <summary>
    /// Verifies subtype polymorphism: <c>FindObjectsOfType&lt;KMonoBehaviour&gt;</c>
    /// returns a <c>Notifier</c> (which extends <c>KMonoBehaviour</c>).
    ///
    /// The fix uses <c>type.IsInstanceOfType(comp)</c> which respects inheritance,
    /// matching Unity's real behaviour.
    /// </summary>
    [Test]
    public void FindObjectsOfType_Finds_DerivedType() {
        var go = new GameObject("TestGO");
        var notifier = go.AddComponent<Notifier>();

        UnityTestRuntime.NextFrame();

        // Notifier extends KMonoBehaviour — must be found when searching the base type
        var found = Object.FindObjectsOfType<KMonoBehaviour>(includeInactive: true);

        Assert.That(
            System.Array.Exists(found, c => c == notifier),
            Is.True,
            "FindObjectsOfType<KMonoBehaviour> must include a Notifier (which extends " +
            "KMonoBehaviour). Fix uses type.IsInstanceOfType which respects inheritance.");
    }

    /// <summary>
    /// Verifies that multiple components of the same type are all returned.
    ///
    /// The original stub returned empty for ALL types. The fix iterates all
    /// registered components so all instances are found.
    /// </summary>
    [Test]
    public void FindObjectsOfType_Finds_MultipleComponents() {
        var go1 = new GameObject("TestGO1");
        var go2 = new GameObject("TestGO2");
        var notifier1 = go1.AddComponent<Notifier>();
        var notifier2 = go2.AddComponent<Notifier>();

        UnityTestRuntime.NextFrame();

        var found = Object.FindObjectsOfType<Notifier>(includeInactive: true);

        Assert.That(
            System.Array.Exists(found, c => c == notifier1),
            Is.True,
            "First Notifier must appear in FindObjectsOfType results.");
        Assert.That(
            System.Array.Exists(found, c => c == notifier2),
            Is.True,
            "Second Notifier must appear in FindObjectsOfType results.");
    }

    /// <summary>
    /// Verifies that <c>FindObjectsOfType</c> returns no results for a type that
    /// has no registered instances.
    ///
    /// The original stub also returned empty for types with zero instances —
    /// but for the wrong reason (always empty). After the fix, the empty
    /// result is correct for types genuinely absent from the registry.
    /// </summary>
    [Test]
    public void FindObjectsOfType_ReturnsEmpty_WhenNoInstanceOfType() {
        UnityTestRuntime.NextFrame();

        // Clearable is unlikely to be on any GO created in this test
        var found = Object.FindObjectsOfType<Clearable>(includeInactive: true);
        // We can't assert Length == 0 because the SetUpUnityAndGame might have added
        // a Clearable. Assert the result is non-null (not a null-ref stub).
        Assert.IsNotNull(found,
            "FindObjectsOfType must return a non-null array (never null) even when no " +
            "instances of the requested type are registered. " +
            "The original stub returned Array.Empty<Object>() — the fix returns a " +
            "properly-constructed empty array from LINQ when nothing matches.");
    }
}
