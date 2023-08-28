using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using dnlib.DotNet;
using Microsoft.Build.Framework;

namespace AssemblyExposer.MSBuild.Task;

public class RewriteRule {

    public string Name { get; }
    public Regex Pattern { get; }
    public TargetOption Target { get; }
    public VisibilityOption Visibility { get; }

    public RewriteRule(ITaskItem item) {
        Name = item.ItemSpec;
        var patternSpec = GetSpec(item, nameof(Pattern));
        var typeSpec = GetSpec(item, nameof(Target));
        var visibilitySpec = GetSpec(item, nameof(Visibility));

        Pattern = GetRegexp(patternSpec);
        Target = typeSpec.Split(',').Select(it => it.Trim()).Aggregate(
            TargetOption.None,
            (acc, t) => {
                var success = Enum.TryParse<TargetOption>(t, out var result);
                if (!success)
                    throw new Exception($"Target '{t}' in {item.ItemSpec} is invalid");

                return acc | result;
            }
        );
        var success = Enum.TryParse<VisibilityOption>(visibilitySpec, out var visibility);
        if (!success)
            throw new Exception($"Visibility '{visibilitySpec}' in {item.ItemSpec} is invalid");

        Visibility = visibility;
    }

    private static Regex GetRegexp(string patternSpec) {
        StringBuilder pattern = new StringBuilder();
        if (!patternSpec.StartsWith("^"))
            pattern.Append(".*?");
        pattern.Append(patternSpec);
        if (!patternSpec.EndsWith("$"))
            pattern.Append(".*");
        return new Regex(pattern.ToString(), RegexOptions.Compiled);
    }

    private string GetSpec(ITaskItem item, string name) {
        var fullName = $"{name}Rule";
        if (item.MetadataNames.Cast<string>().All(it => it != fullName))
            throw new Exception($"Property '{fullName}' not found in '{item.ItemSpec}'");

        return item.GetMetadata(fullName);
    }

    public bool Matches(IMemberDef def) {
        var target = def switch {
            TypeDef => TargetOption.Type,
            MethodDef => TargetOption.Method,
            FieldDef => TargetOption.Field,
            _ => TargetOption.None
        };
        return Target.HasFlag(target) && Pattern.IsMatch(def.ExtendedName());
    }

}
