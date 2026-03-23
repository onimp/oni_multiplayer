using ProcGenGame;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Klei;
using UnityEngine;

namespace DedicatedServer.Game;

/// <summary>
/// Loads game resources: elements from YAML, personalities/modifiers from Unity asset binaries.
/// </summary>
public class ResourceLoader {

    private static readonly string GameStreamingAssetsPath = GetRequiredEnvPath("ONI_STREAMING_ASSETS",
        "Path to ONI StreamingAssets (e.g. .../OxygenNotIncluded_Data/StreamingAssets)");

    public static string StreamingAssetsPath => GameStreamingAssetsPath;
    public static string DataPath => Path.GetDirectoryName(GameStreamingAssetsPath)!;
    public string PersonalitiesCsv { get; private set; } = "";
    public string ModifiersCsv { get; private set; } = "";
    public string ResearchTreeExpansion1Xml { get; private set; } = "";
    public string ResearchTreeVanillaXml { get; private set; } = "";

    public void Load() {
        Console.WriteLine("[Resources] Loading elements from YAML...");
        LoadElements();
        Console.WriteLine("[Resources] Loading data from game assets...");
        PersonalitiesCsv = ExtractCsvFromAsset("sharedassets0.assets", "Name,Gender,Model,RequiredDlcId");
        ModifiersCsv = ExtractCsvFromAsset("resources.assets", "Id,Type,Attribute,Value");
        var xmls = ExtractAllXmlFromAsset("resources.assets", "<graphml>", "</graphml>");
        if (xmls.Count >= 2) {
            ResearchTreeExpansion1Xml = xmls[0]; // first has NuclearResearch
            ResearchTreeVanillaXml = xmls[1];
        } else if (xmls.Count == 1) {
            ResearchTreeVanillaXml = xmls[0];
            ResearchTreeExpansion1Xml = xmls[0];
        }
        Console.WriteLine($"[Resources] Personalities: {PersonalitiesCsv.Split('\n').Length} lines, Modifiers: {ModifiersCsv.Split('\n').Length} lines, ResearchTrees: {xmls.Count}");
    }

    private void LoadElements() {
        var elementsPath = GameStreamingAssetsPath + "/elements/";
        if (!Directory.Exists(elementsPath)) {
            Console.WriteLine($"[Resources] Elements path not found: {elementsPath}");
            return;
        }

        var entries = new List<ElementLoader.ElementEntry>();
        foreach (var yamlFile in Directory.GetFiles(elementsPath, "*.yaml")) {
            if (Path.GetFileName(yamlFile).StartsWith(".")) continue;
            try {
                var collection = YamlIO.LoadFile<ElementLoader.ElementEntryCollection>(yamlFile, null);
                if (collection?.elements != null) entries.AddRange(collection.elements);
            } catch (Exception ex) {
                Console.WriteLine($"[Resources] Failed to load {Path.GetFileName(yamlFile)}: {ex.Message}");
                BootDiagnostics.Record();
            }
        }

        ElementLoader.elements = new List<Element>();
        ElementLoader.elementTable = new Dictionary<int, Element>();

        foreach (var entry in entries) {
            var hash = Hash.SDBMLower(entry.elementId);
            if (ElementLoader.elementTable.ContainsKey(hash)) continue;

            var element = new Element {
                id = (SimHashes)hash,
                name = entry.elementId,
                nameUpperCase = entry.elementId.ToUpper(),
                tag = TagManager.Create(entry.elementId, entry.elementId),
                dlcId = entry.dlcId,
                specificHeatCapacity = entry.specificHeatCapacity,
                thermalConductivity = entry.thermalConductivity,
                molarMass = entry.molarMass,
                strength = entry.strength,
                disabled = entry.isDisabled,
                flow = entry.flow,
                maxMass = entry.maxMass,
                maxCompression = entry.liquidCompression,
                viscosity = entry.speed,
                minHorizontalFlow = entry.minHorizontalFlow,
                minVerticalFlow = entry.minVerticalFlow,
                solidSurfaceAreaMultiplier = entry.solidSurfaceAreaMultiplier,
                liquidSurfaceAreaMultiplier = entry.liquidSurfaceAreaMultiplier,
                gasSurfaceAreaMultiplier = entry.gasSurfaceAreaMultiplier,
                state = entry.state,
                hardness = entry.hardness,
                lowTemp = entry.lowTemp,
                lowTempTransitionTarget = (SimHashes)Hash.SDBMLower(entry.lowTempTransitionTarget),
                highTemp = entry.highTemp,
                highTempTransitionTarget = (SimHashes)Hash.SDBMLower(entry.highTempTransitionTarget),
                highTempTransitionOreID = (SimHashes)Hash.SDBMLower(entry.highTempTransitionOreId),
                highTempTransitionOreMassConversion = entry.highTempTransitionOreMassConversion,
                lowTempTransitionOreID = (SimHashes)Hash.SDBMLower(entry.lowTempTransitionOreId),
                lowTempTransitionOreMassConversion = entry.lowTempTransitionOreMassConversion,
                defaultValues = new Sim.PhysicsData {
                    temperature = entry.defaultTemperature,
                    mass = entry.defaultTemperature > 0 ? (entry.defaultMass > 0 ? entry.defaultMass : 1f) : 0f
                },
                substance = new Substance {
                    elementID = (SimHashes)hash,
                    nameTag = TagManager.Create(entry.elementId, entry.elementId),
                    anim = new KAnimFile { IsBuildLoaded = true }
                }
            };
            // Mirror ElementLoader.CopyEntryToElement (line 283-287):
            // Gas element YAMLs have no maxMass field → entry.maxMass=0.
            // Real game hardcodes 1.8f for all gas elements, and forces defaultValues.mass=1f.
            // Without this, SimDLL rejects ALL ModifyCell(gas) calls with "max mass = 0"
            // → template O2 placement rejected → starting cave stays Vacuum.
            if (element.IsGas) {
                element.maxMass = 1.8f;
                element.defaultValues = new Sim.PhysicsData {
                    temperature = element.defaultValues.temperature,
                    mass = 1f
                };
            }
            ElementLoader.elements.Add(element);
            ElementLoader.elementTable[hash] = element;
        }

        FinalizeElements();
        WorldGen.SetupDefaultElements();
        var o2 = ElementLoader.FindElementByHash(SimHashes.Oxygen);
        var co2 = ElementLoader.FindElementByHash(SimHashes.CarbonDioxide);
        Console.WriteLine($"[Resources] Registered {ElementLoader.elements.Count} elements. O2.maxMass={o2?.maxMass ?? -1f} CO2.maxMass={co2?.maxMass ?? -1f} (must be 1.8 for template placement)"  );
    }

