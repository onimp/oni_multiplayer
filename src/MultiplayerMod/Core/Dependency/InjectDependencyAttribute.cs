using System;

namespace MultiplayerMod.Core.Dependency;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class InjectDependencyAttribute : Attribute { }
