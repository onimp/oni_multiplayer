using System;

namespace AssemblyExposer.MSBuild.Task;

[Flags]
public enum TargetOption {
    None = 0,
    Type = 1,
    Method = 2,
    Field = 4,
    All = 7
}
