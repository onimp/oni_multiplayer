using System;
using System.Reflection;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.ModRuntime;

namespace MultiplayerMod.Test.Environment;

public class TestRuntime : Runtime {

    public event Action<Runtime>? Deactivated;
    public event Action<Runtime>? Activated;

    public TestRuntime(DependencyContainer container) : base(container) { }

    public void Activate() {
        var oldRuntime = Instance;
        var property = typeof(Runtime).GetProperty("Instance", BindingFlags.Static | BindingFlags.Public)!;
        property.SetValue(null, this);
        Deactivated?.Invoke(oldRuntime);
        Activated?.Invoke(this);
    }

}
