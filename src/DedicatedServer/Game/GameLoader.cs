using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using HarmonyLib;
using MultiplayerMod.Test.Environment.Patches;
using MultiplayerMod.Test.Environment.Unity;
using MultiplayerMod.Test.GameRuntime.Patches;
using UnityEngine;
using Random = System.Random;

namespace DedicatedServer.Game;

/// <summary>
/// Boots the ONI game world from DLLs, similar to PlayableGameTest.
/// Installs Unity patches, initializes game singletons, sets up the Grid.
/// </summary>
public class GameLoader {

    private const int DefaultWidth = 64;
    private const int DefaultHeight = 64;

    private Harmony harmony = null!;
    private int width;
    private int height;

    public int Width => width;
    public int Height => height;
    public bool IsLoaded { get; private set; }

    public void Boot(int worldWidth = DefaultWidth, int worldHeight = DefaultHeight) {
        width = worldWidth;
        height = worldHeight;

        Console.WriteLine("[GameLoader] Installing patches...");
        harmony = new Harmony("DedicatedServer");

        // Get all Unity patches from the test assembly (same as UnityTestRuntime.Install())
        var unityPatchTypes = typeof(UnityTestRuntime).Assembly.GetTypes()
            .Where(type => type.Namespace?.StartsWith(typeof(UnityTestRuntime).Namespace + ".Patches") == true)
            .ToList();

        Console.WriteLine($"[GameLoader] Found {unityPatchTypes.Count} Unity patch types");

        // Test Harmony init with a simple self-test first
        Console.WriteLine("[GameLoader] Testing Harmony...");
        Console.Out.Flush();
        try {
            var testPatch = harmony.CreateClassProcessor(unityPatchTypes[0]);
            Console.WriteLine($"[GameLoader] Created processor for: {unityPatchTypes[0].Name}");
            Console.Out.Flush();
            testPatch.Patch();
            Console.WriteLine($"[GameLoader] First patch OK: {unityPatchTypes[0].Name}");
        } catch (Exception ex) {
            Console.WriteLine($"[GameLoader] First patch failed: {ex.Message}");
            var inner = ex;
            while (inner.InnerException != null) {
                inner = inner.InnerException;
                Console.WriteLine($"[GameLoader]   → {inner.GetType().Name}: {inner.Message}");
            }
        }
        Console.Out.Flush();

        // Apply remaining patches
        for (var i = 1; i < unityPatchTypes.Count; i++) {
            var patchType = unityPatchTypes[i];
            try {
                harmony.CreateClassProcessor(patchType).Patch();
                Console.WriteLine($"[GameLoader] Patch OK: {patchType.Name}");
            } catch (Exception ex) {
                Console.WriteLine($"[GameLoader] Patch FAIL: {patchType.Name}: {ex.Message}");
            }
        }

        // Install game-specific patches
        var gamePatches = new Type[] {
            typeof(DbPatch),
            typeof(AssetsPatch),
            typeof(ElementLoaderPatch),
            typeof(SensorsPatch),
            typeof(ChoreConsumerStatePatch)
        };
        foreach (var patchType in gamePatches) {
            try {
                harmony.CreateClassProcessor(patchType).Patch();
                Console.WriteLine($"[GameLoader] Patch OK: {patchType.Name}");
            } catch (Exception ex) {
                Console.WriteLine($"[GameLoader] Patch FAIL: {patchType.Name}: {ex.Message}");
            }
        }

        Console.WriteLine("[GameLoader] Initializing game world...");
        InitializeWorld();

        IsLoaded = true;
        Console.WriteLine($"[GameLoader] World ready: {width}x{height} ({width * height} cells)");
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
        game.assignmentManager = new AssignmentManager();
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

        RegisterElements();
        ResetGrid();
        PopulateWorld(width, height);

        GameScenePartitioner.instance?.OnForcedCleanUp();
        Console.WriteLine("[GameLoader] Grid initialized and populated.");
    }

