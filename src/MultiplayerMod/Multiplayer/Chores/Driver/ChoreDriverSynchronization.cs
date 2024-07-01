using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using MultiplayerMod.Core.Collections;
using MultiplayerMod.Game.Chores;
using MultiplayerMod.Multiplayer.Chores.Driver.Commands;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.Chores.Driver;

[Core.Dependency.Dependency, UsedImplicitly]
public class ChoreDriverSynchronization {

    private readonly ChoresPatcher processor;
    private readonly IMultiplayerServer server;

    private readonly ConditionalWeakTable<ChoreDriver, BoxedValue<bool>> driverSynchronizationState = new();

    public ChoreDriverSynchronization(ChoresPatcher processor, IMultiplayerServer server) {
        this.processor = processor;
        this.server = server;
        ChoreDriverEvents.ChoreSetting += ChoreDriverOnChoreSetting;
    }

    private void ChoreDriverOnChoreSetting(
        ChoreDriver driver,
        Chore? previousChore,
        ref Chore.Precondition.Context context
    ) {
        var synchronized = driverSynchronizationState.GetValue(driver, _ => new BoxedValue<bool>(false));
        var shouldReleaseDriver = synchronized.Value && previousChore != null && processor.Supported(previousChore);
        if (shouldReleaseDriver) {
            server.Send(new ReleaseChoreDriver(driver), MultiplayerCommandOptions.SkipHost);
            synchronized.Value = false;
        }

        if (!processor.Supported(context.chore))
            return;

        var command = new SetDriverChore(driver, context.consumerState.consumer, context.chore, context.data);
        server.Send(command, MultiplayerCommandOptions.SkipHost);
        synchronized.Value = true;
    }

}
