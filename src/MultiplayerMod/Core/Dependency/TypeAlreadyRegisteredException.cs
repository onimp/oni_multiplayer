using System;

namespace MultiplayerMod.Core.Dependency;

public class TypeAlreadyRegisteredException : Exception {
    public TypeAlreadyRegisteredException(Type type) : base($"A type {type} is already registered") { }
}
