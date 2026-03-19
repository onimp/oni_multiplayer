using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MultiplayerMod.Test.Environment.Patches;
using MultiplayerMod.Test.Environment.Unity;
using MultiplayerMod.Test.GameRuntime.Patches;
using UnityEngine;

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

        ElementLoader.elements = new List<Element> { new() };
        ResetGrid();

        GameScenePartitioner.instance?.OnForcedCleanUp();
        Console.WriteLine("[GameLoader] Grid initialized.");
    }

    public unsafe void ResetGrid() {
        var numCells = width * height;
        GridSettings.Reset(width, height);
        fixed (ushort* ptr = &(new ushort[numCells])[0]) {
            Grid.elementIdx = ptr;
        }
        fixed (float* ptr = &(new float[numCells])[0]) {
            Grid.temperature = ptr;
            Grid.radiation = ptr;
        }
        Grid.InitializeCells();
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
