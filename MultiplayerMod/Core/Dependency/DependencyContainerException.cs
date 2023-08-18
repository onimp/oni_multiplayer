using System;

namespace MultiplayerMod.Core.Dependency;

public class DependencyContainerException : Exception {
    public DependencyContainerException(string message) : base(message) { }
}
