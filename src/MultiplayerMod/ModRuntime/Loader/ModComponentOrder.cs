using System;

namespace MultiplayerMod.ModRuntime.Loader;

[AttributeUsage(AttributeTargets.Class)]
public class ModComponentOrder : Attribute {

    public const int Runtime = 0;
    public const int Platform = 1;
    public const int Default = 2;
    public const int Configuration = 3;

    public int Order { get; }

    public ModComponentOrder(int order) {
        Order = order;
    }

}
