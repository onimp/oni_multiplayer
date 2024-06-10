using System;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Multiplayer.Objects.Extensions;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Multiplayer.Chores.Commands;

[Serializable]
public class QueueChore(ChoreDriver driver, ChoreConsumer consumer, Chore chore, object data) : MultiplayerCommand {

    private ComponentReference<ChoreDriver> driverReference = driver.GetReference();
    private ComponentReference<ChoreConsumer> consumerReference = consumer.GetReference();
    private ChoreReference choreReference = chore.GetReference();
    private object? data = ArgumentUtils.WrapObject(data);

    public override void Execute(MultiplayerCommandContext context) {
        var chore = choreReference.Resolve();
        var driver = driverReference.Resolve();
        var choreContext = new Chore.Precondition.Context(
            chore,
            new ChoreConsumerState(consumerReference.Resolve()),
            is_attempting_override: false,
            ArgumentUtils.UnWrapObject(data)
        );
        context.Dependencies.Get<DriverChoresQueue>().Enqueue(driver, ref choreContext);
    }

}
