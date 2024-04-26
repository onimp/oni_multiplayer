using System.Runtime.CompilerServices;
using HarmonyLib;
using MultiplayerMod.Core.Patch.ControlFlow;
using NUnit.Framework;

namespace MultiplayerMod.Test.Core.Patch;

[TestFixture]
public class ControlFlowCustomizerTests {

    private static readonly Harmony harmony = new("Test");
    private static readonly ControlFlowCustomizer customizer = new(harmony);

    [OneTimeTearDown]
    public static void TearDown() => harmony.UnpatchAll("Test");

    [Test]
    public void MustSkipMethodWithVoidReturn() {
        Container<int> container = new();
        container.SetValue(1);
        Assert.AreEqual(expected: 1, actual: container.Value);
        var setValue = typeof(Container<int>).GetMethod(nameof(Container<int>.SetValue))!;
        customizer.Detour(container, setValue);
        container.SetValue(2);
        Assert.AreEqual(expected: 1, actual: container.Value);
        customizer.Reset(container);
        container.SetValue(3);
        Assert.AreEqual(expected: 3, actual: container.Value);
    }

    [Test]
    public void MustSkipMethodWithValueTypeReturn() {
        Container<int> container = new();
        Assert.AreEqual(expected: 1, actual: container.SetValueRet(1));
        var setValue = typeof(Container<int>).GetMethod(nameof(Container<int>.SetValueRet))!;
        customizer.Detour(container, setValue);
        Assert.AreEqual(expected: 0, actual: container.SetValueRet(2));
        Assert.AreEqual(expected: 1, actual: container.Value);
        customizer.Reset(container);
        Assert.AreEqual(expected: 3, actual: container.SetValueRet(3));
        Assert.AreEqual(expected: 3, actual: container.Value);
    }

    [Test]
    public void MustSkipMethodWithValueTypeReturnAndOverrideReturnValue() {
        Container<int> container = new();
        Assert.AreEqual(expected: 1, actual: container.SetValueRet(1));
        var setValue = typeof(Container<int>).GetMethod(nameof(Container<int>.SetValueRet))!;
        customizer.Detour(container, setValue, returnValue: 10);
        Assert.AreEqual(expected: 10, actual: container.SetValueRet(2));
        Assert.AreEqual(expected: 1, actual: container.Value);
        customizer.Reset(container);
        Assert.AreEqual(expected: 3, actual: container.SetValueRet(3));
        Assert.AreEqual(expected: 3, actual: container.Value);
    }

    [Test]
    public void MustSkipMethodWithNullableTypeReturn() {
        Container<string> container = new();
        Assert.AreEqual(expected: "Earth", actual: container.SetValueRet("Earth"));
        var setValue = typeof(Container<string>).GetMethod(nameof(Container<string>.SetValueRet))!;
        customizer.Detour(container, setValue);
        Assert.AreEqual(expected: null, actual: container.SetValueRet("Mars"));
        Assert.AreEqual(expected: "Earth", actual: container.Value);
        customizer.Reset(container);
        Assert.AreEqual(expected: "Jupiter", actual: container.SetValueRet("Jupiter"));
        Assert.AreEqual(expected: "Jupiter", actual: container.Value);
    }

    [Test]
    public void MustSkipMethodWithNullableTypeReturnAndOverrideReturnValue() {
        Container<string> container = new();
        Assert.AreEqual(expected: "Earth", actual: container.SetValueRet("Earth"));
        var setValue = typeof(Container<string>).GetMethod(nameof(Container<string>.SetValueRet))!;
        customizer.Detour(container, setValue, returnValue: "Mercury");
        Assert.AreEqual(expected: "Mercury", actual: container.SetValueRet("Mars"));
        Assert.AreEqual(expected: "Earth", actual: container.Value);
        customizer.Reset(container);
        Assert.AreEqual(expected: "Jupiter", actual: container.SetValueRet("Jupiter"));
        Assert.AreEqual(expected: "Jupiter", actual: container.Value);
    }

    private class Container<T> {

        public T? Value { get; private set; }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void SetValue(T value) => Value = value;

        [MethodImpl(MethodImplOptions.NoInlining)]
        public T SetValueRet(T value) => Value = value;

    }

}
