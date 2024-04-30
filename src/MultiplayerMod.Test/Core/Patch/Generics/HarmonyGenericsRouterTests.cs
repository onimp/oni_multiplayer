using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.Core.Patch.Generics;
using NUnit.Framework;

namespace MultiplayerMod.Test.Core.Patch.Generics;

[TestFixture]
public class HarmonyGenericsRouterTests {

    [HarmonyPatch(typeof(BaseClass<DerivedAlpha, string>.Subclass), nameof(DerivedAlpha.Subclass.SetValue))]
    public static class SimpleGenericPatcher {

        [UsedImplicitly]
        public static bool Prefix() => true;

    }

    [HarmonyPatch(typeof(BaseClass<DerivedAlpha, string>.Subclass), nameof(DerivedAlpha.Subclass.SetValue))]
    public static class RoutedGenericPatcher {

        [UsedImplicitly]
        public static bool Prefix(
            MethodBase __originalMethod,
            object __instance,
            object[] __args,
            ref object? __result
        ) => !HarmonyGenericsRouter.TryRoute(__originalMethod, __instance, __args, ref __result);

    }

    [Test]
    public void InvalidInternalGenericTypes() {
        var harmony = new Harmony(nameof(HarmonyGenericsRouterTests));
        harmony.CreateClassProcessor(typeof(HarmonyGenericsRouter)).Patch();
        harmony.CreateClassProcessor(typeof(SimpleGenericPatcher)).Patch();

        var alpha = new DerivedAlpha();
        var beta = new DerivedBeta();
        var gamma = new DerivedGamma();
        alpha.Init();
        beta.Init();
        gamma.Init();

        // Reference types should fail - the same generic implementation is used for all reference types
        Assert.AreSame(expected: typeof(Container<string>), actual: alpha.subA.Value!.GetType());
        Assert.AreSame(expected: typeof(Container<string>), actual: alpha.subB.Value!.GetType());
        Assert.AreSame(expected: typeof(Container<string>), actual: beta.subA.Value!.GetType());
        Assert.AreSame(expected: typeof(Container<string>), actual: beta.subB.Value!.GetType());

        // Value types should work fine - different generic implementation is produced
        Assert.AreSame(expected: typeof(Container<int>), actual: gamma.subA.Value!.GetType());
        Assert.AreSame(expected: typeof(Container<int>), actual: gamma.subB.Value!.GetType());

        harmony.UnpatchAll(nameof(HarmonyGenericsRouterTests));
    }

    [Test]
    public void CorrectInternalGenericTypes() {
        var harmony = new Harmony(nameof(HarmonyGenericsRouterTests));
        harmony.CreateClassProcessor(typeof(HarmonyGenericsRouter)).Patch();
        harmony.CreateClassProcessor(typeof(RoutedGenericPatcher)).Patch();

        var alpha = new DerivedAlpha();
        var beta = new DerivedBeta();
        var gamma = new DerivedGamma();
        alpha.Init();
        beta.Init();
        gamma.Init();

        // Reference types should work fine - generic types must be routed to the correct patches
        Assert.AreSame(expected: typeof(Container<string>), actual: alpha.subA.Value!.GetType());
        Assert.AreSame(expected: typeof(Container<string>), actual: alpha.subB.Value!.GetType());
        Assert.AreSame(expected: typeof(Container<Marker>), actual: beta.subA.Value!.GetType());
        Assert.AreSame(expected: typeof(Container<Marker>), actual: beta.subB.Value!.GetType());

        // Value types should work fine - different generic implementation is produced
        Assert.AreSame(expected: typeof(Container<int>), actual: gamma.subA.Value!.GetType());
        Assert.AreSame(expected: typeof(Container<int>), actual: gamma.subB.Value!.GetType());

        harmony.UnpatchAll(nameof(HarmonyGenericsRouterTests));
    }

    public class Marker(string value) {
        public string Value = value;
    }

    public class Container<T>(T value) {
        public T Value { get; } = value;
    }

    public class BaseClass<T1, T2> where T1 : BaseClass<T1, T2> {

        public class Subclass {

            public Container<T2>? Value;

            [MethodImpl(MethodImplOptions.NoInlining)]
            public Subclass SetValue(T2 value) {
                Value = new Container<T2>(value);
                return this;
            }

        }

        public virtual void Init() { }

    }

    public class DerivedAlpha : BaseClass<DerivedAlpha, string> {

        public Subclass subA = new();
        public Subclass subB = new();

        public override void Init() {
            subA.SetValue("Sub Alpha A");
            subB.SetValue("Sub Alpha B");
        }

        public new class Subclass : BaseClass<DerivedAlpha, string>.Subclass;

    }

    public class DerivedBeta : BaseClass<DerivedBeta, Marker> {

        public Subclass subA = new();
        public Subclass subB = new();

        public override void Init() {
            subA.SetValue(new Marker("Sub Beta A"));
            subB.SetValue(new Marker("Sub Beta B"));
        }

        public new class Subclass : BaseClass<DerivedBeta, Marker>.Subclass;

    }

    public class DerivedGamma : BaseClass<DerivedGamma, int> {

        public Subclass subA = new();
        public Subclass subB = new();

        public override void Init() {
            subA.SetValue(1);
            subB.SetValue(2);
        }

        public new class Subclass : BaseClass<DerivedGamma, int>.Subclass;

    }

}
