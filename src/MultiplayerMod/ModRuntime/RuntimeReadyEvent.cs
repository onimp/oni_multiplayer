using MultiplayerMod.Core.Events;

namespace MultiplayerMod.ModRuntime;

public record RuntimeReadyEvent(Runtime Runtime) : IDispatchableEvent;
