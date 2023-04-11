using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using dnlib.DotNet;
using Microsoft.Build.Framework;

namespace AssemblyExposer.MSBuild.Task;

public class ExposeAssembly : Microsoft.Build.Utilities.Task {

    public virtual ITaskItem[] SourceAssemblies { get; set; }
    public virtual string OutputDirectory { get; set; } = @"lib\exposed";
    public virtual ITaskItem[] Rules { get; set; }

    public override bool Execute() {
        if (SourceAssemblies == null) {
            Log.LogError($"Property {nameof(SourceAssemblies)} isn't set");
            return false;
        }
        if (Rules == null) {
            Log.LogError($"Property {nameof(Rules)} isn't set");
            return false;
        }

        try {
            WriteAssemblies();
        } catch (Exception e) {
            Log.LogErrorFromException(e);
            return false;
        }
        return true;
    }

    private void WriteAssemblies() {
        var rules = Rules.Select(it => new RewriteRule(it)).ToList();

        foreach (var assemblyItem in SourceAssemblies) {
            var sourceAssembly = assemblyItem.ItemSpec;

            if (!ProcessingRequired(sourceAssembly, rules))
                continue;

            var name = Path.GetFileName(sourceAssembly);
            Directory.CreateDirectory(OutputDirectory);
            Apply(sourceAssembly, rules).Write(Path.Combine(OutputDirectory, name));
            SaveState(sourceAssembly, rules);
        }
    }

    private string CalculateRulesHash(List<RewriteRule> rules) {
        using var algorithm = SHA1.Create();
        var data = rules.Aggregate(
            new StringBuilder(),
            (s, r) => s
                .Append("###")
                .Append(r.Pattern)
                .Append("###")
                .Append(r.Target.ToString())
                .Append("###")
                .Append(r.Visibility)
                .Append("###")
        ).ToString();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(data));
        return Convert.ToBase64String(algorithm.ComputeHash(stream));
    }

    private string CalculateFileHash(string fileName) {
        using var algorithm = SHA1.Create();
        using var stream = File.OpenRead(fileName);
        return Convert.ToBase64String(algorithm.ComputeHash(stream));
    }

    private string GetStateFileName(string assemblyPath) => Path.Combine(
        OutputDirectory,
        Path.GetFileName(assemblyPath) + ".state"
    );

    private bool ProcessingRequired(string assemblyFileName, List<RewriteRule> rules) {
        var stateFileName = GetStateFileName(Path.GetFileName(assemblyFileName));
        if (!File.Exists(stateFileName))
            return true;

        var hashes = File.ReadAllLines(stateFileName);
        return CalculateRulesHash(rules) != hashes[0] || CalculateFileHash(assemblyFileName) != hashes[1];
    }

    private void SaveState(string assemblyFileName, List<RewriteRule> rules) {
        File.WriteAllLines(
            GetStateFileName(assemblyFileName),
            new[] { CalculateRulesHash(rules), CalculateFileHash(assemblyFileName) }
        );
    }

    private ModuleDef Apply(string assemblyFileName, List<RewriteRule> rules) {
        Log.LogMessage($"Rewriting visibility for {assemblyFileName}");
        ModuleDef assembly = ModuleDefMD.Load(assemblyFileName);
        foreach (var type in assembly.GetTypes()) {
            ApplyRules(rules, type);
            foreach (var method in type.Methods)
                ApplyRules(rules, method);
            foreach (var field in type.Fields) {
                if (type.HasEvents) {
                    var eventDef = type.FindEvent(field.Name);
                    if (eventDef != null) {
                        Log.LogMessage($"Ignoring field '{field.FullName}' as it's related to the event '{eventDef.FullName}'");
                        continue;
                    }
                }
                ApplyRules(rules, field);
            }
        }
        return assembly;
    }

    private void ApplyRules(List<RewriteRule> rules, IMemberDef def) {
        var rule = rules.FirstOrDefault(rule => rule.Matches(def));
        if (rule == null)
            return;

        def.ApplyRule(rule);
        Log.LogMessage($"Rule '{rule.Name}' applied to '{def.ExtendedName()}'");
    }

}
