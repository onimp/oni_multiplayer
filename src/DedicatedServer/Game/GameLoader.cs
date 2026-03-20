using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using Database;
using HarmonyLib;
using Klei;
using MultiplayerMod.Test.Environment.Patches;
using MultiplayerMod.Test.Environment.Unity;
using MultiplayerMod.Test.GameRuntime.Patches;
using ProcGen;
using ProcGenGame;
using UnityEngine;
using Path = System.IO.Path;
using Random = System.Random;

namespace DedicatedServer.Game;

/// <summary>
/// Boots the ONI game world from DLLs with real WorldGen and SimDLL physics.
/// </summary>
public class GameLoader {

    /// <summary>
    /// Path to the game's StreamingAssets directory.
    /// Required env variable: ONI_STREAMING_ASSETS
    /// </summary>
    private static readonly string GameStreamingAssetsPath = GetRequiredEnvPath("ONI_STREAMING_ASSETS",
        "Path to ONI StreamingAssets (e.g. .../OxygenNotIncluded_Data/StreamingAssets)");

    private static string GetRequiredEnvPath(string envVar, string description) {
        var path = Environment.GetEnvironmentVariable(envVar);
        if (string.IsNullOrEmpty(path))
            throw new InvalidOperationException($"Environment variable {envVar} is required. {description}");
        if (!Directory.Exists(path))
            throw new DirectoryNotFoundException($"{envVar}={path} — directory not found");
        return path;
    }

    private Harmony harmony = null!;
    private int width;
    private int height;

    public int Width => width;
    public int Height => height;
    public bool IsLoaded { get; private set; }
    public bool SimRunning { get; private set; }
    public int SimTick { get; private set; }
    public ProcGenGame.GameSpawnData SpawnData { get; private set; }

    // GC handles to keep pinned arrays alive for the server lifetime
    private static GCHandle elementIdxHandle;
    private static GCHandle temperatureHandle;
    private static GCHandle radiationHandle;
    private static GCHandle massHandle;

