using System;

namespace MultiplayerMod.Core.Dependency;

[Flags]
public enum DependencyOptions {

    /// Resolves the dependency automatically during registration.
    AutoResolve = 1,

    /// Default options.
    Default = 0

}