    private static void FinalizeElements() {
        foreach (var elem in ElementLoader.elements) {
            if (elem.thermalConductivity == 0f) elem.state |= Element.State.TemperatureInsulated;
            if (elem.strength == 0f) elem.state |= Element.State.Unbreakable;
        }
        ElementLoader.elements = ElementLoader.elements
            .OrderByDescending(e => (int)(e.state & Element.State.Solid))
            .ThenBy(e => e.id).ToList();

        ElementLoader.elementTable.Clear();
        ElementLoader.elementTagTable = new Dictionary<Tag, Element>();
        for (var i = 0; i < ElementLoader.elements.Count; i++) {
            var elem = ElementLoader.elements[i];
            elem.idx = (ushort)i;
            if (elem.substance != null) elem.substance.idx = i;
            ElementLoader.elementTable[(int)elem.id] = elem;
            ElementLoader.elementTagTable[elem.tag] = elem;
        }
        foreach (var elem in ElementLoader.elements) {
            if (elem.IsSolid) elem.highTempTransition = ElementLoader.FindElementByHash(elem.highTempTransitionTarget);
            else if (elem.IsLiquid) {
                elem.highTempTransition = ElementLoader.FindElementByHash(elem.highTempTransitionTarget);
                elem.lowTempTransition = ElementLoader.FindElementByHash(elem.lowTempTransitionTarget);
            } else if (elem.IsGas) {
                elem.lowTempTransition = ElementLoader.FindElementByHash(elem.lowTempTransitionTarget);
            }
        }
    }

    private string ExtractCsvFromAsset(string assetFileName, string headerSignature) {
        try {
            var assetPath = Path.Combine(DataPath, assetFileName);
            if (!File.Exists(assetPath)) {
                Console.WriteLine($"[Resources] {assetFileName} not found");
                return "";
            }
            var data = File.ReadAllBytes(assetPath);
            var headerBytes = System.Text.Encoding.UTF8.GetBytes(headerSignature);
            var idx = FindBytes(data, headerBytes);
            if (idx == -1) {
                Console.WriteLine($"[Resources] CSV header not found in {assetFileName}");
                return "";
            }
            var end = idx;
            while (end < data.Length && data[end] != 0) end++;
            return System.Text.Encoding.UTF8.GetString(data, idx, end - idx);
        } catch (Exception ex) {
            Console.WriteLine($"[Resources] Failed to extract from {assetFileName}: {ex.Message}");
            BootDiagnostics.Record();
            return "";
        }
    }

    private List<string> ExtractAllXmlFromAsset(string assetFileName, string startTag, string endTag) {
        var results = new List<string>();
        var assetPath = Path.Combine(DataPath, assetFileName);
        if (!File.Exists(assetPath)) return results;
        var data = File.ReadAllBytes(assetPath);
        var startBytes = System.Text.Encoding.UTF8.GetBytes(startTag);
        var endBytes = System.Text.Encoding.UTF8.GetBytes(endTag);
        int offset = 0;
        while (offset < data.Length) {
            var idx = FindBytes(data, startBytes, offset);
            if (idx == -1) break;
            var endIdx = FindBytes(data, endBytes, idx + startBytes.Length);
            if (endIdx == -1) break;
            endIdx += endBytes.Length;
            results.Add(System.Text.Encoding.UTF8.GetString(data, idx, endIdx - idx));
            offset = endIdx;
        }
        return results;
    }

    private static int FindBytes(byte[] haystack, byte[] needle, int startOffset = 0) {
        for (int i = startOffset; i <= haystack.Length - needle.Length; i++) {
            bool found = true;
            for (int j = 0; j < needle.Length; j++) {
                if (haystack[i + j] != needle[j]) { found = false; break; }
            }
            if (found) return i;
        }
        return -1;
    }

    private static string GetRequiredEnvPath(string envVar, string description) {
        var path = Environment.GetEnvironmentVariable(envVar);
        if (string.IsNullOrEmpty(path))
            throw new InvalidOperationException($"Environment variable {envVar} is required. {description}");
        if (!Directory.Exists(path))
            throw new DirectoryNotFoundException($"{envVar}={path} — directory not found");
        return path;
    }
}
