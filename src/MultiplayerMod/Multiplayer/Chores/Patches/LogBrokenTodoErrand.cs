using System;
using System.Collections.Generic;
using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer;
using MultiplayerMod.Multiplayer.CoreOperations;
using MultiplayerMod.Multiplayer.Objects;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Chores.Patches;

// Diagnostic for the recurring client "Test text" errand.
//
// The duplicant "Current Errand" row is rendered by MinionTodoChoreEntry.Apply, which reads
// GameUtil.GetChoreName(chore) (-> choreType.Name) for the label and chore.target.gameObject.GetProperName()
// for the sub-label. "Test text" is NOT a string in any ONI/mod assembly or loc file - it is the baked
// placeholder text of the MinionTodoChoreEntry prefab, shown when Apply throws before it reaches
// label.SetText(...). It throws when the chore context is malformed on the client: a null chore, a null
// choreType, or a null/destroyed target - exactly the residue a failed chore reconstruction leaves behind.
//
// So instead of guessing the literal, this prefix reproduces the game's own render check: whenever Apply is
// about to render a context that WILL degrade to the placeholder, it dumps everything we know about that
// chore (type, choreType, target, driver, whether it is a known multiplayer object + its id) so we can trace
// which sync path produced the orphan. It only logs - the original Apply still runs unchanged, so the bug
// stays visible in-game. Deduped per (dupe, chore) so an open side screen doesn't spam the log every frame.
[UsedImplicitly]
[HarmonyPatch(typeof(MinionTodoChoreEntry), nameof(MinionTodoChoreEntry.Apply))]
public static class LogBrokenTodoErrand {

    private static readonly Core.Logging.Logger log = LoggerFactory.GetLogger(typeof(LogBrokenTodoErrand));

    private static readonly HashSet<int> logged = new();

    [UsedImplicitly]
    [HarmonyPrefix]
    [RequireMultiplayerMode(MultiplayerMode.Client)]
    [RequireExecutionLevel(ExecutionLevel.Multiplayer)]
    public static void LogIfBroken(Chore.Precondition.Context context) {
        try {
            var chore = context.chore;
            var target = chore?.target;
            var targetGo = target?.gameObject;
            var choreType = chore?.choreType;

            // Conditions under which the original Apply NREs before label.SetText -> prefab "Test text" shows.
            var brokenChore = chore == null;
            var brokenChoreType = choreType == null;
            var brokenTarget = target == null || targetGo == null; // Unity-null covers a destroyed GameObject
            if (!brokenChore && !brokenChoreType && !brokenTarget)
                return;

            var consumer = context.consumerState.consumer;
            var dupe = consumer != null ? consumer.gameObject.GetProperName() : "<null consumer>";

            var key = (dupe?.GetHashCode() ?? 0) * 397 ^ (chore?.GetHashCode() ?? 0);
            if (!logged.Add(key))
                return;
            if (logged.Count > 256) // debug tool: keep the dedup set bounded across a long session
                logged.Clear();

            string? multiplayerId = null;
            if (chore != null) {
                var mpObject = Runtime.Instance.Dependencies.Get<MultiplayerObjects>().Get(chore);
                multiplayerId = mpObject != null ? mpObject.Id.ToString() : "<not a multiplayer object>";
            }

            log.Warning(
                "Broken \"Test text\" errand about to render:\n" +
                $"  dupe            = {dupe}\n" +
                $"  chore           = {(chore != null ? chore.GetType().FullName : "<null>")}\n" +
                $"  choreType       = {(choreType != null ? $"{choreType.Id} (\"{choreType.Name}\")" : "<null>")}\n" +
                $"  target          = {DescribeTarget(target, targetGo)}\n" +
                $"  chore.gameObject= {(chore != null ? Describe(chore.gameObject) : "<null>")}\n" +
                $"  driver          = {(chore?.driver != null ? chore.driver.name : "<null>")}\n" +
                $"  multiplayerId   = {multiplayerId ?? "<n/a>"}\n" +
                $"  data            = {(context.data != null ? $"{context.data.GetType().Name}: {context.data}" : "<null>")}\n" +
                $"  reason          = {(brokenChore ? "null-chore " : "")}{(brokenChoreType ? "null-choreType " : "")}{(brokenTarget ? "null-target" : "")}"
            );
        } catch (Exception exception) {
            // Never let the diagnostic disturb the UI render it is only observing.
            log.Trace(() => $"LogBrokenTodoErrand failed: {exception}");
        }
    }

    private static string DescribeTarget(IStateMachineTarget? target, GameObject? go) {
        if (target == null)
            return "<null target>";
        return go == null ? "<destroyed gameObject>" : Describe(go);
    }

    private static string Describe(GameObject? go) =>
        go == null ? "<null>" : $"{go.GetProperName()} [{go.name}]";

}
