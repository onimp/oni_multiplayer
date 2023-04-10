using System;

namespace MultiplayerMod.Core.Dependency;

public class ContainerException : Exception {
    public ContainerException(string message) : base(message) { }
}
