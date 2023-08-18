using System;

namespace MultiplayerMod.Core.Loader;

[AttributeUsage(AttributeTargets.Class)]
public class ModComponentOrder : Attribute {

    public const int Platform = -10000;
    public const int Default = 0;
    public const int Configuration = 10000;

    public int Order { get; }

    public ModComponentOrder(int order) {
        Order = order;
    }

}
