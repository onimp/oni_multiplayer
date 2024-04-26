using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using MultiplayerMod.Core.Patch.ControlFlow;
using MultiplayerMod.Core.Patch.ControlFlow.Evaluators;
using NUnit.Framework;

namespace MultiplayerMod.Test.Core.Patch.Evaluators;

public class MethodBoundedDetourTests {

    private static readonly Harmony harmony = new("Test");
    private static readonly ControlFlowCustomizer customizer = new(harmony);

    [OneTimeTearDown]
    public static void TearDown() => harmony.UnpatchAll("Test");

    [Test]
    public void MustDetourInvocationsFromCurrentMethod() {
        Calc calc = new();
        var setValue = typeof(Calc).GetMethod(nameof(Calc.Set))!;
        var currentMethod = MethodBase.GetCurrentMethod()!;
        customizer.Detour(calc, setValue, new MethodBoundedDetour(currentMethod));
        Assert.AreEqual(expected: 0, actual: calc.Set(5));
        Assert.AreEqual(expected: 5, actual: calc.Add(5));
        Assert.AreEqual(expected: 0, actual: calc.Set(6));
        Assert.AreEqual(expected: 30, actual: calc.Mul(6));
    }

    private class Calc {

        private int Value { get; set; }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public int Set(int value) => Value = value;

        [MethodImpl(MethodImplOptions.NoInlining)]
        public int Add(int value) => Set(Value + value);

        [MethodImpl(MethodImplOptions.NoInlining)]
        public int Mul(int value) => Set(Value * value);

    }

}
