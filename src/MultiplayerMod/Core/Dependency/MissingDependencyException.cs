using System;

namespace MultiplayerMod.Core.Dependency;

public class MissingDependencyException : Exception {
    public MissingDependencyException(Type type) : base($"Dependency of type \"{type}\" not found") { }
}
