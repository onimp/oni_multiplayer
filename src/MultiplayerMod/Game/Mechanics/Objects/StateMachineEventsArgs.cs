using System.Reflection;

namespace MultiplayerMod.Game.Mechanics.Objects;

public record StateMachineEventsArgs(StateMachine.Instance StateMachineInstance, MethodBase Method, object[] Args);
