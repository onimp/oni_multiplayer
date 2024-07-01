using System;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Multiplayer.Objects.Extensions;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Multiplayer.Chores.Driver.Commands;

[Serializable]
public class SetDriverChore(ChoreDriver driver, ChoreConsumer consumer, Chore chore, object data) : MultiplayerCommand {

    private static Core.Logging.Logger log = LoggerFactory.GetLogger<SetDriverChore>();

    private ComponentReference<ChoreDriver> driverReference = driver.GetReference();
    private ComponentReference<ChoreConsumer> consumerReference = consumer.GetReference();
    private ChoreReference choreReference = chore.GetReference();
    private object? data = ArgumentUtils.WrapObject(data);

    public override void Execute(MultiplayerCommandContext context) {
        var chore = choreReference.Resolve();
        var driver = driverReference.Resolve();
        Chore.Precondition.Context choreContext;

        // TODO: A temporary solution until all chores are synced.
        // TODO: Now there can be a case when a consumer doesn't have required components.
        try {
            choreContext = new Chore.Precondition.Context(
                chore,
                new ChoreConsumerState(consumerReference.Resolve()),
                is_attempting_override: false,
                ArgumentUtils.UnWrapObject(data)
            );
        } catch (Exception exception) {
            log.Warning($"Unable to create chore context:\n{exception.StackTrace}");
            return;
        }
        context.Dependencies.Get<MultiplayerDriverChores>().Set(driver, ref choreContext);
    }

}
