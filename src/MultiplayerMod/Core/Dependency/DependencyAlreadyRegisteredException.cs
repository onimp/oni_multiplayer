using System;

namespace MultiplayerMod.Core.Dependency;

public class DependencyAlreadyRegisteredException : Exception {
    public DependencyAlreadyRegisteredException(string name) : base($"Dependency \"{name}\" is already registered") { }
}
