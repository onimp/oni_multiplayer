using System;

namespace MultiplayerMod.Core.Dependency;

public class InvalidDependencyException : Exception {
    public InvalidDependencyException(string message) : base(message) { }
}
