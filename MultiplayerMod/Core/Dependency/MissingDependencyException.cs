using System;

namespace MultiplayerMod.Core.Dependency;

public class MissingDependencyException : Exception {
    public MissingDependencyException(string message) : base(message) { }
}
