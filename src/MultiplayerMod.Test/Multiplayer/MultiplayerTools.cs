using System.Collections;
using System.Collections.Generic;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Core.Scheduling;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Multiplayer.Commands.Registry;
using MultiplayerMod.Multiplayer.CoreOperations;
using MultiplayerMod.Multiplayer.CoreOperations.Binders;
using MultiplayerMod.Multiplayer.CoreOperations.CommandExecution;
using MultiplayerMod.Multiplayer.CoreOperations.PlayersManagement;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Players;
using MultiplayerMod.Multiplayer.World;
using MultiplayerMod.Test.Environment;
using MultiplayerMod.Test.Environment.Network;

namespace MultiplayerMod.Test.Multiplayer;

public static class MultiplayerTools {

    private static int nextPlayerId = 1;

    public static TestRuntime CreateTestRuntime(MultiplayerMode mode, string playerName) {
        var builder = new DependencyContainerBuilder()
            .AddType<EventDispatcher>()
            .AddType<ExecutionContextManager>()
            .AddType<ExecutionLevelManager>()
            .AddSingleton(new TestPlayerProfileProvider(new PlayerProfile(playerName)))
            .AddSingleton(new TestMultiplayerClientId(nextPlayerId++))
            .AddType<TestMultiplayerClient>()
            .AddType<TestMultiplayerServer>()
            .AddType<WorldManager>()
            .AddType<UnityTaskScheduler>()
            .AddType<MultiplayerIdentityProvider>()
            .AddType<MultiplayerGame>()
            .AddType<MultiplayerCommandExecutor>()
            .AddType<GameEventsBinder>()
            .AddType<HostEventsBinder>()
            .AddType<GameStateEventsRelay>()
            .AddType<ExecutionLevelController>()
            .AddType<MultiplayerServerController>()
            .AddType<MultiplayerGameObjectsSpawner>()
            .AddType<MultiplayerCommandController>()
            .AddType<MultiplayerJoinRequestController>()
            .AddType<PlayersManagementController>()
            .AddType<SpeedControlScreenContext>()
            .AddType<MultiplayerCommandRegistry>()
            .AddType<Recorders>()
            .AddType<TestRuntime>();

        new MultiplayerCommandsConfigurer().Configure(builder);
        new UnityTaskSchedulerConfigurer().Configure(builder);

        var container = builder.Build();
        var runtime = container.Get<TestRuntime>();
        var recorders = container.Get<Recorders>();
        var client = container.Get<TestMultiplayerClient>();
        var server = container.Get<TestMultiplayerServer>();
        var eventDispatcher = container.Get<EventDispatcher>();
        var multiplayer = container.Get<MultiplayerGame>();

        server.CommandReceived += (_, command) => recorders.ServerCommands.Add(command);
        client.CommandReceived += command => recorders.ClientCommands.Add(command);

        eventDispatcher.EventDispatching += e => recorders.Events.Add(e);
        runtime.Activated += it => { it.Dependencies.Get<SpeedControlScreenContext>().Apply(); };
        runtime.Deactivated += it => { it.Dependencies.Get<SpeedControlScreenContext>().Restore(); };

        multiplayer.Refresh(mode);
        return runtime;
    }

    public class TestPlayerProfileProvider : IPlayerProfileProvider {
        private readonly PlayerProfile profile;

        public TestPlayerProfileProvider(PlayerProfile profile) {
            this.profile = profile;
        }

        public PlayerProfile GetPlayerProfile() => profile;
    }

    public class Recorders {
        public Recorder Events { get; } = new();
        public Recorder ServerCommands { get; } = new();
        public Recorder ClientCommands { get; } = new();
    }

    public class Recorder : IReadOnlyList<object> {
        private readonly List<object> objects = new();
        public void Add(object obj) => objects.Add(obj);
        public IEnumerator<object> GetEnumerator() => objects.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public int Count => objects.Count;
        public object this[int index] => objects[index];
    }

}
