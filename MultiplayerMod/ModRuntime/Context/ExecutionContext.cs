namespace MultiplayerMod.ModRuntime.Context;

public class ExecutionContext {

    public ExecutionLevel Level { get; }

    public ExecutionContext(ExecutionLevel level) {
        Level = level;
    }

}
