using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Test.Environment.Patches;
using MultiplayerMod.Test.Environment.Unity;
using MultiplayerMod.Test.Multiplayer.Commands.Chores;
using NUnit.Framework;
using UnityEngine;

namespace MultiplayerMod.Test;

public abstract class AbstractGameTest {

    private static Harmony harmony = null!;

    protected HashSet<Type> patches = typeof(CreateHostChoreTest).Assembly.GetTypes()
        .Where(type => type.Namespace == typeof(CreateHostChoreTest).Namespace + ".Patches")
        .ToHashSet();

    [SetUp]
    public void SetUp() {
        harmony = new Harmony("CreateHostChoreTest");
        UnityTestRuntime.Install();
        PatchesSetup.Install(harmony, patches);
        SetUpUnityAndGame();
        SetupDependencies();
    }

    [TearDown]
    public static void TearDown() {
        UnityTestRuntime.Uninstall();
        PatchesSetup.Uninstall(harmony);
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
        ReportManager.Instance = worldGameObject.AddComponent<ReportManager>();
        ReportManager.Instance.Awake();
        ReportManager.Instance.todaysReport = new ReportManager.DailyReport(ReportManager.Instance);

        GlobalChoreProvider.Instance = new GlobalChoreProvider();

        StateMachineDebuggerSettings._Instance = new StateMachineDebuggerSettings();
        StateMachineDebuggerSettings._Instance.Initialize();
    }

    private static void InitGame(GameObject worldGameObject) {
        // From global
        Singleton<StateMachineUpdater>.CreateInstance();
        Singleton<StateMachineManager>.CreateInstance();

        var game = worldGameObject.AddComponent<global::Game>();
        global::Game.Instance = game;

        var widthInCells = 40;
        var heightInCells = 40;
        var numCells = widthInCells * heightInCells;

        TuningData<CPUBudget.Tuning>._TuningData = new CPUBudget.Tuning();

        game.gasConduitSystem = new UtilityNetworkManager<FlowUtilityNetwork, Vent>(widthInCells, heightInCells, 13);
        game.liquidConduitSystem = new UtilityNetworkManager<FlowUtilityNetwork, Vent>(widthInCells, heightInCells, 17);
        game.electricalConduitSystem =
            new UtilityNetworkManager<ElectricalUtilityNetwork, Wire>(widthInCells, heightInCells, 27);
        game.travelTubeSystem = new UtilityNetworkTubesManager(widthInCells, heightInCells, 35);
        game.gasConduitFlow = new ConduitFlow(ConduitType.Gas, numCells, game.gasConduitSystem, 1f, 0.25f);
        game.liquidConduitFlow = new ConduitFlow(ConduitType.Liquid, numCells, game.liquidConduitSystem, 10f, 0.75f);

        GridSettings.Reset(widthInCells, heightInCells);

        GameScenePartitioner.instance?.OnForcedCleanUp();
        var partitioner = worldGameObject.AddComponent<GameScenePartitioner>();
        partitioner.OnPrefabInit();
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
            new DependencyInfo(nameof(DependencyContainer), typeof(DependencyContainer), false));
        new Runtime(dependencyContainer);
        Runtime.Instance.Dependencies.Get<MultiplayerGame>().Refresh(MultiplayerMode.Host);
        Runtime.Instance.Dependencies.Get<ExecutionLevelManager>().EnterOverrideSection(ExecutionLevel.Game);
    }
}