    /// <summary>
    /// Register elements matching the frontend's hardcoded indices (0-10).
    /// These must match constants.ts ELEMENT_NAMES/ELEMENT_COLORS.
    /// </summary>
    public static void RegisterElements() {
        var elementDefs = new (string name, SimHashes hash, Element.State state, float defaultTemp, float defaultMass)[] {
            ("Vacuum",         SimHashes.Vacuum,              Element.State.Vacuum, 0f,      0f),
            ("Oxygen",         SimHashes.Oxygen,              Element.State.Gas,    293.15f, 1.8f),
            ("Carbon Dioxide", SimHashes.CarbonDioxide,       Element.State.Gas,    293.15f, 1.8f),
            ("Hydrogen",       SimHashes.Hydrogen,            Element.State.Gas,    293.15f, 0.09f),
            ("Water",          SimHashes.Water,               Element.State.Liquid, 293.15f, 1000f),
            ("Dirty Water",    SimHashes.DirtyWater,          Element.State.Liquid, 293.15f, 1000f),
            ("Granite",        SimHashes.Granite,             Element.State.Solid,  293.15f, 2500f),
            ("Sandstone",      SimHashes.SandStone,           Element.State.Solid,  293.15f, 2000f),
            ("Algae",          SimHashes.Algae,               Element.State.Solid,  293.15f, 200f),
            ("Copper Ore",     SimHashes.Cuprite,             Element.State.Solid,  293.15f, 2000f),
            ("Ice",            SimHashes.Ice,                 Element.State.Solid,  243.15f, 1000f),
        };

        ElementLoader.elements = new List<Element>();
        ElementLoader.elementTable = new Dictionary<int, Element>();

        for (ushort i = 0; i < elementDefs.Length; i++) {
            var def = elementDefs[i];
            var elem = new Element {
                id = def.hash,
                name = def.name,
                nameUpperCase = def.name.ToUpper(),
                idx = i,
                state = def.state,
                defaultValues = new Sim.PhysicsData {
                    temperature = def.defaultTemp,
                    mass = def.defaultMass,
                },
                substance = new Substance()
            };
            ElementLoader.elements.Add(elem);
            ElementLoader.elementTable[(int)def.hash] = elem;
        }

        Console.WriteLine($"[GameLoader] Registered {ElementLoader.elements.Count} elements.");
    }

    // GC handles to keep pinned arrays alive for the server lifetime
    private static GCHandle elementIdxHandle;
    private static GCHandle temperatureHandle;
    private static GCHandle radiationHandle;

    /// <summary>
    /// Allocate pinned Grid arrays that survive GC.
    /// The fixed() block in PlayableGameTest only pins during the block —
    /// for a long-running server we need GCHandle.Alloc with Pinned type.
    /// </summary>
    public static unsafe void AllocatePinnedGrid(int gridWidth, int gridHeight) {
        var numCells = gridWidth * gridHeight;
        GridSettings.Reset(gridWidth, gridHeight);

        var elementIdxArr = new ushort[numCells];
        elementIdxHandle = GCHandle.Alloc(elementIdxArr, GCHandleType.Pinned);
        Grid.elementIdx = (ushort*)elementIdxHandle.AddrOfPinnedObject();

        var tempArr = new float[numCells];
        temperatureHandle = GCHandle.Alloc(tempArr, GCHandleType.Pinned);
        Grid.temperature = (float*)temperatureHandle.AddrOfPinnedObject();

        var radArr = new float[numCells];
        radiationHandle = GCHandle.Alloc(radArr, GCHandleType.Pinned);
        Grid.radiation = (float*)radiationHandle.AddrOfPinnedObject();

        Grid.InitializeCells();
        Console.WriteLine($"[GameLoader] Pinned Grid allocated: {gridWidth}x{gridHeight} ({numCells} cells).");
    }

