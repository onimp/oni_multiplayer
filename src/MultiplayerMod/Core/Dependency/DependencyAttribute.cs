using System;

namespace MultiplayerMod.Core.Dependency;

[AttributeUsage(AttributeTargets.Class)]
public class DependencyAttribute : Attribute {
    public string? Name { get; set; } = null;
    public bool Lazy { get; set; } = false;
}
