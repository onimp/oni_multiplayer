using System;

namespace MultiplayerMod.Multiplayer.StateMachines.Configuration.Parameters;

public class StateMachineMultiplayerParameterInfo<T>(string name, bool shared = true, T? defaultValue = default) {
    public string Name { get; } = name;
    public Type ValueType { get; } = typeof(T);
    public bool Shared { get; } = shared;
    public object? DefaultValue { get; } = defaultValue;
}
