using System;
using System.Collections.Generic;
using System.Reflection;
using MultiplayerMod.ModRuntime.StaticCompatibility;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Objects.Extensions;
using STRINGS;
using TUNING;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Chores.Serialization;

public static class ChoreArgumentsWrapper {

    // SocialGatheringPoint.workables (the private array of runtime child locators a gather building - the
    // Printing Pod - spawns in OnSpawn). Read reflectively so the client can grab its OWN locator by slot.
    private static readonly FieldInfo socialGatheringWorkablesField =
        typeof(SocialGatheringPoint).GetField("workables", BindingFlags.NonPublic | BindingFlags.Instance);

    // Full reconstruction from wrapped (serialized) arguments to a ready-to-invoke ctor argument array.
    // Every reconstruction site (CreateChore, RuntimeChoresStateManager) goes through here so they all apply
    // the same reference resolution AND per-chore fixups. SleepChore resolves tolerantly (an unresolved bed
    // becomes null) so its floor-locator fallback in Unwrap can fire instead of the whole chore aborting on a
    // bed that isn't present on this client - the bug where a client dupe stood idle all night.
    public static object?[] Reconstruct(Type choreType, object?[] wrappedArgs) {
        var args = choreType == typeof(SleepChore)
            ? ArgumentUtils.UnWrapObjectsTolerant(wrappedArgs)
            : ArgumentUtils.UnWrapObjects(wrappedArgs);
        return Unwrap(choreType, args);
    }

     public static object?[] Wrap(Type choreType, object?[] args) {
        if (choreType == typeof(MoveChore)) {
            args[2] = ((Func<MoveChore.StatesInstance, int>) args[2]!).Invoke(null!);
        }
        if (choreType == typeof(VomitChore)) {
            args[2] = null;
            args[3] = null;
        }
        if (choreType == typeof(BansheeChore)) {
            args[2] = null;
        }
        if (choreType == typeof(WaterCoolerChore)) {
            // args: [WaterCooler master, SocialGatheringPointWorkable chat_workable, on_complete, on_begin, on_end].
            // The chat workable is a runtime child locator of the cooler with no shared id, so ship its socialize
            // index instead - the client grabs the matching workable off its OWN cooler (Unwrap). Drop the host-side
            // bookkeeping callbacks (they mutate the host cooler's chore array; the workables outlive the chore).
            if (args[0] is WaterCooler cooler)
                args[1] = Array.IndexOf(cooler.workables, args[1]);
            args[2] = null;
            args[3] = null;
            args[4] = null;
        }
        if (choreType == typeof(PartyChore)) {
            // args: [locator master, PartyPointWorkable chat_workable, on_complete, on_begin, on_end]. Master and
            // workable are the SAME throw-away locator, spawned at a random cell with a random work time and no
            // shared id. Ship the cell + work time so the client rebuilds an equivalent locator (Unwrap).
            var locator = ((IStateMachineTarget) args[0]!).gameObject;
            var workable = (PartyPointWorkable) args[1]!;
            return [Grid.PosToCell(locator), workable.workTime];
        }
        if (choreType == typeof(FetchAreaChore)) {
            var context = (Chore.Precondition.Context) args[0]!;
            return [
                context.chore.MultiplayerId(), context.consumerState.consumer, context.consumerState.choreProvider,
                context.masterPriority.priority_class, context.masterPriority.priority_value
            ];
        }
        if (choreType == typeof(WorkChore<SocialGatheringPointWorkable>)) {
            // The Printing Pod's "gather" chore targets a runtime SocialGatheringPointWorkable locator that has
            // no shared id and isn't registered in Grid.Objects, so its GameObjectReference can't resolve on the
            // client (dupes never gather at the pod). The parent SocialGatheringPoint building IS resolvable, so
            // ship it in the target slot (WrapObjects turns it into a ComponentReference) plus the workable's
            // slot index in the always-null chore_provider slot; Unwrap swaps the client's OWN workable back in.
            var (targetIdx, providerIdx) = SocialGatheringArgIndices(choreType);
            if (targetIdx >= 0 && providerIdx >= 0 && args[targetIdx] is SocialGatheringPointWorkable workable &&
                args[providerIdx] == null && TryFindSocialGathering(workable, out var owner, out var slot)) {
                args[targetIdx] = owner;
                args[providerIdx] = slot;
            }
        }
        return args;
    }

    private static (int targetIdx, int providerIdx) SocialGatheringArgIndices(Type choreType) {
        var parameters = choreType.GetConstructors()[0].GetParameters();
        return (
            Array.FindIndex(parameters, parameter => parameter.Name == "target"),
            Array.FindIndex(parameters, parameter => parameter.Name == "chore_provider")
        );
    }

    // Host-side: find the SocialGatheringPoint that owns this workable locator (scanning only the workable's own
    // world) plus its slot index in the owner's workables array.
    private static bool TryFindSocialGathering(
        SocialGatheringPointWorkable workable, out SocialGatheringPoint owner, out int slot
    ) {
        owner = null!;
        slot = -1;
        if (workable == null || workable.gameObject == null || socialGatheringWorkablesField == null)
            return false;
        var worldId = Grid.WorldIdx[Grid.PosToCell(workable.gameObject)];
        foreach (var point in global::Components.SocialGatheringPoints.GetItems(worldId)) {
            if (socialGatheringWorkablesField.GetValue(point) is not SocialGatheringPointWorkable[] workables)
                continue;
            var index = Array.IndexOf(workables, workable);
            if (index < 0)
                continue;
            owner = point;
            slot = index;
            return true;
        }
        return false;
    }

