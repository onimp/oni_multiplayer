using System;
using System.Reflection;
using MultiplayerMod.ModRuntime;

namespace MultiplayerMod.Test.Environment;

public class TestRuntime : Runtime {

    public event Action<Runtime>? Deactivated;
    public event Action<Runtime>? Activated;

    public void Activate() {
        var oldRuntime = Instance;
        var property = typeof(Runtime).GetProperty("Instance", BindingFlags.Static | BindingFlags.Public)!;
        property.SetValue(null, this);
        Deactivated?.Invoke(oldRuntime);
        Activated?.Invoke(this);
    }

}
