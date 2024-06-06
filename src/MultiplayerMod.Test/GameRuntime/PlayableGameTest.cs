using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Core.Extensions;
using MultiplayerMod.Core.Patch.ControlFlow;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.ModRuntime.StaticCompatibility;
using MultiplayerMod.Multiplayer;
using MultiplayerMod.Multiplayer.Commands.Registry;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Players;
using MultiplayerMod.Test.Environment;
using MultiplayerMod.Test.Environment.Network;
using MultiplayerMod.Test.Environment.Patches;
using MultiplayerMod.Test.Environment.Unity;
using MultiplayerMod.Test.GameRuntime.Patches;
using MultiplayerMod.Test.Multiplayer;
using NUnit.Framework;
using UnityEngine;

namespace MultiplayerMod.Test.GameRuntime;

public abstract class PlayableGameTest {

    protected static Harmony Harmony = null!;
    protected static IDependencyContainer DependencyContainer => Dependencies.Get<IDependencyContainer>();
    protected static EventDispatcher Events => Dependencies.Get<EventDispatcher>();

    [OneTimeSetUp]
    public static void SetUpGame() {
        Harmony = new Harmony("AbstractGameTest");
        var patches = new HashSet<Type>(new[] { typeof(DbPatch), typeof(AssetsPatch), typeof(ElementLoaderPatch) });
        UnityTestRuntime.Install();
        PatchesSetup.Install(Harmony, patches);
        SetUpUnityAndGame();
        SetupDependencies();
        Events.Dispatch(new RuntimeReadyEvent(Runtime.Instance));
    }

    [OneTimeTearDown]
    public static void TearDown() {
        UnityTestRuntime.Uninstall();
        PatchesSetup.Uninstall(Harmony);
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
        GameScreenManager.Instance = new GameScreenManager();
        GameScreenManager.Instance.worldSpaceCanvas = new GameObject();
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
        GameScheduler.Instance = worldGameObject.AddComponent<GameScheduler>();

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

    protected static event Action<DependencyContainerBuilder>? DependencyContainerConfiguring;

    private static void SetupDependencies() {
        var builder = new DependencyContainerBuilder()
            .AddType<MultiplayerGame>()
            .AddType<ExecutionLevelManager>()
            .AddType<ExecutionContextManager>()
            .AddType<EventDispatcher>()
            .AddType<TestRuntime>()
            .AddType<ControlFlowCustomizer>()
            .AddType<TestMultiplayerServer>()
            .AddType<TestMultiplayerClient>()
            .AddSingleton(new MultiplayerTools.TestPlayerProfileProvider(new PlayerProfile("Test")))
            .AddSingleton(new TestMultiplayerClientId(1))
            .AddType<MultiplayerCommandRegistry>()
            .AddSingleton(Harmony);

        ResolveCustomizationProviders<ConfigureDependenciesAttribute>(methods => methods
            .Where(it => it.GetParameters().Length == 1)
            .Where(it => it.GetParameters()[0].ParameterType == typeof(DependencyContainerBuilder))
        ).Invoke([builder]);

        var container = builder.Build();
        container.Get<TestRuntime>().Activate();

        Runtime.Instance.Dependencies.Get<MultiplayerGame>().Refresh(MultiplayerMode.Host);
        Runtime.Instance.Dependencies.Get<ExecutionLevelManager>().EnterOverrideSection(ExecutionLevel.Game);
    }

    protected static TestCustomizer ResolveCustomizationProviders<T>(
        Func<IEnumerable<MethodInfo>, IEnumerable<MethodInfo>>? filter = null
    ) where T : Attribute {
        var methods = Type.GetType(TestContext.CurrentContext.Test.ClassName!)!
            .GetInheritedTypes()
            .Reverse()
            .SelectMany(it => it.GetAllMethods())
            .Where(it => it.IsStatic)
            .Where(it => it.GetCustomAttribute<T>() != null);
        if (filter != null)
            methods = filter(methods);
        return new TestCustomizer(methods.ToArray());
    }

    protected class TestCustomizer(IEnumerable<MethodInfo> methods) {
        public T[] Invoke<T>(object?[] arguments) => methods.Select(it => (T) it.Invoke(null, arguments)).ToArray();
        public void Invoke(object?[] arguments) => methods.ForEach(it => it.Invoke(null, arguments));
    }

    protected class ConfigureDependenciesAttribute : Attribute;

}
