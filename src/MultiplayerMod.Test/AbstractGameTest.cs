using System;
using System.Collections.Generic;
using HarmonyLib;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Test.Environment.Patches;
using MultiplayerMod.Test.Environment.Unity;
using MultiplayerMod.Test.Multiplayer.Commands.Chores.Patches;
using NUnit.Framework;
using UnityEngine;

namespace MultiplayerMod.Test;

public abstract class AbstractGameTest {

    private static Harmony harmony = null!;

    [OneTimeSetUp]
    public static void SetUpGame() {
        harmony = new Harmony("AbstractGameTest");
        var patches = new HashSet<Type>(new[] { typeof(DbPatch), typeof(AssetsPatch), typeof(ElementLoaderPatch) });
        UnityTestRuntime.Install();
        PatchesSetup.Install(harmony, patches);
        SetUpUnityAndGame();
        SetupDependencies();
    }

    [OneTimeTearDown]
    public static void TearDown() {
        UnityTestRuntime.Uninstall();
        PatchesSetup.Uninstall(harmony);
        global::Game.Instance = null;
    }

    protected static GameObject createGameObject() {
        var gameObject = new GameObject();
        gameObject.AddComponent<KMonoBehaviour>();
        gameObject.AddComponent<MultiplayerInstance>();
        Grid.Objects[Grid.PosToCell(gameObject), 0] = gameObject;
        return gameObject;
    }

    private static void SetUpUnityAndGame() {
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
        worldGameObject.AddComponent<NameDisplayScreen>().Awake();
        worldGameObject.AddComponent<BuildingConfigManager>().Awake();
        SetupAssets(worldGameObject);
        worldGameObject.AddComponent<CustomGameSettings>().Awake();
        GameComps.InfraredVisualizers = new InfraredVisualizerComponents();
    }

    private static void SetupAssets(GameObject worldGameObject) {
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
        assets.personalitiesFile = new TextAsset("");
        Assets.instance = assets;

        AsyncLoadManager<IGlobalAsyncLoader>.Run();
        //assets.Awake();
    }

    private static unsafe void InitGame(GameObject worldGameObject) {
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

        var widthInCells = 40;
        var heightInCells = 40;

        TuningData<CPUBudget.Tuning>._TuningData = new CPUBudget.Tuning();

        game.gasConduitSystem = new UtilityNetworkManager<FlowUtilityNetwork, Vent>(widthInCells, heightInCells, 13);
        game.liquidConduitSystem = new UtilityNetworkManager<FlowUtilityNetwork, Vent>(widthInCells, heightInCells, 17);
        game.electricalConduitSystem =
            new UtilityNetworkManager<ElectricalUtilityNetwork, Wire>(widthInCells, heightInCells, 27);
        game.travelTubeSystem = new UtilityNetworkTubesManager(widthInCells, heightInCells, 35);
        game.gasConduitFlow = new ConduitFlow(
            ConduitType.Gas,
            widthInCells * heightInCells,
            game.gasConduitSystem,
            1f,
            0.25f
        );
        game.liquidConduitFlow = new ConduitFlow(
            ConduitType.Liquid,
            widthInCells * heightInCells,
            game.liquidConduitSystem,
            10f,
            0.75f
        );
        game.mingleCellTracker = worldGameObject.AddComponent<MingleCellTracker>();

        game.statusItemRenderer = new StatusItemRenderer();
        game.fetchManager = new FetchManager();

        ElementLoader.elements = new List<Element> { new() };
        ResetGrid(widthInCells, heightInCells);

        GameScenePartitioner.instance?.OnForcedCleanUp();
    }

    protected static unsafe void ResetGrid(int widthInCells, int heightInCells) {
        var numCells = widthInCells * heightInCells;
        GridSettings.Reset(widthInCells, heightInCells);
        fixed (ushort* ptr = &(new ushort[numCells])[0]) {
            Grid.elementIdx = ptr;
        }
        fixed (float* ptr = &(new float[numCells])[0]) {
            Grid.temperature = ptr;
            Grid.radiation = ptr;
        }
        Grid.InitializeCells();
    }

    private static void SetupDependencies() {
        var dependencyContainer = new DependencyContainer();
        dependencyContainer.Register(new DependencyInfo(nameof(MultiplayerGame), typeof(MultiplayerGame), false));
        dependencyContainer.Register(
            new DependencyInfo(nameof(ExecutionLevelManager), typeof(ExecutionLevelManager), false)
        );
        dependencyContainer.Register(
            new DependencyInfo(nameof(ExecutionContextManager), typeof(ExecutionContextManager), false)
        );
        dependencyContainer.Register(
            new DependencyInfo(nameof(DependencyContainer), typeof(DependencyContainer), false)
        );
        new Runtime(dependencyContainer);
        Runtime.Instance.Dependencies.Get<MultiplayerGame>().Refresh(MultiplayerMode.Host);
        Runtime.Instance.Dependencies.Get<ExecutionLevelManager>().EnterOverrideSection(ExecutionLevel.Game);
    }
}
