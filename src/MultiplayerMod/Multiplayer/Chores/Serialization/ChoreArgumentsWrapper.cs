using System;
using System.Collections.Generic;
using MultiplayerMod.ModRuntime.StaticCompatibility;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Objects.Extensions;
using STRINGS;

namespace MultiplayerMod.Multiplayer.Chores.Serialization;

public static class ChoreArgumentsWrapper {

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
        if (choreType == typeof(FetchAreaChore)) {
            var context = (Chore.Precondition.Context) args[0]!;
            return [
                context.chore.MultiplayerId(), context.consumerState.consumer, context.consumerState.choreProvider,
                context.masterPriority.priority_class, context.masterPriority.priority_value
            ];
        }
        return args;
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
        // bool isInterruptable]. Floor / passed-out sleep (bedIsLocator == true) ships a runtime-built locator
        // GameObject that has no shared MultiplayerId, so the host's reference can't resolve here - it degrades
        // to whatever occupies the grid cell (usually the sleeping dupe, which has no Sleepable, so
        // SleepChore.SetAnim NREs and the assignment is skipped -> the client dupe just stands while the host
        // sleeps). Rebuild an equivalent floor locator on the client's OWN sleeper: GetSafeFloorLocator runs the
        // same SafeCellSensor query the host used, so it lands on the same cell, and the dupe lies down and plays
        // the floor sleep animation in place. Bed-backed sleep (bedIsLocator == false) keeps its resolved bed.
        if (choreType == typeof(SleepChore) && args.Length >= 5 && args[3] is true) {
            if (args[1] is IStateMachineTarget target && target.gameObject != null)
                args[2] = SleepChore.GetSafeFloorLocator(target.gameObject).gameObject;
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
