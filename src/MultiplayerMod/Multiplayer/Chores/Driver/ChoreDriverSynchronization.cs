using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using MultiplayerMod.Core.Collections;
using MultiplayerMod.Core.Extensions;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Game.Chores;
using MultiplayerMod.Multiplayer.Chores.Commands;
using MultiplayerMod.Multiplayer.Chores.Driver.Commands;
using MultiplayerMod.Multiplayer.Chores.Serialization;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.Chores.Driver;

[Core.Dependency.Dependency, UsedImplicitly]
public class ChoreDriverSynchronization {

    private readonly ChoresPatcher processor;
    private readonly IMultiplayerServer server;
    private readonly MultiplayerObjects objects;

    private readonly Core.Logging.Logger log = LoggerFactory.GetLogger<ChoreDriverSynchronization>();

    private readonly ConditionalWeakTable<ChoreDriver, BoxedValue<bool>> driverSynchronizationState = new();

    public ChoreDriverSynchronization(ChoresPatcher processor, IMultiplayerServer server, MultiplayerObjects objects) {
        this.processor = processor;
        this.server = server;
        this.objects = objects;
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
            server.Send(new ReleaseChoreDriver(driver));
            synchronized.Value = false;
        }

        if (!processor.Supported(context.chore))
            return;

        // WorkChore<> is never created through the constructor-patch path (patching its generic ctor
        // corrupts Mono generic sharing), so it does not yet exist on the client. Replicate it here,
        // immediately before assigning it, from a recipe built off the instance. If replication fails
        // we skip the driver sync so we never send a SetDriverChore whose ChoreReference can't resolve.
        if (!EnsureReplicated(context.chore))
            return;

        var command = new SetDriverChore(driver, context.consumerState.consumer, context.chore, context.data);
        server.Send(command);
        synchronized.Value = true;
    }

    private bool EnsureReplicated(Chore chore) {
        if (objects.Get(chore) != null)
            return true; // already on the client (normal chores, or a WorkChore replicated earlier)
        if (!ChoresPatcher.IsWorkChore(chore.GetType()))
            return true; // non-WorkChore supported chores are created via their patched constructor

        try {
            var arguments = WorkChoreArguments.Build(chore);
            var id = objects.Register(chore).Id;
            server.Send(new CreateChore(id, chore.GetType(), arguments));
            log.Debug($"Replicated {chore.GetType().GetSignature()} [id={id}]");
            return true;
        } catch (Exception exception) {
            log.Warning($"Unable to replicate {chore.GetType().GetSignature()}: {exception.Message}");
            return false;
        }
    }

}
