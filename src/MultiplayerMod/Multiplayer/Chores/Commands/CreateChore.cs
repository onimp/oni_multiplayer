using System;
using System.Collections.Generic;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Objects.Extensions;
using STRINGS;

namespace MultiplayerMod.Multiplayer.Chores.Commands;

[Serializable]
public class CreateChore : MultiplayerCommand {

    private static Core.Logging.Logger log = LoggerFactory.GetLogger<CreateChore>();

    public new readonly MultiplayerId Id;
    public readonly Type ChoreType;
    public readonly object?[] Arguments;

    public CreateChore(MultiplayerId id, Type choreType, object?[] arguments) {
        Id = id;
        ChoreType = choreType;
        Arguments = ArgumentUtils.WrapObjects(SpecialWrap(arguments));
    }

    public override void Execute(MultiplayerCommandContext context) {
        var args = SpecialUnWrap(context, ArgumentUtils.UnWrapObjects(Arguments));
        log.Debug($"Create chore {ChoreType} [id={Id}]");
        var chore = (Chore) ChoreType.GetConstructors()[0].Invoke(args);
        chore.Register(Id);
    }

    private object?[] SpecialWrap(object?[] args) {
        if (ChoreType == typeof(MoveChore)) {
            args[2] = ((Func<MoveChore.StatesInstance, int>) args[2]!).Invoke(null!);
        }
        if (ChoreType == typeof(VomitChore)) {
            args[2] = null;
            args[3] = null;
        }
        if (ChoreType == typeof(BansheeChore)) {
            args[2] = null;
        }
        if (ChoreType == typeof(FetchAreaChore)) {
            var context = (Chore.Precondition.Context) args[0]!;
            return new object[] {
                context.chore.MultiplayerId(), context.consumerState.consumer, context.consumerState.choreProvider,
                context.masterPriority.priority_class, context.masterPriority.priority_value
            };
        }
        return args;
    }

    private object?[] SpecialUnWrap(MultiplayerCommandContext context, object?[] args) {
        if (ChoreType == typeof(MoveChore)) {
            var targetCell = (int) args[2]!;
            args[2] = new Func<MoveChore.StatesInstance, int>(_ => targetCell);
        }
        if (ChoreType == typeof(VomitChore)) {
            args[2] = Db.Get().DuplicantStatusItems.Vomiting;
            args[3] = new Notification(
                (string) DUPLICANTS.STATUSITEMS.STRESSVOMITING.NOTIFICATION_NAME,
                NotificationType.Bad,
                (Func<List<Notification>, object, string>) ((notificationList, data) =>
                    (string) DUPLICANTS.STATUSITEMS.STRESSVOMITING.NOTIFICATION_TOOLTIP +
                    notificationList.ReduceMessages(false))
            );
        }
        if (ChoreType == typeof(BansheeChore)) {
            args[2] = new Notification(
                (string) DUPLICANTS.MODIFIERS.BANSHEE_WAILING.NOTIFICATION_NAME,
                NotificationType.Bad,
                (Func<List<Notification>, object, string>) ((notificationList, data) =>
                    (string) DUPLICANTS.MODIFIERS.BANSHEE_WAILING.NOTIFICATION_TOOLTIP +
                    notificationList.ReduceMessages(false))
            );
        }
        if (ChoreType == typeof(FetchAreaChore)) {
            var choreId = (MultiplayerId) args[0]!;
            var choreConsumer = (ChoreConsumer) args[1]!;
            var choreProvider = (ChoreProvider) args[2]!;
            var priorityClass = (PriorityScreen.PriorityClass) args[3]!;
            var priorityValue = (int) args[4]!;
            args = [
                new Chore.Precondition.Context {
                    chore = context.Multiplayer.Objects.Get<Chore>(choreId),
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
