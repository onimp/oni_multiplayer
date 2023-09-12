using MultiplayerMod.Multiplayer.CoreOperations.Events;
using MultiplayerMod.Network;
using MultiplayerMod.Test.Environment;
using MultiplayerMod.Test.Environment.Network;

namespace MultiplayerMod.Test.Multiplayer;

public static class TestRuntimeExtensions {

    public static void StartGame(this TestRuntime runtime) {
        runtime.Activate();
        runtime.EventDispatcher.Dispatch(new GameStartedEvent(runtime.Multiplayer));
    }

    public static void ConnectTo(this TestRuntime runtime, TestRuntime hostRuntime) {
        runtime.Activate();
        var client = runtime.Dependencies.Get<TestMultiplayerClient>();
        client.Connect(hostRuntime.Dependencies.Get<IMultiplayerServer>().Endpoint);
    }

}
