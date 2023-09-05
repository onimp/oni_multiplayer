using MultiplayerMod.Game;
using MultiplayerMod.Network;
using MultiplayerMod.Test.Environment;
using MultiplayerMod.Test.Environment.Network;

namespace MultiplayerMod.Test.Multiplayer;

public static class TestRuntimeExtensions {

    public static void StartGame(this TestRuntime runtime) {
        runtime.Activate();
        typeof(GameEvents).RaiseEvent(nameof(GameEvents.GameStarted), runtime);
    }

    public static void ConnectTo(this TestRuntime runtime, TestRuntime hostRuntime) {
        runtime.Activate();
        var client = runtime.Dependencies.Get<TestMultiplayerClient>();
        client.Connect(hostRuntime.Dependencies.Get<IMultiplayerServer>().Endpoint);
    }

}
