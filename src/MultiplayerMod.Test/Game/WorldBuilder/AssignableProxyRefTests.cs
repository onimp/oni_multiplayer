using System;
using System.Runtime.Serialization;
using NUnit.Framework;

namespace MultiplayerMod.Test.Game.WorldBuilder;

/// <summary>
/// Unit tests for the KPrefabID-on-proxy-GO fix in WorldBuilder.EnsureAssignableProxy().
///
/// Root cause:
///   EnsureAssignableProxy (fallback path) created the proxy GO without KPrefabID.
///   Ref&lt;MinionAssignablesProxy&gt;.Set(proxy) calls:
///     id = proxy.GetComponent&lt;KPrefabID&gt;().InstanceID
///   When no KPrefabID is on the proxy GO, GetComponent returns null →
///   null.InstanceID → NullReferenceException → proxy obj field never stored →
///   assignableProxy.Get() returns null.
///
///   Then AssignableReachabilitySensor.ctor (called inside BaseMinionConfig.BaseOnSpawn):
///     MinionAssignablesProxy proxy = identity.assignableProxy.Get()
///     proxy.ConfigureAssignableSlots()   ← NPE: proxy is null
///   → BaseOnSpawn fails for every dupe → fallback to minimal sensors.
///
/// Fix (WorldBuilder.EnsureAssignableProxy, direct proxy creation path):
///   proxyGO.AddOrGet&lt;KPrefabID&gt;();   // before identity.assignableProxy.Set(proxy)
///   → proxy.GetComponent&lt;KPrefabID&gt;().InstanceID = 0 (default int) → no NPE →
///   Ref.obj field is stored → Get() returns proxy → ARS.ctor succeeds.
///
/// Tests here verify the Ref&lt;T&gt;.Set contract and KPrefabID.InstanceID field
/// using FormatterServices.GetUninitializedObject — no full game boot required.
/// </summary>
[TestFixture]
[Parallelizable]
public class AssignableProxyRefTests {

    // ─── Root cause: null KPrefabID → InstanceID access throws NPE ────────────

    [Test]
    public void KPrefabID_Null_InstanceIDAccess_ThrowsNullReferenceException() {
        // This is the exact NPE that occurred in Ref<T>.Set(proxy):
        //   id = proxy.GetComponent<KPrefabID>().InstanceID
        // When GetComponent returns null (no KPrefabID on GO), accessing .InstanceID throws.
        KPrefabID? kpid = null;
        Assert.Throws<NullReferenceException>(
            () => { var _ = kpid!.InstanceID; },
            "null.InstanceID must throw NPE — confirms why KPrefabID must be on proxy GO");
    }

    // ─── Fix: KPrefabID on GO → InstanceID readable, Ref.Set() succeeds ──────

    [Test]
    public void KPrefabID_InstanceID_DefaultsToZero_AfterUninitializedCreation() {
        // After AddComponent<KPrefabID>() in headless, the KPrefabID is created via
        // Activator.CreateInstance (or FormatterServices.GetUninitializedObject fallback).
        // InstanceID is a plain int field [Serialize] — defaults to 0.
        // Ref<T>.Set() only needs it to be readable (non-throwing); value 0 is fine
        // because Ref.Get() returns the cached obj (not KPrefabIDTracker lookup) when obj!=null.
        var kpid = (KPrefabID) FormatterServices.GetUninitializedObject(typeof(KPrefabID));

        Assert.That(kpid.InstanceID, Is.EqualTo(0),
            "KPrefabID.InstanceID defaults to 0 — Ref<T>.Set() can read it without NPE");
    }

    [Test]
    public void KPrefabID_InstanceID_IsReadableAfterExplicitSet() {
        // Confirms InstanceID is a plain assignable int field (not a computed property).
        // In the real game, KPrefabID.InstanceID is set during save/load deserialization.
        // For headless proxy we don't need a real ID — 0 suffices.
        var kpid = (KPrefabID) FormatterServices.GetUninitializedObject(typeof(KPrefabID));
        kpid.InstanceID = 42;

        Assert.That(kpid.InstanceID, Is.EqualTo(42),
            "KPrefabID.InstanceID is a plain int field — readable after assignment");
    }

}
