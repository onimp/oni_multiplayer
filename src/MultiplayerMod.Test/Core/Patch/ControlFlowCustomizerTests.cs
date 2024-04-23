using HarmonyLib;
using MultiplayerMod.Core.Patch;
using NUnit.Framework;

namespace MultiplayerMod.Test.Core.Patch;

[TestFixture]
public class ControlFlowCustomizerTests {

    private static readonly Harmony harmony = new("Test");
    private static readonly ControlFlowCustomizer customizer = new(harmony);

    [OneTimeTearDown]
    public static void TearDown() => harmony.UnpatchAll("Test");

    [Test]
    public void MustDisableMethodWithVoidReturn() {
        Container<int> container = new();
        container.SetValue(1);
        Assert.AreEqual(expected: 1, actual: container.Value);
        var setValue = typeof(Container<int>).GetMethod(nameof(Container<int>.SetValue))!;
        customizer.DisableMethod(container, setValue);
        container.SetValue(2);
        Assert.AreEqual(expected: 1, actual: container.Value);
        customizer.EnableMethods(container);
        container.SetValue(3);
        Assert.AreEqual(expected: 3, actual: container.Value);
    }

    [Test]
    public void MustDisableMethodWithValueTypeReturn() {
        Container<int> container = new();
        Assert.AreEqual(expected: 1, actual: container.SetValueRet(1));
        var setValue = typeof(Container<int>).GetMethod(nameof(Container<int>.SetValueRet))!;
        customizer.DisableMethod(container, setValue);
        Assert.AreEqual(expected: 0, actual: container.SetValueRet(2));
        Assert.AreEqual(expected: 1, actual: container.Value);
        customizer.EnableMethods(container);
        Assert.AreEqual(expected: 3, actual: container.SetValueRet(3));
        Assert.AreEqual(expected: 3, actual: container.Value);
    }

    [Test]
    public void MustDisableMethodWithValueTypeReturnAndOverrideReturnValue() {
        Container<int> container = new();
        Assert.AreEqual(expected: 1, actual: container.SetValueRet(1));
        var setValue = typeof(Container<int>).GetMethod(nameof(Container<int>.SetValueRet))!;
        customizer.DisableMethod(container, setValue, 10);
        Assert.AreEqual(expected: 10, actual: container.SetValueRet(2));
        Assert.AreEqual(expected: 1, actual: container.Value);
        customizer.EnableMethods(container);
        Assert.AreEqual(expected: 3, actual: container.SetValueRet(3));
        Assert.AreEqual(expected: 3, actual: container.Value);
    }

    [Test]
    public void MustDisableMethodWithNullableTypeReturn() {
        Container<string> container = new();
        Assert.AreEqual(expected: "Earth", actual: container.SetValueRet("Earth"));
        var setValue = typeof(Container<string>).GetMethod(nameof(Container<string>.SetValueRet))!;
        customizer.DisableMethod(container, setValue);
        Assert.AreEqual(expected: null, actual: container.SetValueRet("Mars"));
        Assert.AreEqual(expected: "Earth", actual: container.Value);
        customizer.EnableMethods(container);
        Assert.AreEqual(expected: "Jupiter", actual: container.SetValueRet("Jupiter"));
        Assert.AreEqual(expected: "Jupiter", actual: container.Value);
    }

    [Test]
    public void MustDisableMethodWithNullableTypeReturnAndOverrideReturnValue() {
        Container<string> container = new();
        Assert.AreEqual(expected: "Earth", actual: container.SetValueRet("Earth"));
        var setValue = typeof(Container<string>).GetMethod(nameof(Container<string>.SetValueRet))!;
        customizer.DisableMethod(container, setValue, "Mercury");
        Assert.AreEqual(expected: "Mercury", actual: container.SetValueRet("Mars"));
        Assert.AreEqual(expected: "Earth", actual: container.Value);
        customizer.EnableMethods(container);
        Assert.AreEqual(expected: "Jupiter", actual: container.SetValueRet("Jupiter"));
        Assert.AreEqual(expected: "Jupiter", actual: container.Value);
    }

    class Container<T> {
        public T? Value { get; private set; }

        public virtual void SetValue(T value) {
            Value = value;
        }

        public virtual T SetValueRet(T value) {
            Value = value;
            return Value;
        }
    }

}
