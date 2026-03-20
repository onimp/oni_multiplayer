using System;
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

    private const int DefaultWidth = 256;
    private const int DefaultHeight = 384;

    private static readonly string GameStreamingAssetsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        "Library/Application Support/Steam/steamapps/common/OxygenNotIncluded",
        "OxygenNotIncluded.app/Contents/Resources/Data/StreamingAssets"
    );

    private Harmony harmony = null!;
    private int width;
    private int height;

    public int Width => width;
    public int Height => height;
    public bool IsLoaded { get; private set; }
    public bool SimRunning { get; private set; }

    // GC handles to keep pinned arrays alive for the server lifetime
    private static GCHandle elementIdxHandle;
    private static GCHandle temperatureHandle;
    private static GCHandle radiationHandle;
    private static GCHandle massHandle;

    public void Boot(int worldWidth = DefaultWidth, int worldHeight = DefaultHeight) {
        width = worldWidth;
        height = worldHeight;

        Console.WriteLine("[GameLoader] Installing patches...");
        harmony = new Harmony("DedicatedServer");
        InstallPatches();

        Console.WriteLine("[GameLoader] Fixing streaming assets path...");
        FixStreamingAssetsPath();

        Console.WriteLine("[GameLoader] Loading elements from game YAML...");
        LoadElementsFromGame();

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
    /// Load all elements from the game's YAML files, bypassing SubstanceTable (Unity assets).
    /// Reads YAML directly from disk — the game's FileSystem may not be initialized.
    /// </summary>
    private void LoadElementsFromGame() {
        ElementLoader.elements = new List<Element>();
        ElementLoader.elementTable = new Dictionary<int, Element>();
        ElementLoader.elementTagTable = new Dictionary<Tag, Element>();

        var elementsPath = GameStreamingAssetsPath + "/elements/";
        Console.WriteLine($"[GameLoader] Elements path: {elementsPath}");
        Console.WriteLine($"[GameLoader] Path exists: {Directory.Exists(elementsPath)}");

        // Read YAML files directly (game's FileSystem may not be set up)
        var entries = new List<ElementLoader.ElementEntry>();
        if (Directory.Exists(elementsPath)) {
            foreach (var yamlFile in Directory.GetFiles(elementsPath, "*.yaml")) {
                if (Path.GetFileName(yamlFile).StartsWith(".")) continue;
                try {
                    var collection = Klei.YamlIO.LoadFile<ElementLoader.ElementEntryCollection>(yamlFile, null);
                    if (collection?.elements != null) {
                        entries.AddRange(collection.elements);
                    }
                } catch (Exception ex) {
                    Console.WriteLine($"[GameLoader] Failed to load {Path.GetFileName(yamlFile)}: {ex.Message}");
                }
            }
        }
        Console.WriteLine($"[GameLoader] Found {entries.Count} element entries in YAML");

        foreach (var entry in entries) {
            var hash = Hash.SDBMLower(entry.elementId);
            if (ElementLoader.elementTable.ContainsKey(hash)) continue;

            // Skip DLC elements if not "vanilla" (dlcId == "")
            // For simplicity, load all — SimDLL will ignore unused ones
            var element = new Element();
            element.id = (SimHashes)hash;
            element.name = entry.elementId; // Use ID as name (no localization)
            element.nameUpperCase = entry.elementId.ToUpper();
            element.tag = TagManager.Create(entry.elementId, entry.elementId);
            element.dlcId = entry.dlcId;

            // Copy all physical properties
            CopyEntryToElement(entry, element);

            // Create stub Substance (needed by FinaliseElementsTable)
            element.substance = new Substance {
                nameTag = element.tag,
                anim = new KAnimFile { IsBuildLoaded = true }
            };

            var defMass = entry.defaultMass;
            if (defMass <= 0f) defMass = 1f;
            element.defaultValues = new Sim.PhysicsData {
                temperature = entry.defaultTemperature,
                mass = defMass
            };

            ElementLoader.elements.Add(element);
            ElementLoader.elementTable[hash] = element;
            ElementLoader.elementTagTable[element.tag] = element;
        }

        // Sort and index elements (like FinaliseElementsTable)
        FinaliseElements();
        WorldGen.SetupDefaultElements();

        Console.WriteLine($"[GameLoader] Registered {ElementLoader.elements.Count} elements");
    }

    private static void CopyEntryToElement(ElementLoader.ElementEntry entry, Element elem) {
        elem.specificHeatCapacity = entry.specificHeatCapacity;
        elem.thermalConductivity = entry.thermalConductivity;
        elem.molarMass = entry.molarMass;
        elem.strength = entry.strength;
        elem.disabled = entry.isDisabled;
        elem.flow = entry.flow;
        elem.maxMass = entry.maxMass;
        elem.maxCompression = entry.liquidCompression;
        elem.viscosity = entry.speed;
        elem.minHorizontalFlow = entry.minHorizontalFlow;
        elem.minVerticalFlow = entry.minVerticalFlow;
        elem.solidSurfaceAreaMultiplier = entry.solidSurfaceAreaMultiplier;
        elem.liquidSurfaceAreaMultiplier = entry.liquidSurfaceAreaMultiplier;
        elem.gasSurfaceAreaMultiplier = entry.gasSurfaceAreaMultiplier;
        elem.state = entry.state;
        elem.hardness = entry.hardness;
        elem.lowTemp = entry.lowTemp;
        elem.lowTempTransitionTarget = (SimHashes)Hash.SDBMLower(entry.lowTempTransitionTarget);
        elem.highTemp = entry.highTemp;
        elem.highTempTransitionTarget = (SimHashes)Hash.SDBMLower(entry.highTempTransitionTarget);
        elem.highTempTransitionOreID = (SimHashes)Hash.SDBMLower(entry.highTempTransitionOreId);
        elem.highTempTransitionOreMassConversion = entry.highTempTransitionOreMassConversion;
        elem.lowTempTransitionOreID = (SimHashes)Hash.SDBMLower(entry.lowTempTransitionOreId);
        elem.lowTempTransitionOreMassConversion = entry.lowTempTransitionOreMassConversion;
    }

    /// <summary>
    /// Simplified FinaliseElementsTable — sort, index, set transition targets.
    /// Skips SubstanceTable/texture lookups.
    /// </summary>
    private static void FinaliseElements() {
        // Set temperature-related flags
        foreach (var elem in ElementLoader.elements) {
            if (elem.thermalConductivity == 0f)
                elem.state |= Element.State.TemperatureInsulated;
            if (elem.strength == 0f)
                elem.state |= Element.State.Unbreakable;
        }

        // Sort: solids first (descending), then by id
        ElementLoader.elements = ElementLoader.elements
            .OrderByDescending(e => (int)(e.state & Element.State.Solid))
            .ThenBy(e => e.id)
            .ToList();

        // Re-index and rebuild lookup tables
        ElementLoader.elementTable.Clear();
        for (var i = 0; i < ElementLoader.elements.Count; i++) {
            var elem = ElementLoader.elements[i];
            elem.idx = (ushort)i;
            if (elem.substance != null)
                elem.substance.idx = i;
            ElementLoader.elementTable[(int)elem.id] = elem;
        }

        // Resolve transition targets
        foreach (var elem in ElementLoader.elements) {
            if (elem.IsSolid) {
                elem.highTempTransition = ElementLoader.FindElementByHash(elem.highTempTransitionTarget);
            } else if (elem.IsLiquid) {
                elem.highTempTransition = ElementLoader.FindElementByHash(elem.highTempTransitionTarget);
                elem.lowTempTransition = ElementLoader.FindElementByHash(elem.lowTempTransitionTarget);
            } else if (elem.IsGas) {
                elem.lowTempTransition = ElementLoader.FindElementByHash(elem.lowTempTransitionTarget);
            }
        }
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
        assets.personalitiesFile = new TextAsset(
            "Name,Gender,PersonalityType,StressTrait,JoyTrait,StickerType,CongenitalTrait," +
            "HeadShape,Mouth,Neck,Eyes,Hair,Body,Belt,Cuff,Foot,Hand,Pelvis,Leg,Arm_Skin,Leg_Skin," +
            "ValidStarter,Grave,Model,SpeechMouth,RequiredDlcId\n" +
            "TestDupe,Male,Sweet,UglyCrier,BalloonArtist,,,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,testdupe,Minion,0,"
        );
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
                skipPlacingTemplates: true
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

            // RenderOffline with doSettle: false (skip SimDLL settle, we do our own)
            using var ms = new System.IO.MemoryStream();
            using var writer = new System.IO.BinaryWriter(ms);

            var result = wg.RenderOffline(
                doSettle: false,
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
            Sim.SIM_Initialize(Sim.DLL_MessageHandler);
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