    public unsafe void ResetGrid() {
        AllocatePinnedGrid(width, height);
    }

    /// <summary>
    /// Fill Grid cells with a procedural world layout.
    /// Matches the structure of MockWorldState for visual consistency.
    /// </summary>
    public static unsafe void PopulateWorld(int width, int height) {
        const ushort VACUUM = 0, OXYGEN = 1, CO2 = 2, HYDROGEN = 3, WATER = 4;
        const ushort DIRTY_WATER = 5, GRANITE = 6, SANDSTONE = 7, ALGAE = 8, COPPER = 9, ICE = 10;

        var rng = new Random(42);

        for (var y = 0; y < height; y++) {
            for (var x = 0; x < width; x++) {
                var cell = y * width + x;
                ushort element;
                float temp;

                if (y < height / 12) {
                    // Bottom layer: rock floor
                    element = GRANITE;
                    temp = 310f + (float)(rng.NextDouble() * 20);
                } else if (y < height / 4) {
                    // Lower zone: mixed resources
                    var r = rng.NextDouble();
                    if (r < 0.4) element = SANDSTONE;
                    else if (r < 0.6) element = COPPER;
                    else if (r < 0.75) element = ALGAE;
                    else element = OXYGEN;
                    temp = 295f + (float)(rng.NextDouble() * 15);
                } else if (y < height * 3 / 4) {
                    // Middle: habitable zone
                    var inStartingArea = x >= width / 4 && x < width * 3 / 4
                                      && y >= height / 3 && y < height * 2 / 3;

                    if (inStartingArea) {
                        // Starting biome: oxygen-rich
                        element = OXYGEN;
                        temp = 293f + (float)(rng.NextDouble() * 5);
                    } else if (x < 3 || x >= width - 3) {
                        // Side walls
                        element = rng.NextDouble() < 0.7 ? GRANITE : SANDSTONE;
                        temp = 300f + (float)(rng.NextDouble() * 10);
                    } else {
                        // Open area: gas mix
                        var r = rng.NextDouble();
                        if (r < 0.5) element = OXYGEN;
                        else if (r < 0.7) element = CO2;
                        else if (r < 0.85) element = HYDROGEN;
                        else element = VACUUM;
                        temp = element == VACUUM ? 0f : 290f + (float)(rng.NextDouble() * 10);
                    }

                    // Water pool in starting area
                    if (x >= width / 3 && x < width * 2 / 3
                        && y >= height / 3 && y < height / 3 + 4) {
                        element = rng.NextDouble() < 0.9 ? WATER : DIRTY_WATER;
                        temp = 288f + (float)(rng.NextDouble() * 5);
                    }
                } else if (y < height * 7 / 8) {
                    // Upper cold zone
                    var r = rng.NextDouble();
                    if (r < 0.4) element = ICE;
                    else if (r < 0.6) element = OXYGEN;
                    else element = GRANITE;
                    temp = element == ICE ? 243f + (float)(rng.NextDouble() * 10)
                                         : 260f + (float)(rng.NextDouble() * 15);
                } else {
                    // Top: near-vacuum
                    element = rng.NextDouble() < 0.8 ? VACUUM : OXYGEN;
                    temp = element == VACUUM ? 0f : 250f + (float)(rng.NextDouble() * 20);
                }

                Grid.elementIdx[cell] = element;
                Grid.temperature[cell] = temp;
            }
        }

        Console.WriteLine($"[GameLoader] World populated: {width}x{height} cells.");
    }

    public void Shutdown() {
        if (!IsLoaded) return;
        Console.WriteLine("[GameLoader] Shutting down...");

        UnityTestRuntime.Uninstall();
        PatchesSetup.Uninstall(harmony);

        global::Game.Instance = null;
        Global.Instance = null;
        KObjectManager.Instance = null;
        World.Instance = null;
        IsLoaded = false;
    }
}