    public void Boot() {
        Console.WriteLine("[GameLoader] Installing patches...");

        // Pre-set MonoMod's platform detection to avoid DeterminePlatform() hanging.
        // On macOS under Rosetta, DeterminePlatform() spawns a process (uname) and
        // StreamReader.ReadLine() blocks forever reading its stdout.
        // Setting Current before any Harmony use skips the broken auto-detection.
        try {
            var platformHelper = typeof(Harmony).Assembly.GetType("MonoMod.Utils.PlatformHelper");
            var currentProp = platformHelper?.GetProperty("Current", BindingFlags.Public | BindingFlags.Static);
            if (currentProp?.GetSetMethod() != null) {
                var platformEnum = typeof(Harmony).Assembly.GetType("MonoMod.Utils.Platform");
                // MacOS = 73 (OS | Unix | MacOS-specific bits)
                currentProp.SetValue(null, Enum.ToObject(platformEnum, 73));
                Console.WriteLine("[GameLoader] Pre-set MonoMod platform to MacOS");
            }
        } catch (Exception ex) {
            Console.WriteLine($"[GameLoader] Platform pre-set failed (non-fatal): {ex.Message}");
        }

        System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(System.Reflection.Emit.DynamicMethod).TypeHandle);
        System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(System.Reflection.Emit.ILGenerator).TypeHandle);
        harmony = new Harmony("DedicatedServer");
        InstallPatches();

        Console.WriteLine("[GameLoader] Fixing streaming assets path...");
        FixStreamingAssetsPath();

        Console.WriteLine("[GameLoader] Loading elements from game YAML...");
        LoadElementsFromGame();

        // Temporary grid size for game system init — overridden by WorldGen settings
        width = 256;
        height = 384;

        Console.WriteLine("[GameLoader] Initializing game world...");
        InitializeWorld();

        Console.WriteLine("[GameLoader] Loading WorldGen settings...");
        LoadWorldGenSettings();

        Console.WriteLine("[GameLoader] Generating world...");
        var cells = GenerateWorld();

        if (cells == null) {
            throw new Exception("WorldGen failed — cannot start server without a generated world");
        }

        Console.WriteLine("[GameLoader] Initializing SimDLL with WorldGen cells...");
        InitSimDLL(cells.Value.cells, cells.Value.bgTemp, cells.Value.dc);

        IsLoaded = true;
        Console.WriteLine($"[GameLoader] World ready: {width}x{height} ({width * height} cells), SimDLL: {SimRunning}");
    }

    private void InstallPatches() {
        // Unity patches from test assembly
        var unityPatchTypes = typeof(UnityTestRuntime).Assembly.GetTypes()
            .Where(type => type.Namespace?.StartsWith(typeof(UnityTestRuntime).Namespace + ".Patches") == true)
            .ToList();

        Console.WriteLine($"[GameLoader] Found {unityPatchTypes.Count} Unity patch types");

        foreach (var patchType in unityPatchTypes) {
            try {
                Console.WriteLine($"[GameLoader]   Patching {patchType.Name}...");
                Console.Out.Flush();
                harmony.CreateClassProcessor(patchType).Patch();
            } catch (Exception ex) {
                Console.WriteLine($"[GameLoader] Patch FAIL: {patchType.Name}: {ex.Message}");
            }
        }

        // Game-specific patches
        var gamePatches = new[] {
            typeof(DbPatch),
            typeof(AssetsPatch),
            typeof(SensorsPatch),
            typeof(ChoreConsumerStatePatch)
        };
        foreach (var patchType in gamePatches) {
            try {
                harmony.CreateClassProcessor(patchType).Patch();
            } catch (Exception ex) {
                Console.WriteLine($"[GameLoader] Patch FAIL: {patchType.Name}: {ex.Message}");
            }
        }
        // NOTE: We intentionally skip ElementLoaderPatch — we want real FindElementByHash

        // Override DebugLogHandlerPatch — it throws on LogError, which kills WorldGen
        // (WorldGen uses Debug.Assert which logs errors for non-fatal conditions)
        var logMethod = typeof(DebugLogHandler).GetMethod(
            nameof(DebugLogHandler.LogFormat),
            new[] { typeof(LogType), typeof(UnityEngine.Object), typeof(string), typeof(object[]) }
        );
        if (logMethod != null) {
            harmony.Patch(logMethod,
                prefix: new HarmonyMethod(typeof(GameLoader), nameof(SoftDebugLogHandler)) { priority = Priority.First });
        }

        // Patch ReportWorldGenError to not crash on GenericGameSettings.instance == null
        var reportMethod = typeof(WorldGen).GetMethod("ReportWorldGenError", BindingFlags.Public | BindingFlags.Instance);
        if (reportMethod != null) {
            harmony.Patch(reportMethod,
                prefix: new HarmonyMethod(typeof(GameLoader), nameof(SafeReportWorldGenError)));
        }

        // Provide GenericGameSettings._instance to prevent NPEs in WorldGen error reporting
        var ggsField = typeof(GenericGameSettings).GetField("_instance", BindingFlags.NonPublic | BindingFlags.Static);
        if (ggsField != null && ggsField.GetValue(null) == null) {
            ggsField.SetValue(null, new GenericGameSettings());
        }

        // Patch ManifestSubstanceForElement — SubstanceTable requires Unity textures,
        // we just create a stub Substance for each element instead
        var manifestMethod = AccessTools.Method(typeof(ElementLoader), "ManifestSubstanceForElement");
        if (manifestMethod != null) {
            harmony.Patch(manifestMethod,
                prefix: new HarmonyMethod(typeof(GameLoader), nameof(StubManifestSubstance)));
            Console.WriteLine("[GameLoader] Patched ManifestSubstanceForElement");
        } else {
            Console.WriteLine("[GameLoader] WARNING: ManifestSubstanceForElement not found!");
        }
    }

    /// <summary>
    /// Non-throwing debug log handler. The test DebugLogHandlerPatch throws on LogError,
    /// which is fatal for WorldGen (it uses Debug.Assert for non-fatal warnings).
    /// </summary>
    private static bool SoftDebugLogHandler(LogType logType, string format, object[] args) {
        var message = string.Format(format, args);
        switch (logType) {
            case LogType.Error:
                Console.WriteLine($"[ERROR] {message}");
                break;
            case LogType.Warning:
                Console.WriteLine($"[WARNING] {message}");
                break;
            default:
                Console.WriteLine($"[INFO] {message}");
                break;
        }
        return false; // skip original
    }

    /// <summary>
    /// Fix ElementLoader.path before CollectElementsFromYAML reads it.
    /// </summary>
    private static void FixElementPath() {
        var field = AccessTools.Field(typeof(ElementLoader), "path");
        Console.WriteLine($"[FixElementPath] field={field}, current='{field?.GetValue(null)}'");
        Console.WriteLine($"[FixElementPath] streamingAssetsPath='{Application.streamingAssetsPath}'");
        if (field != null) {
            var newPath = Application.streamingAssetsPath + "/elements/";
            field.SetValue(null, newPath);
            Console.WriteLine($"[FixElementPath] set to '{field.GetValue(null)}'");
        }
    }

    /// <summary>
    /// Stub substance creation — replaces ManifestSubstanceForElement which needs Unity textures.
    /// </summary>
    private static bool StubManifestSubstance(Element elem, ref Hashtable substanceList) {
        if (substanceList.ContainsKey(elem.id)) {
            elem.substance = substanceList[elem.id] as Substance;
        } else {
            elem.substance = new Substance();
            elem.substance.elementID = elem.id;
            elem.substance.renderedByWorld = elem.IsSolid;
            elem.substance.idx = substanceList.Count;
            elem.substance.nameTag = elem.tag;
            elem.substance.anim = new KAnimFile { IsBuildLoaded = true };
            substanceList[elem.id] = elem.substance;
        }
        return false; // skip original
    }

    /// <summary>
    /// Safe WorldGen error reporter — logs to console instead of crashing.
    /// </summary>
    private static bool SafeReportWorldGenError(Exception e, string errorMessage) {
        Console.WriteLine($"[WorldGen] ERROR: {errorMessage ?? "WorldGen failure"}");
        Console.WriteLine($"[WorldGen] Exception: {e?.GetType().Name}: {e?.Message}");
        if (e?.StackTrace != null)
            Console.WriteLine($"[WorldGen] Stack: {e.StackTrace.Split('\n').FirstOrDefault()}");
        return false; // skip original (which crashes on GenericGameSettings.instance)
    }

    /// <summary>
    /// Override Application.streamingAssetsPath to point to the real game's StreamingAssets.
    /// The test patches set it to "" — we use a high-priority Prefix to override.
    /// </summary>
    private void FixStreamingAssetsPath() {
        var prop = typeof(Application).GetProperty("streamingAssetsPath", BindingFlags.Public | BindingFlags.Static);
        if (prop == null) {
            Console.WriteLine("[GameLoader] WARNING: Cannot find Application.streamingAssetsPath property");
            return;
        }

        var getter = prop.GetGetMethod();
        harmony.Patch(getter,
            prefix: new HarmonyMethod(typeof(GameLoader), nameof(StreamingAssetsPathPrefix)) { priority = Priority.Last });

        // Also fix dataPath (used by some subsystems)
        var dataProp = typeof(Application).GetProperty("dataPath", BindingFlags.Public | BindingFlags.Static);
        if (dataProp != null) {
            harmony.Patch(dataProp.GetGetMethod(),
                prefix: new HarmonyMethod(typeof(GameLoader), nameof(DataPathPrefix)) { priority = Priority.Last });
        }

        Console.WriteLine($"[GameLoader] streamingAssetsPath → {GameStreamingAssetsPath}");

        // Fix ElementLoader's static path field (initialized at class load time with old value)
        var pathField = typeof(ElementLoader).GetField("path", BindingFlags.NonPublic | BindingFlags.Static);
        if (pathField != null) {
            pathField.SetValue(null, GameStreamingAssetsPath + "/elements/");
            Console.WriteLine($"[GameLoader] ElementLoader.path fixed");
        }
    }

    private static bool StreamingAssetsPathPrefix(ref string __result) {
        __result = GameStreamingAssetsPath;
        return false; // skip original
    }

    private static bool DataPathPrefix(ref string __result) {
        __result = Path.GetDirectoryName(GameStreamingAssetsPath) ?? GameStreamingAssetsPath;
        return false;
    }

    /// <summary>
    /// Extract personalities CSV from the game's sharedassets0.assets file.
    /// Unity TextAsset in serialized file: asset name "Personalities" followed by length-prefixed UTF8 data.
    /// Falls back to a minimal stub if extraction fails.
    /// </summary>
    private static string LoadPersonalitiesCsv() {
        try {
            var dataDir = Path.GetDirectoryName(GameStreamingAssetsPath);
            var assetPath = Path.Combine(dataDir!, "sharedassets0.assets");
            if (!File.Exists(assetPath)) {
                Console.WriteLine($"[GameLoader] sharedassets0.assets not found at {assetPath}");
                return FallbackPersonalitiesCsv();
            }

            var data = File.ReadAllBytes(assetPath);
            var marker = System.Text.Encoding.UTF8.GetBytes("Name,Gender,Model,RequiredDlcId");
            var idx = FindBytes(data, marker);
            if (idx < 0) {
                Console.WriteLine("[GameLoader] Personalities CSV header not found in assets");
                return FallbackPersonalitiesCsv();
            }

            // Find end of CSV (null terminator)
            var end = idx;
            while (end < data.Length && data[end] != 0) end++;

            var csv = System.Text.Encoding.UTF8.GetString(data, idx, end - idx);
            var lineCount = csv.Split('\n').Length;
            Console.WriteLine($"[GameLoader] Loaded {lineCount} personalities from game assets");
            return csv;
        } catch (Exception ex) {
            Console.WriteLine($"[GameLoader] Failed to load personalities: {ex.Message}");
            return FallbackPersonalitiesCsv();
        }
    }

    private static int FindBytes(byte[] haystack, byte[] needle) {
        for (var i = 0; i <= haystack.Length - needle.Length; i++) {
            var found = true;
            for (var j = 0; j < needle.Length; j++) {
                if (haystack[i + j] != needle[j]) { found = false; break; }
            }
            if (found) return i;
        }
        return -1;
    }

    private static string FallbackPersonalitiesCsv() =>
        "Name,Gender,PersonalityType,StressTrait,JoyTrait,StickerType,CongenitalTrait," +
        "HeadShape,Mouth,Neck,Eyes,Hair,Body,Belt,Cuff,Foot,Hand,Pelvis,Leg,Arm_Skin,Leg_Skin," +
        "ValidStarter,Grave,Model,SpeechMouth,RequiredDlcId\n" +
        "Meep,Male,Doofy,Aggressive,SparkleStreaker,,,3,0,0,5,7,3,0,0,0,0,0,0,3,3,0,meep,Minion,0,\n" +
        "Bubbles,Female,Doofy,BingeEater,StickerBomber,glitter,,3,0,0,2,30,2,0,0,0,0,0,0,3,3,1,bubbles,Minion,0,\n" +
        "Stinky,Male,Doofy,StressVomiter,BalloonArtist,,,1,0,0,5,15,3,0,0,0,0,0,0,1,1,1,stinky,Minion,0,";

    /// <summary>
    /// Load elements using the game's ElementLoader with stub SubstanceTables.
    /// This calls the game's own YAML parsing + element creation logic.
    /// </summary>
    private void LoadElementsFromGame() {
        // Provide stub SubstanceTable for each DLC — ElementLoader.Load() checks
        // substanceTablesByDlc.ContainsKey(entry.dlcId) to decide which elements to load.
        // Ensure FileSystem is initialized (ElementLoader.Load uses FileSystem.GetFiles)
        Klei.FileSystem.Initialize();

        var substanceList = new Hashtable();
        var substanceTables = new Dictionary<string, SubstanceTable> { { "", CreateStubSubstanceTable() } };

        // Add DLC substance tables if DLCs are present
        foreach (var dlcId in DlcManager.RELEASED_VERSIONS) {
            if (!substanceTables.ContainsKey(dlcId)) {
                substanceTables[dlcId] = CreateStubSubstanceTable();
            }
        }

        // Patch CollectElementsFromYAML — the static `path` field reads Application.streamingAssetsPath
        // at class load time, which may be empty. We override it via Harmony prefix.
        var collectMethod = AccessTools.Method(typeof(ElementLoader), "CollectElementsFromYAML");
        if (collectMethod != null) {
            harmony.Patch(collectMethod,
                prefix: new HarmonyMethod(typeof(GameLoader), nameof(FixElementPath)));
        }

        ElementLoader.Load(ref substanceList, substanceTables);

        // Fix element names — use tag name if localization didn't resolve
        foreach (var elem in ElementLoader.elements) {
            if (elem.name != null && elem.name.Contains("MISSING.STRINGS")) {
                elem.name = elem.tag.Name;
                elem.nameUpperCase = elem.name.ToUpper();
            }
            if (elem.substance == null) {
                elem.substance = new Substance {
                    nameTag = elem.tag,
                    anim = new KAnimFile { IsBuildLoaded = true }
                };
            }
        }

        // Build elementTagTable if not already populated
        if (ElementLoader.elementTagTable == null || ElementLoader.elementTagTable.Count == 0) {
            ElementLoader.elementTagTable = new Dictionary<Tag, Element>();
            foreach (var elem in ElementLoader.elements) {
                ElementLoader.elementTagTable[elem.tag] = elem;
            }
        }

        WorldGen.SetupDefaultElements();
        Console.WriteLine($"[GameLoader] Registered {ElementLoader.elements.Count} elements");
    }

    /// <summary>
    /// Create a SubstanceTable with an initialized (empty) list.
    /// The default ScriptableObject constructor leaves 'list' null.
    /// </summary>
    private static SubstanceTable CreateStubSubstanceTable() {
        var table = ScriptableObject.CreateInstance<SubstanceTable>();
        var listField = typeof(SubstanceTable).GetField("list", BindingFlags.NonPublic | BindingFlags.Instance);
        listField?.SetValue(table, new List<Substance>());
        return table;
    }

    private void InitializeWorld() {
        var worldGameObject = new GameObject();
        KObjectManager.Instance?.OnDestroy();
        var kObjectManager = worldGameObject.AddComponent<KObjectManager>();
        kObjectManager.Awake();
        DistributionPlatform.sImpl = worldGameObject.AddComponent<SteamDistributionPlatform>();

        InitGame(worldGameObject);
        worldGameObject.AddComponent<Notifier>();
        ReportManager.Instance = worldGameObject.AddComponent<ReportManager>();
        ReportManager.Instance.Awake();
        ReportManager.Instance.todaysReport = new ReportManager.DailyReport(ReportManager.Instance);

        StateMachineDebuggerSettings._Instance = new StateMachineDebuggerSettings();
        StateMachineDebuggerSettings._Instance.Initialize();

        StateMachineManager.Instance.Clear();
        StateMachine.Instance.error = false;

        worldGameObject.AddComponent<MinionGroupProber>().Awake();
        worldGameObject.AddComponent<GameClock>().Awake();
        worldGameObject.AddComponent<GlobalChoreProvider>().Awake();
        worldGameObject.AddComponent<GameScenePartitioner>().Awake();
        World.Instance = null;
        worldGameObject.AddComponent<World>().Awake();
        worldGameObject.AddComponent<Pathfinding>().Awake();
        PathFinder.Initialize();
        new GameNavGrids(Pathfinding.Instance);
        worldGameObject.AddComponent<NavigationReservations>().Awake();
        worldGameObject.AddComponent<ScheduleManager>().Awake();
        worldGameObject.AddComponent<NameDisplayScreen>().Awake();
        worldGameObject.AddComponent<BuildingConfigManager>().Awake();
        SetupAssets(worldGameObject);
        worldGameObject.AddComponent<CustomGameSettings>().Awake();
        GameComps.InfraredVisualizers = new InfraredVisualizerComponents();
        GameScreenManager.Instance = new GameScreenManager();
        GameScreenManager.Instance.worldSpaceCanvas = new GameObject();

        Console.WriteLine("[GameLoader] Game singletons initialized.");
    }

    private void SetupAssets(GameObject worldGameObject) {
        worldGameObject.AddComponent<BundledAssetsLoader>().Awake();
        worldGameObject.AddComponent<BuildingLoader>().Awake();

        var assets = worldGameObject.AddComponent<Assets>();
        assets.AnimAssets = new List<KAnimFile>();
        assets.SpriteAssets = new List<Sprite>();
        assets.TintedSpriteAssets = new List<TintedSprite>();
        assets.MaterialAssets = new List<Material>();
        assets.TextureAssets = new List<Texture2D>();
        assets.TextureAtlasAssets = new List<TextureAtlas>();
        assets.BlockTileDecorInfoAssets = new List<BlockTileDecorInfo>();
        Assets.ModLoadedKAnims = new List<KAnimFile>() { ScriptableObject.CreateInstance<KAnimFile>() };
        assets.elementAudio = new TextAsset("");
        assets.personalitiesFile = new TextAsset(LoadPersonalitiesCsv());
        Assets.instance = assets;
        AsyncLoadManager<IGlobalAsyncLoader>.Run();
    }

    private unsafe void InitGame(GameObject worldGameObject) {
        new GameObject { name = "Canvas" };
        Singleton<CellChangeMonitor>.CreateInstance();

        Global.Instance?.OnDestroy();
        worldGameObject.AddComponent<Global>().Awake();

        var game = worldGameObject.AddComponent<global::Game>();
        game.maleNamesFile = new TextAsset("Bob");
        game.femaleNamesFile = new TextAsset("Alisa");
        try { game.assignmentManager = new AssignmentManager(); }
        catch (ArgumentException) { /* AssignmentGroups already initialized by Db */ }
        global::Game.Instance = game;
        game.obj = KObjectManager.Instance.GetOrCreateObject(game.gameObject);

        TuningData<CPUBudget.Tuning>._TuningData = new CPUBudget.Tuning();

        game.gasConduitSystem = new UtilityNetworkManager<FlowUtilityNetwork, Vent>(width, height, 13);
        game.liquidConduitSystem = new UtilityNetworkManager<FlowUtilityNetwork, Vent>(width, height, 17);
        game.electricalConduitSystem =
            new UtilityNetworkManager<ElectricalUtilityNetwork, Wire>(width, height, 27);
        game.travelTubeSystem = new UtilityNetworkTubesManager(width, height, 35);
        game.gasConduitFlow = new ConduitFlow(ConduitType.Gas, width * height, game.gasConduitSystem, 1f, 0.25f);
        game.liquidConduitFlow = new ConduitFlow(ConduitType.Liquid, width * height, game.liquidConduitSystem, 10f, 0.75f);
        game.mingleCellTracker = worldGameObject.AddComponent<MingleCellTracker>();

        game.statusItemRenderer = new StatusItemRenderer();
        game.fetchManager = new FetchManager();
        GameScheduler.Instance = worldGameObject.AddComponent<GameScheduler>();

        // Allocate temporary Grid for game init — will be replaced by WorldGen/SimDLL
        AllocatePinnedGrid(width, height);

        GameScenePartitioner.instance?.OnForcedCleanUp();
        Console.WriteLine("[GameLoader] Grid initialized.");
    }

    /// <summary>
    /// Load worldgen settings from the game's YAML files via SettingsCache.
    /// </summary>
    private void LoadWorldGenSettings() {
        try {
            // Clear any cached paths (they may have been set with old streamingAssetsPath)
            var cachedPaths = typeof(SettingsCache).GetField("s_cachedPaths", BindingFlags.NonPublic | BindingFlags.Static);
            if (cachedPaths != null) {
                ((Dictionary<string, string>)cachedPaths.GetValue(null)!).Clear();
            }

            var errors = new List<Klei.YamlIO.Error>();
            SettingsCache.LoadFiles(errors);
            if (errors.Count > 0) {
                Console.WriteLine($"[GameLoader] WorldGen settings loaded with {errors.Count} errors:");
                foreach (var err in errors.Take(5)) {
                    Console.WriteLine($"  {err.file}: {err.text}");
                }
            } else {
                Console.WriteLine("[GameLoader] WorldGen settings loaded successfully");
            }
        } catch (Exception ex) {
            Console.WriteLine($"[GameLoader] Failed to load WorldGen settings: {ex.Message}");
            Console.WriteLine($"  {ex.StackTrace?.Split('\n').FirstOrDefault()}");
        }
    }

    public struct WorldGenResult {
        public Sim.Cell[] cells;
        public float[] bgTemp;
        public Sim.DiseaseCell[] dc;
    }

    /// <summary>
    /// Generate a real ONI world using the game's WorldGen pipeline.
    /// </summary>
    private WorldGenResult? GenerateWorld() {
        try {
            var worldName = "worlds/SandstoneDefault";
            var seed = 42;

            Console.WriteLine($"[GameLoader] Creating WorldGen for {worldName}, seed {seed}...");
            var wg = new WorldGen(worldName, new List<string>(), new List<string>(), false);

            // Set world size in Grid
            var worldSize = wg.Settings.world.worldsize;
            width = worldSize.x;
            height = worldSize.y;
            Console.WriteLine($"[GameLoader] World size from settings: {width}x{height}");

            // Re-allocate Grid for the actual world size
            AllocatePinnedGrid(width, height);

            // Reinitialize game systems that depend on world size
            var game = global::Game.Instance;
            if (game != null) {
                game.gasConduitSystem = new UtilityNetworkManager<FlowUtilityNetwork, Vent>(width, height, 13);
                game.liquidConduitSystem = new UtilityNetworkManager<FlowUtilityNetwork, Vent>(width, height, 17);
                game.electricalConduitSystem = new UtilityNetworkManager<ElectricalUtilityNetwork, Wire>(width, height, 27);
                game.travelTubeSystem = new UtilityNetworkTubesManager(width, height, 35);
                game.gasConduitFlow = new ConduitFlow(ConduitType.Gas, width * height, game.gasConduitSystem, 1f, 0.25f);
                game.liquidConduitFlow = new ConduitFlow(ConduitType.Liquid, width * height, game.liquidConduitSystem, 10f, 0.75f);
            }

            // Must set world size before generation (like Cluster.BeginGeneration does)
            wg.SetWorldSize(width, height);
            wg.SetHiddenYOffset(wg.Settings.world.hiddenY);

            wg.Initialise(
                (key, pct, stage) => {
                    Console.WriteLine($"[WorldGen] {stage}: {pct:P0}");
                    return true;
                },
                error => Console.WriteLine($"[WorldGen] ERROR: {error.errorDesc}"),
                worldSeed: seed, layoutSeed: seed, terrainSeed: seed, noiseSeed: seed,
                skipPlacingTemplates: false
            );

            Console.WriteLine("[GameLoader] Running noise + layout generation...");
            if (!wg.GenerateOffline()) {
                Console.WriteLine("[GameLoader] WorldGen.GenerateOffline failed!");
                return null;
            }

            Console.WriteLine("[GameLoader] Rendering world to cells...");
            Sim.Cell[] cells = null;
            Sim.DiseaseCell[] dc = null;
            var placedStoryTraits = new List<ProcGen.WorldTrait>();

            // RenderOffline with doSettle: true (enables template/mob spawning)
            using var ms = new System.IO.MemoryStream();
            using var writer = new System.IO.BinaryWriter(ms);

            var result = wg.RenderOffline(
                doSettle: true,
                simSeed: (uint)seed,
                writer: writer,
                cells: ref cells,
                dc: ref dc,
                baseId: 0,
                placedStoryTraits: ref placedStoryTraits,
                isStartingWorld: true
            );

            if (!result || cells == null) {
                Console.WriteLine("[GameLoader] WorldGen.RenderOffline failed!");
                return null;
            }

            Console.WriteLine($"[GameLoader] WorldGen complete: {cells.Length} cells generated");

            // Capture spawn data (buildings, mobs, geysers, POIs)
            SpawnData = wg.SpawnData;
            if (SpawnData != null) {
                var bldg = SpawnData.buildings?.Count ?? 0;
                var ores = SpawnData.elementalOres?.Count ?? 0;
                var other = SpawnData.otherEntities?.Count ?? 0;
                var picks = SpawnData.pickupables?.Count ?? 0;
                Console.WriteLine($"[GameLoader] SpawnData: {bldg} buildings, {ores} ores, {other} entities, {picks} pickupables");
                Console.WriteLine($"[GameLoader] Start position: {SpawnData.baseStartPos}");

                // Add 3 starter duplicants near the start position using real personalities
                var startX = SpawnData.baseStartPos.x;
                var startY = SpawnData.baseStartPos.y;
                var starters = Db.Get()?.Personalities?.GetStartingPersonalities();
                var names = new List<string>();
                if (starters != null && starters.Count >= 3) {
                    var rng = new Random();
                    var used = new HashSet<int>();
                    for (var i = 0; i < 3; i++) {
                        int idx;
                        do { idx = rng.Next(starters.Count); } while (used.Contains(idx));
                        used.Add(idx);
                        names.Add(starters[idx].Name);
                    }
                } else {
                    names.AddRange(new[] { "Meep", "Bubbles", "Stinky" });
                }
                for (var i = 0; i < 3; i++) {
                    SpawnData.otherEntities.Add(new TemplateClasses.Prefab(
                        names[i], TemplateClasses.Prefab.Type.Other,
                        startX + i, startY, (SimHashes)0));
                }
                Console.WriteLine($"[GameLoader] Added 3 starter duplicants: {string.Join(", ", names)}");
            }

            // Build bgTemp from cells
            var bgTemp = new float[cells.Length];
            for (var i = 0; i < cells.Length; i++) {
                bgTemp[i] = cells[i].temperature;
            }

            return new WorldGenResult { cells = cells, bgTemp = bgTemp, dc = dc ?? new Sim.DiseaseCell[cells.Length] };
        } catch (Exception ex) {
            Console.WriteLine($"[GameLoader] WorldGen failed: {ex.GetType().Name}: {ex.Message}");
            Console.WriteLine($"  {ex.StackTrace}");
            return null;
        }
    }

    /// <summary>
    /// Initialize SimDLL with the generated world cells.
    /// After this, Grid pointers point to DLL-owned memory and physics simulation runs.
    /// </summary>
    private unsafe void InitSimDLL(Sim.Cell[] cells, float[] bgTemp, Sim.DiseaseCell[] dc) {
        try {
            Console.WriteLine("[SimDLL] Initializing...");
            Sim.SIM_Initialize(ServerDllMessageHandler);
            Console.WriteLine("[SimDLL] SIM_Initialize OK");

            Console.WriteLine($"[SimDLL] Creating element table ({ElementLoader.elements.Count} elements)...");
            SimMessages.CreateSimElementsTable(ElementLoader.elements);
            Console.WriteLine("[SimDLL] Element table created");

            // Create empty disease table (no diseases in headless mode)
            Console.WriteLine("[SimDLL] Creating disease table...");
            var diseases = new Diseases(null, statsOnly: true);
            SimMessages.CreateDiseaseTable(diseases);
            Console.WriteLine("[SimDLL] Disease table created");

            Console.WriteLine($"[SimDLL] Initializing from cells ({width}x{height})...");
            SimMessages.SimDataInitializeFromCells(width, height, 42, cells, bgTemp, dc, headless: true);
            Console.WriteLine("[SimDLL] Cell data sent to SimDLL");

            Console.WriteLine("[SimDLL] Starting simulation...");
            Sim.Start();
            Console.WriteLine("[SimDLL] Simulation started — Grid now points to DLL-owned memory");

            SimRunning = true;
        } catch (Exception ex) {
            Console.WriteLine($"[SimDLL] Failed: {ex.GetType().Name}: {ex.Message}");
            Console.WriteLine($"  {ex.StackTrace}");
            Console.WriteLine("[SimDLL] Falling back to pinned arrays (no physics)");
            SimRunning = false;

            // Restore pinned Grid arrays since SimDLL failed
            AllocatePinnedGrid(width, height);
            // Copy cell data into managed arrays
            for (var i = 0; i < cells.Length && i < width * height; i++) {
                Grid.elementIdx[i] = cells[i].elementIdx;
                Grid.temperature[i] = cells[i].temperature;
            }
        }
    }

    /// <summary>
    /// Tick the SimDLL physics by one step (200ms game time).
    /// Call this in a loop to advance the simulation.
    /// </summary>
    public unsafe void TickSimulation() {
        if (!SimRunning) return;
        SimTick++;

        var activeRegions = new List<global::Game.SimActiveRegion> {
            new() {
                region = new Pair<Vector2I, Vector2I>(
                    new Vector2I(0, 0),
                    new Vector2I(width, height)
                )
            }
        };

        SimMessages.NewGameFrame(0.2f, activeRegions);

        var visible = new byte[Grid.CellCount];
        for (var i = 0; i < visible.Length; i++) visible[i] = byte.MaxValue;

        var ptr = Sim.HandleMessage(SimMessageHashes.PrepareGameData, visible.Length, visible);
        if (ptr != IntPtr.Zero) {
            var update = (Sim.GameDataUpdate*)(void*)ptr;
            Grid.elementIdx = update->elementIdx;
            Grid.temperature = update->temperature;
            Grid.mass = update->mass;
            Grid.radiation = update->radiation;
            Grid.properties = update->properties;
            Grid.strengthInfo = update->strengthInfo;
            Grid.insulation = update->insulation;
        }
    }

    /// <summary>
    /// Allocate pinned Grid arrays that survive GC for long-running server.
    /// </summary>
    public static unsafe void AllocatePinnedGrid(int gridWidth, int gridHeight) {
        var numCells = gridWidth * gridHeight;
        GridSettings.Reset(gridWidth, gridHeight);

        // Free previous handles if any
        if (elementIdxHandle.IsAllocated) elementIdxHandle.Free();
        if (temperatureHandle.IsAllocated) temperatureHandle.Free();
        if (radiationHandle.IsAllocated) radiationHandle.Free();
        if (massHandle.IsAllocated) massHandle.Free();

        var elementIdxArr = new ushort[numCells];
        elementIdxHandle = GCHandle.Alloc(elementIdxArr, GCHandleType.Pinned);
        Grid.elementIdx = (ushort*)elementIdxHandle.AddrOfPinnedObject();

        var tempArr = new float[numCells];
        temperatureHandle = GCHandle.Alloc(tempArr, GCHandleType.Pinned);
        Grid.temperature = (float*)temperatureHandle.AddrOfPinnedObject();

        var radArr = new float[numCells];
        radiationHandle = GCHandle.Alloc(radArr, GCHandleType.Pinned);
        Grid.radiation = (float*)radiationHandle.AddrOfPinnedObject();

        var massArr = new float[numCells];
        massHandle = GCHandle.Alloc(massArr, GCHandleType.Pinned);
        Grid.mass = (float*)massHandle.AddrOfPinnedObject();

        Grid.InitializeCells();
        Console.WriteLine($"[GameLoader] Pinned Grid allocated: {gridWidth}x{gridHeight}");
    }

    /// <summary>
    /// Safe SimDLL message handler — logs to console instead of crashing via KCrashReporter.
    /// </summary>
    private static int ServerDllMessageHandler(int messageId, IntPtr data) {
        Console.WriteLine($"[SimDLL] Message from DLL: id={messageId}");
        return 0;
    }

    public void Shutdown() {
        if (!IsLoaded) return;
        Console.WriteLine("[GameLoader] Shutting down...");

        if (SimRunning) {
            try { Sim.SIM_Shutdown(); } catch { /* ignore */ }
            SimRunning = false;
        }

        UnityTestRuntime.Uninstall();
        PatchesSetup.Uninstall(harmony);

        global::Game.Instance = null;
        Global.Instance = null;
        KObjectManager.Instance = null;
        World.Instance = null;
        IsLoaded = false;
    }
}
