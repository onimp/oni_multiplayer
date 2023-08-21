namespace MultiplayerMod.ModRuntime.Context;

public static class ExecutionContextManagerExtensions {

    public static void UsingLevel(this ExecutionContextManager manager, ExecutionLevel level, System.Action action) {
        manager.UsingContext(new ExecutionContext(level), action);
    }

}
