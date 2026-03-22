using NUnit.Framework;
using UnityEngine;

namespace MultiplayerMod.Test.GameRuntime;

/// <summary>
/// Tests for the DS-005 proxy component fix:
/// AssignableReachabilitySensor.ctor calls proxy.GetComponents&lt;Assignables&gt;()
/// and iterates the result. The proxy GO must have Ownables and Equipment
/// (both : Assignables) or the sensor ctor NPEs at [0x00028].
///
/// WorldBuilder.FixRationalAi adds Ownables+Equipment to the proxy GO when
/// GetComponents&lt;Assignables&gt;() returns an empty array.
/// </summary>
public class AssignableProxyTest : PlayableGameTest {

    /// <summary>
    /// Baseline: a plain proxy GO has no Assignables components.
    /// This is the broken state before the fix.
    /// </summary>
    [Test]
    public void ProxyGO_WithoutOwnables_HasNoAssignablesComponents() {
        var proxyGo = createGameObject();
        var assignables = proxyGo.GetComponents<Assignables>();
        Assert.AreEqual(0, assignables.Length,
            "Fresh proxy GO should have zero Assignables before components are added");
    }

    /// <summary>
    /// After adding Ownables and Equipment (our fix), GetComponents&lt;Assignables&gt;()
    /// returns both — AssignableReachabilitySensor.ctor won't log an error and
    /// the slots array is built correctly.
    /// </summary>
    [Test]
    public void ProxyGO_WithOwnablesAndEquipment_HasTwoAssignablesComponents() {
        var proxyGo = createGameObject();
        proxyGo.AddOrGet<Ownables>();
        proxyGo.AddOrGet<Equipment>();

        var assignables = proxyGo.GetComponents<Assignables>();
        Assert.IsNotNull(assignables,
            "GetComponents<Assignables>() must not return null");
        Assert.AreEqual(2, assignables.Length,
            "Proxy must have exactly Ownables + Equipment as Assignables components");
    }

    /// <summary>
    /// Ownables and Equipment are both subclasses of Assignables — verifies the
    /// type hierarchy assumption the fix relies on.
    /// </summary>
    [Test]
    public void Ownables_And_Equipment_Are_Subclasses_Of_Assignables() {
        Assert.IsTrue(typeof(Assignables).IsAssignableFrom(typeof(Ownables)),
            "Ownables must be a subclass of Assignables");
        Assert.IsTrue(typeof(Assignables).IsAssignableFrom(typeof(Equipment)),
            "Equipment must be a subclass of Assignables");
    }
}
