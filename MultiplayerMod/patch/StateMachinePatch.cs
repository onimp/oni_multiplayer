using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MultiplayerMod.multiplayer;

namespace MultiplayerMod.patch
{

    [HarmonyPatch]
    public static class StateMachinePatch
    {
        public static bool DisablePatch;
        public static event Action<object[]> OnGoToState;

        private static MethodInfo TargetMethod()
        {
            return typeof(StateMachine<object, StateMachine.Instance, IStateMachineTarget, object>.GenericInstance)
                .GetMethod(
                    "GoTo",
                    BindingFlags.Public | BindingFlags.Instance,
                    null,
                    CallingConventions.Any,
                    new[] { typeof(StateMachine.BaseState) },
                    null
                );
        }

        public static bool Prefix(
            StateMachine.Instance __instance,
            StateMachine.BaseState base_state,
            ref int ___gotoId,
            ref int ___stackSize,
            ref Stack<StateMachine.BaseState> ___gotoStack,
            ref object ___stateStack,
            ref StateMachineController ___controller,
            ref StateMachine ___stateMachine
        )
        {
            var executeOriginal = Prefix(__instance, base_state);

            if (executeOriginal)
                Original(
                    __instance,
                    base_state,
                    ref ___gotoId,
                    ref ___stackSize,
                    ref ___gotoStack,
                    ref ___stateStack,
                    ref ___controller,
                    ref ___stateMachine
                );
            return false;
        }

        private static bool Prefix(
            StateMachine.Instance __instance,
            StateMachine.BaseState base_state
        )
        {
            if (DisablePatch) return true;

            if (__instance == null || __instance.GetComponent<KPrefabID>() == null || base_state == null)
            {
                Debug.LogWarning($"WTF????? {__instance} {base_state}");
                return true;
            }

            OnGoToState?.Invoke(
                new object[]
                {
                    __instance.GetComponent<KPrefabID>().InstanceID,
                    base_state.name,
                    __instance.GetType(),
                    Environment.StackTrace
                }
            );

            // Skip original method on the client side.
            return MultiplayerState.MultiplayerRole != MultiplayerState.Role.Client ||
                   __instance.GetCurrentState() == null;
        }

        private static void Original(
            StateMachine.Instance __instance,
            StateMachine.BaseState base_state,
            ref int ___gotoId,
            ref int ___stackSize,
            ref Stack<StateMachine.BaseState> ___gotoStack,
            ref object ___stateStack,
            ref StateMachineController ___controller,
            ref StateMachine ___stateMachine
        )
        {
            // Searching typed StateMachine<?,?,?,?>.GenericInstance type
            var instanceType = __instance.GetType();
            while (!instanceType.Name.Contains("GenericInstance"))
                instanceType = instanceType.BaseType;

            if (App.IsExiting || StateMachine.Instance.error ||
                __instance.GetProperty<bool>("isMasterNull")
               )
                return;
            if (__instance.GetPrivateProperty<object>("smi").IsNullOrDestroyed())
                return;

            try
            {
                if (__instance.IsBreakOnGoToEnabled())
                    Debugger.Break();
                if (base_state != null)
                    while (base_state.defaultState != null)
                        base_state = base_state.defaultState;
                if (__instance.GetCurrentState() == null)
                    __instance.SetStatus(StateMachine.Status.Running);
                if (___gotoStack.Count > 100)
                {
                    var str = "Potential infinite transition loop detected in state machine: " + __instance +
                              "\nGoto stack:\n";
                    str = ___gotoStack.Aggregate(str, (current, baseState) => current + "\n" + baseState.name);
                    Debug.LogError(str);
                    __instance.Error();
                } else
                {
                    ___gotoStack.Push(base_state);
                    if (base_state == null)
                    {
                        __instance.StopSM("StateMachine.GoTo(null)");
                        ___gotoStack.Pop();
                    } else
                    {
                        var num = ++___gotoId;
                        var branch = base_state.branch;
                        var index1 = 0;
                        while (index1 < ___stackSize && index1 < branch.Length)
                        {
                            var stackItem = ___stateStack.GetType().GetMethod("GetValue", new[] { typeof(int) })
                                .Invoke(
                                    ___stateStack,
                                    new object[] { index1 }
                                );

                            var baseState = stackItem.GetField<StateMachine.BaseState>("state");
                            if (baseState != branch[index1]) break;

                            ++index1;
                        }
                        var index2 = ___stackSize - 1;
                        if (index2 >= 0 && index2 == index1 - 1)
                        {
                            var stackItem = ___stateStack.GetType().GetMethod("GetValue", new[] { typeof(int) })
                                .Invoke(
                                    ___stateStack,
                                    new object[] { index2 }
                                );
                            __instance.InvokePrivate(
                                instanceType,
                                "FinishStateInProgress",
                                stackItem.GetField<StateMachine.BaseState>("state")
                            );
                        }
                        while (___stackSize > index1 && num == ___gotoId)
                            __instance.InvokePrivate(instanceType, "PopState");
                        for (var index3 = index1; index3 < branch.Length && num == ___gotoId; ++index3)
                            __instance.InvokePrivate(instanceType, "PushState", branch[index3]);
                        ___gotoStack.Pop();
                    }
                }
            } catch (Exception ex)
            {
                if (StateMachine.Instance.error)
                    return;

                //  __instance.Error();
                var str1 = "(Stop)";
                if (base_state != null)
                    str1 = base_state.name;
                var str2 = "(NULL).";
                if (!__instance.GetMaster().isNull)
                    str2 = "(" + __instance.gameObject.name + ").";
                // DebugUtil.LogErrorArgs(
                //     ___controller,
                //     "Exception in: " + str2 + ___stateMachine + ".GoTo(" + str1 + ")" + "\n" +
                //     ex
                // );
                throw ex;
            }
        }
    }

}