    public static object?[] Unwrap(Type choreType, object?[] args) {
        if (choreType == typeof(MoveChore)) {
            var targetCell = (int) args[2]!;
            args[2] = new Func<MoveChore.StatesInstance, int>(_ => targetCell);
        }
        if (choreType == typeof(VomitChore)) {
            args[2] = Db.Get().DuplicantStatusItems.Vomiting;
            args[3] = new Notification(
                (string) DUPLICANTS.STATUSITEMS.STRESSVOMITING.NOTIFICATION_NAME,
                NotificationType.Bad,
                (Func<List<Notification>, object, string>) ((notificationList, data) =>
                    (string) DUPLICANTS.STATUSITEMS.STRESSVOMITING.NOTIFICATION_TOOLTIP +
                    notificationList.ReduceMessages(false))
            );
        }
        if (choreType == typeof(BansheeChore)) {
            args[2] = new Notification(
                (string) DUPLICANTS.MODIFIERS.BANSHEE_WAILING.NOTIFICATION_NAME,
                NotificationType.Bad,
                (Func<List<Notification>, object, string>) ((notificationList, data) =>
                    (string) DUPLICANTS.MODIFIERS.BANSHEE_WAILING.NOTIFICATION_TOOLTIP +
                    notificationList.ReduceMessages(false))
            );
        }
        // SleepChore ctor[0] args: [ChoreType, IStateMachineTarget target, GameObject bed, bool bedIsLocator,
        // bool isInterruptable]. The bed reference doesn't resolve on this client in two cases: floor / passed-out
        // sleep (bedIsLocator == true) ships a runtime-built locator with no shared MultiplayerId, and a bed built
        // since the last hard-sync isn't present here yet. Reconstruct resolves this arg tolerantly, so an
        // unresolved bed arrives as null. In either case rebuild an equivalent floor locator on the client's OWN
        // sleeper: GetSafeFloorLocator runs the same SafeCellSensor query the host used, so the dupe lies down and
        // plays the floor sleep animation in place instead of failing to reconstruct and standing idle all night.
        // A bed that DID resolve (normal assigned bed with a shared id) keeps it and sleeps in bed as before.
        if (choreType == typeof(SleepChore) && args.Length >= 5) {
            var bedUnresolved = args[2] == null;
            if ((bedUnresolved || args[3] is true) && args[1] is IStateMachineTarget target && target.gameObject != null) {
                args[2] = SleepChore.GetSafeFloorLocator(target.gameObject).gameObject;
                args[3] = true; // the substitute IS a locator, so SleepChore treats it as one
            }
        }
        if (choreType == typeof(WaterCoolerChore)) {
            // The cooler (args[0]) resolved via its shared id; swap the shipped socialize index back for the
            // client's own workable so the chit-chat locator points at a real, local target.
            var cooler = (WaterCooler) args[0]!;
            args[1] = cooler.workables[(int) args[1]!];
        }
        if (choreType == typeof(PartyChore)) {
            // Rebuild the throw-away party locator on the client at the host's cell + work time (mirrors the host
            // spawn in NewYearParty), and give it a client-side on_end that destroys the locator so nothing leaks.
            var cell = (int) args[0]!;
            var workTime = (float) args[1]!;
            var locator = ChoreHelpers.CreateLocator("PartyWorkable", Grid.CellToPosCBC(cell, Grid.SceneLayer.Move));
            var workable = locator.AddOrGet<PartyPointWorkable>();
            workable.SetWorkTime(workTime);
            workable.basePriority = RELAXATION.PRIORITY.SPECIAL_EVENT;
            workable.faceTargetWhenWorking = true;
            args = [
                locator.GetComponent<IStateMachineTarget>(), workable, null, null,
                (Action<Chore>) (_ => Util.KDestroyGameObject(locator))
            ];
        }
        if (choreType == typeof(WorkChore<SocialGatheringPointWorkable>)) {
            // The owner building (args[target]) resolved via its shared reference; swap the shipped slot index
            // back for this client's OWN gather locator so the chore targets a real, local workable. Restore the
            // borrowed chore_provider slot to null (its original value on the host).
            var (targetIdx, providerIdx) = SocialGatheringArgIndices(choreType);
            if (targetIdx >= 0 && providerIdx >= 0 && args[targetIdx] is SocialGatheringPoint owner &&
                args[providerIdx] is int slot) {
                args[providerIdx] = null;
                var workables = socialGatheringWorkablesField?.GetValue(owner) as SocialGatheringPointWorkable[];
                if (workables == null || slot < 0 || slot >= workables.Length || workables[slot] == null)
                    throw new InvalidOperationException(
                        $"SocialGatheringPoint gather locator (slot {slot}) not available on this client"
                    );
                args[targetIdx] = workables[slot];
            }
        }
        if (choreType == typeof(FetchAreaChore)) {
            var choreId = (MultiplayerId) args[0]!;
            var choreConsumer = (ChoreConsumer) args[1]!;
            var choreProvider = (ChoreProvider) args[2]!;
            var priorityClass = (PriorityScreen.PriorityClass) args[3]!;
            var priorityValue = (int) args[4]!;
            args = [
                new Chore.Precondition.Context {
                    chore = Dependencies.Get<MultiplayerGame>().Objects.Get<Chore>(choreId),
                    consumerState = new ChoreConsumerState(choreConsumer) {
                        choreProvider = choreProvider
                    },
                    masterPriority = new PrioritySetting(priorityClass, priorityValue)
                }
            ];
        }
        return args;
    }

}
