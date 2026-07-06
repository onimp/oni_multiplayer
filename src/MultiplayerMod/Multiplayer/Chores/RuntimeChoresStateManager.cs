using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Multiplayer.Chores.Driver;
using MultiplayerMod.Multiplayer.Chores.Events;
using MultiplayerMod.Multiplayer.Chores.Serialization;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Objects.Extensions;
using MultiplayerMod.Multiplayer.Objects.Reference;
using MultiplayerMod.Multiplayer.World;
using MultiplayerMod.Multiplayer.World.Data;
using Object = UnityEngine.Object;

namespace MultiplayerMod.Multiplayer.Chores;

[Dependency, UsedImplicitly]
public class RuntimeChoresStateManager : IWorldStateManager {

    private readonly MultiplayerObjects objects;
    private readonly MultiplayerDriverChores driverChores;
    private readonly Dictionary<Chore, ChoreCreatedEvent> events = new();

    private readonly Core.Logging.Logger log = Core.Logging.LoggerFactory.GetLogger<RuntimeChoresStateManager>();

    public RuntimeChoresStateManager(EventDispatcher events, MultiplayerObjects objects, MultiplayerDriverChores driver) {
        this.objects = objects;
        driverChores = driver;
        events.Subscribe<ChoreCreatedEvent>(@event => this.events.Add(@event.Chore, @event));
        events.Subscribe<ChoreCleanupEvent>(@event => this.events.Remove(@event.Chore));
    }

    public void SaveState(WorldState worldState) {
        var state = new ChoresState(worldState);

        // Serialize each chore/driver defensively: a single chore whose argument references a
        // destroyed or never-registered object would otherwise throw and abort the whole world sync,
        // leaving joining clients stuck on the loading screen. Skip the offending entry and log it.
        state.Chores = events.Values
            .Select(TrySerializeChore)
            .Where(it => it != null)
            .Select(it => it!.Value)
            .ToArray();

        state.Drivers = Object.FindObjectsOfType<ChoreDriver>()
            .Where(it => {
                var chore = it.GetCurrentChore();
                if (chore == null)
                    return false;

                var multiplayerObject = objects.Get(chore);
                return multiplayerObject != null && multiplayerObject.Persistent;
            })
            .Select(TrySerializeDriver)
            .Where(it => it != null)
            .Select(it => it!.Value)
            .ToArray();
    }

    private ChoreState? TrySerializeChore(ChoreCreatedEvent it) {
        try {
            return new ChoreState {
                id = it.Id,
                type = it.Type,
                arguments = ArgumentUtils.WrapObjects(ChoreArgumentsWrapper.Wrap(it.Type, it.Arguments))
            };
        } catch (Exception exception) {
            log.Warning($"Skipping chore {it.Type} from world sync: {exception.Message}");
            return null;
        }
    }

    private ChoreDriverState? TrySerializeDriver(ChoreDriver it) {
        try {
            return new ChoreDriverState {
                driverReference = it.GetReference(),
                consumerReference = it.context.consumerState.consumer.GetReference(),
                choreReference = new ChoreReference(it.GetCurrentChore())
            };
        } catch (Exception exception) {
            log.Warning($"Skipping chore driver from world sync: {exception.Message}");
            return null;
        }
    }

    public void LoadState(WorldState worldState) {
        var state = new ChoresState(worldState);

        // Load each chore/driver defensively, mirroring SaveState. A single argument that references an
        // object not present on this client (runtime-spawned under a shared id, destroyed, or not yet
        // replicated) throws ObjectNotFoundException; without a per-entry guard that one failure aborts the
        // whole foreach — dropping every remaining chore AND all drivers below — and the chore-completion
        // fallback synchronizers then spam trying to self-heal the missing state. Skip just the offending
        // entry and log which reference failed; the fallbacks reconcile the genuine stragglers.
        foreach (var choreState in state.Chores) {
            try {
                var args = ChoreArgumentsWrapper.Unwrap(choreState.type, ArgumentUtils.UnWrapObjects(choreState.arguments));
                var chore = (Chore) choreState.type.GetConstructors()[0].Invoke(args);
                chore.Register(choreState.id);
            } catch (Exception exception) {
                log.Warning($"Skipping chore {choreState.type} on load: {exception.Message}");
            }
        }

        foreach (var choreDriverState in state.Drivers) {
            try {
                var driver = choreDriverState.driverReference.Resolve();
                var chore = choreDriverState.choreReference.Resolve();
                var consumer = choreDriverState.consumerReference.Resolve();
                var choreContext = new Chore.Precondition.Context(
                    chore,
                    new ChoreConsumerState(consumer),
                    is_attempting_override: false
                );
                driverChores.Set(driver, ref choreContext);
            } catch (Exception exception) {
                log.Warning($"Skipping chore driver on load: {exception.Message}");
            }
        }
    }

    private class ChoresState(WorldState state) {
        private const string choresKey = "chores";
        private const string choreDriversKey = "drivers";

        public ChoreState[] Chores {
            get => (ChoreState[])state.Entries[choresKey];
            set => state.Entries[choresKey] = value;
        }

        public ChoreDriverState[] Drivers {
            get => (ChoreDriverState[])state.Entries[choreDriversKey];
            set => state.Entries[choreDriversKey] = value;
        }
    }

    [Serializable]
    private struct ChoreState {
        public MultiplayerId id;
        public Type type;
        public object?[] arguments;
    }

    [Serializable]
    private struct ChoreDriverState {
        public ComponentReference<ChoreDriver> driverReference;
        public ComponentReference<ChoreConsumer> consumerReference;
        public ChoreReference choreReference;
    }

}
