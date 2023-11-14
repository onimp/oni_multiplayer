using System;
using JetBrains.Annotations;
using NUnit.Framework;
using UnityEngine;

namespace MultiplayerMod.Test.Environment.Unity;

[TestFixture]
public class UnityTestRuntimeTest {

    [OneTimeSetUp]
    public static void SetUp() => UnityTestRuntime.Install();

    [OneTimeTearDown]
    public static void TearDown() => UnityTestRuntime.Uninstall();

    [Test]
    public unsafe void GetComponentFastPath() {
        var castHelper = new CastHelperFake<MinionBrain>();
        var gameObject = new GameObject();
        var expectedResult = UnityTestRuntime.AddComponent(gameObject, typeof(MinionBrain));

        UnityTestRuntime.GetComponentFastPath(
            gameObject,
            typeof(MinionBrain),
            new IntPtr(&castHelper.OnePointerFurtherThanT)
        );
        var component = castHelper.Value;

        Assert.AreEqual(expectedResult, component);
    }

    private struct CastHelperFake<T> {
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
        public T Value;
        public IntPtr OnePointerFurtherThanT;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value
    }
}
