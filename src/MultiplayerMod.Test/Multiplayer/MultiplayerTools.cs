using System.Collections;
using System.Collections.Generic;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Scheduling;
using MultiplayerMod.Multiplayer;
using MultiplayerMod.Multiplayer.Configuration;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Players;
using MultiplayerMod.Network;
using MultiplayerMod.Test.Environment;
using MultiplayerMod.Test.Environment.Network;

namespace MultiplayerMod.Test.Multiplayer;

public static class MultiplayerTools {

    private static int nextPlayerId = 1;

    public static TestRuntime CreateTestRuntime(MultiplayerMode mode, string playerName) {
        var clientId = new TestMultiplayerClientId(nextPlayerId++);
        var runtime = new TestRuntime();
        var dependencies = runtime.Dependencies;

        dependencies.Register<IPlayerProfileProvider>(new TestPlayerProfileProvider(new PlayerProfile(playerName)));

        var client = new TestMultiplayerClient(clientId);
        dependencies.Register(client);
        dependencies.Register<IMultiplayerClient>(client);

        var server = new TestMultiplayerServer(clientId);
        dependencies.Register(server);
        dependencies.Register<IMultiplayerServer>(server);

        dependencies.Register<UnityTaskScheduler>();
        dependencies.Register<MultiplayerIdentityProvider>();
        dependencies.Register<MultiplayerGame>();
        dependencies.Register<PlayerConnectionManager>(DependencyOptions.AutoResolve);
        dependencies.Register<MultiplayerCoordinator>(DependencyOptions.AutoResolve);

        dependencies.Register<SpeedControlScreenContext>();
        runtime.Activated += it => { it.Dependencies.Get<SpeedControlScreenContext>().Apply(); };
        runtime.Deactivated += it => { it.Dependencies.Get<SpeedControlScreenContext>().Restore(); };

        runtime.Multiplayer.Mode = mode;

        var recorders = new Recorders();
        dependencies.Register(recorders);

        runtime.EventDispatcher.EventDispatching += e => recorders.Events.Add(e);
        server.CommandReceived += (_, command) => recorders.ServerCommands.Add(command);
        client.CommandReceived += command => recorders.ClientCommands.Add(command);

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
