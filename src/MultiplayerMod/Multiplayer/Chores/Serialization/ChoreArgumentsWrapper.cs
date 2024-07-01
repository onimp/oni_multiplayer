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
