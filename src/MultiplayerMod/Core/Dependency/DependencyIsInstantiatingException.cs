using System;

namespace MultiplayerMod.Core.Dependency;

public class DependencyIsInstantiatingException : Exception {

    private readonly DependencyInfo info;

    public DependencyIsInstantiatingException(DependencyInfo info) : base(
        $"A dependency \"{info.Name}\" is currently instantiating (circular dependency?)"
    ) {
        this.info = info;
    }

}
