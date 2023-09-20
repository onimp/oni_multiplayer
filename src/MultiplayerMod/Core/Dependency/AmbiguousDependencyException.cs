using System;
using System.Collections.Generic;

namespace MultiplayerMod.Core.Dependency;

public class AmbiguousDependencyException : Exception {

    public AmbiguousDependencyException(Type type, ICollection<DependencyInfo> infos) : base(
        $"Ambiguous dependencies of type \"{type}\": found {infos.Count} instances"
    ) { }

}
