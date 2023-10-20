using System.Collections.Generic;
using System.IO;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Writer;
using JetBrains.Annotations;
using MultiplayerMod.AttributeProcessor.MSBuild.Task.Processors;

namespace MultiplayerMod.AttributeProcessor.MSBuild.Task;

[UsedImplicitly]
public class ProcessAttributes : Microsoft.Build.Utilities.Task {

    public virtual string? AssemblyPath { get; set; }

    public override bool Execute() {
        if (AssemblyPath == null)
            return ReturnError($"\"{nameof(AssemblyPath)}\" is not set");

        if (!File.Exists(AssemblyPath))
            return ReturnError($"Assembly \"{nameof(AssemblyPath)}\" doesn't exist");

        var configuration = Configure(AssemblyPath);
        FileStream OriginalAssemblyFile = new FileStream(configuration.OriginalAssemblyPath, FileMode.Open);
        var module = ModuleDefMD.Load(OriginalAssemblyFile);
        OriginalAssemblyFile.Close();
        if (GetProcessors(module).Any(processor => !processor.Process()))
            return false;

        var options = new ModuleWriterOptions(module) { WritePdb = true };
        module.Write(AssemblyPath, options);
        Cleanup(configuration);
        return true;
    }

    private Configuration Configure(string assemblyPath) {
        var assemblyPathWithoutExtension = Path.Combine(
            // ReSharper disable once AssignNullToNotNullAttribute
            Path.GetDirectoryName(assemblyPath),
            Path.GetFileNameWithoutExtension(assemblyPath)
        );
        var originalAssemblyPath = $"{assemblyPathWithoutExtension}.Original.dll";
        var originalAssemblyPdbPath = $"{assemblyPathWithoutExtension}.Original.pdb";

        File.Copy(assemblyPath, originalAssemblyPath, true);

        var assemblyPdbPath = $"{assemblyPathWithoutExtension}.pdb";
        if (File.Exists(assemblyPdbPath))
            File.Copy(assemblyPdbPath, originalAssemblyPdbPath, true);

        return new Configuration(originalAssemblyPath, originalAssemblyPdbPath);
    }

    private static void Cleanup(Configuration configuration) {
        File.Delete(configuration.OriginalAssemblyPath);
        File.Delete(configuration.OriginalPbdPath);
    }

    private List<IAttributeProcessor> GetProcessors(ModuleDefMD module) => new() {
        new ConditionalInvocationProcessor(module, Log)
    };

    private bool ReturnError(string message) {
        Log.LogError(message);
        return false;
    }

    private record Configuration(string OriginalAssemblyPath, string OriginalPbdPath);

}
