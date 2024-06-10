using System;
using MultiplayerMod.Core.Collections;

namespace MultiplayerMod.Core.Dependency;

public interface IDependencyContainer {
    T Get<T>() where T : notnull;
    Deconstructable<T1, T2> Get<T1, T2>() where T1 : notnull where T2 : notnull;
    object Get(Type type);
}
