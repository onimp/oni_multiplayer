using System;

namespace MultiplayerMod.Core.Dependency;

public class DependencyInstantiationException : Exception {

    private readonly DependencyInfo info;

    public DependencyInstantiationException(DependencyInfo info, Exception cause) : base(
        message: $"Unable to instantiate a dependency \"{info.Name}\"",
        innerException: cause
    ) {
        this.info = info;
    }

}
