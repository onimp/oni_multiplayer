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

    public RuntimeChoresStateManager(EventDispatcher events, MultiplayerObjects objects, MultiplayerDriverChores driver) {
        this.objects = objects;
        driverChores = driver;
        events.Subscribe<ChoreCreatedEvent>(@event => this.events.Add(@event.Chore, @event));
        events.Subscribe<ChoreCleanupEvent>(@event => this.events.Remove(@event.Chore));
    }

    public void SaveState(WorldState worldState) {
        var state = new ChoresState(worldState);

        state.Chores = events.Values.Select(it => new ChoreState {
            id = it.Id,
            type = it.Type,
            arguments = ArgumentUtils.WrapObjects(ChoreArgumentsWrapper.Wrap(it.Type, it.Arguments))
        }).ToArray();

        state.Drivers = Object.FindObjectsOfType<ChoreDriver>()
            .Where(it => {
                var chore = it.GetCurrentChore();
                if (chore == null)
                    return false;

                var multiplayerObject = objects.Get(chore);
                return multiplayerObject != null && multiplayerObject.Persistent;
            })
            .Select(it => new ChoreDriverState {
                driverReference = it.GetReference(),
                consumerReference = it.context.consumerState.consumer.GetReference(),
                choreReference = new ChoreReference(it.GetCurrentChore())
            })
            .ToArray();
    }

    public void LoadState(WorldState worldState) {
        var state = new ChoresState(worldState);

        foreach (var choreState in state.Chores) {
            var args = ChoreArgumentsWrapper.Unwrap(choreState.type, ArgumentUtils.UnWrapObjects(choreState.arguments));
            var chore = (Chore) choreState.type.GetConstructors()[0].Invoke(args);
            chore.Register(choreState.id);
        }

        foreach (var choreDriverState in state.Drivers) {
            var driver = choreDriverState.driverReference.Resolve();
            var chore = choreDriverState.choreReference.Resolve();
            var consumer = choreDriverState.consumerReference.Resolve();
            var choreContext = new Chore.Precondition.Context(
                chore,
                new ChoreConsumerState(consumer),
                is_attempting_override: false
            );
            driverChores.Set(driver, ref choreContext);
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
