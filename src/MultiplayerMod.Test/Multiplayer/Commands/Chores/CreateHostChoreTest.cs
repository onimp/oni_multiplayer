using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using HarmonyLib;
using MultiplayerMod.Core.Unity;
using MultiplayerMod.Game.Chores;
using MultiplayerMod.Multiplayer.Commands.Chores;
using MultiplayerMod.Network;
using MultiplayerMod.Platform.Steam.Network.Messaging;
using MultiplayerMod.Test.Environment.Patches;
using MultiplayerMod.Test.Environment.Unity;
using NUnit.Framework;
using STRINGS;
using UnityEngine;

namespace MultiplayerMod.Test.Multiplayer.Commands.Chores;

[TestFixture]
public class CreateHostChoreTest {

    private static Harmony harmony = null!;

    [SetUp]
    public void SetUp() {
        harmony = SetUpUnityAndGame();
    }

    [TearDown]
    public static void TearDown() {
        UnityTestRuntime.Uninstall();
        PatchesSetup.Uninstall(harmony);
    }

    [Test, TestCaseSource(nameof(GetTestArgs))]
    public void ExecutionTest(Type type) {
        var arg = GetTestArg(type);
        var command = new CreateHostChore(arg);

        command.Execute(null!);
    }

    [Test, TestCaseSource(nameof(GetTestArgs))]
    public void SerializationTest(Type type) {
        var arg = GetTestArg(type);
        var command = new CreateHostChore(arg);
        var messageFactory = new NetworkMessageFactory();

        var handles = messageFactory.Create(command, MultiplayerCommandOptions.SkipHost).ToArray();

        Assert.NotNull(handles);
    }

    public static IEnumerable<Type> GetTestArgs() {
        return ChoreList.DeterministicChores;
    }

    private static ChoreEvents.CreateNewChoreArgs GetTestArg(Type type) {
        var targetGameObject = createGameObject();
        var target = targetGameObject.GetComponent<KMonoBehaviour>();

        var gameObject = createGameObject();
        var kPrefabId = createGameObject().AddComponent<KPrefabID>();
        kPrefabId.gameObject.AddComponent<SkillPerkMissingComplainer>(); // required for RancherChore
        var choreType = new ChoreTypes(null).Astronaut;
        var workable = createGameObject().AddComponent<ToiletWorkableUse>();
        workable.Awake();
        var medicineWorkable = createGameObject().AddComponent<MedicinalPillWorkable>();
        medicineWorkable.Awake();
        var db = Db.Get();

        var result = new[] {
            new ChoreEvents.CreateNewChoreArgs(typeof(AttackChore), new object[] { target, gameObject }),
            new ChoreEvents.CreateNewChoreArgs(typeof(DeliverFoodChore), new object[] { target }),
            new ChoreEvents.CreateNewChoreArgs(typeof(DieChore), new object[] { target, db.Deaths.Frozen }),
            new ChoreEvents.CreateNewChoreArgs(typeof(DropUnusedInventoryChore), new object[] { choreType, target }),
            new ChoreEvents.CreateNewChoreArgs(
                typeof(EmoteChore),
                new object?[] { target, choreType, db.Emotes.Minion.Cheer, 1, null }
            ),
            new ChoreEvents.CreateNewChoreArgs(typeof(EquipChore), new object[] { target }),
            new ChoreEvents.CreateNewChoreArgs(typeof(FixedCaptureChore), new object[] { kPrefabId }),
            new ChoreEvents.CreateNewChoreArgs(
                typeof(MoveChore),
                new object[] { target, choreType, new Func<MoveChore.StatesInstance, int>(_ => 0), false }
            ),
            new ChoreEvents.CreateNewChoreArgs(typeof(MoveToQuarantineChore), new object[] { target, target }),
            new ChoreEvents.CreateNewChoreArgs(
                typeof(PartyChore),
                new object?[] { target, workable, null, null, null }
            ),
            new ChoreEvents.CreateNewChoreArgs(typeof(PeeChore), new object[] { target }),
            new ChoreEvents.CreateNewChoreArgs(typeof(PutOnHatChore), new object[] { target, choreType }),
            new ChoreEvents.CreateNewChoreArgs(typeof(RancherChore), new object[] { kPrefabId }),
            new ChoreEvents.CreateNewChoreArgs(
                typeof(ReactEmoteChore),
                new object?[] { target, choreType, null, null, null, null, null }
            ),
            new ChoreEvents.CreateNewChoreArgs(typeof(RescueIncapacitatedChore), new object[] { target, gameObject }),
            new ChoreEvents.CreateNewChoreArgs(
                typeof(RescueSweepBotChore),
                new object[] { target, gameObject, gameObject }
            ),
            new ChoreEvents.CreateNewChoreArgs(typeof(SighChore), new object[] { target }),
            new ChoreEvents.CreateNewChoreArgs(
                typeof(StressEmoteChore),
                new object?[] { target, choreType, null, null, null, null }
            ),
            new ChoreEvents.CreateNewChoreArgs(typeof(StressIdleChore), new object[] { target }),
            new ChoreEvents.CreateNewChoreArgs(typeof(SwitchRoleHatChore), new object[] { target, choreType }),
            new ChoreEvents.CreateNewChoreArgs(typeof(TakeMedicineChore), new object[] { medicineWorkable }),
            new ChoreEvents.CreateNewChoreArgs(typeof(TakeOffHatChore), new object[] { target, choreType }),
            new ChoreEvents.CreateNewChoreArgs(typeof(UglyCryChore), new object?[] { choreType, target, null }),
            new ChoreEvents.CreateNewChoreArgs(
                typeof(WaterCoolerChore),
                new object?[] { target, workable, null, null, null }
            ),
            new ChoreEvents.CreateNewChoreArgs(
                typeof(WorkChore<ToiletWorkableUse>),
                new object?[] {
                    choreType, workable, null, true, null, null, null, true, null, false, true, null, false, true, true,
                    PriorityScreen.PriorityClass.basic, 5, false, true
                }
            )
        };
        Assert.AreEqual(result.Length, ChoreList.DeterministicChores.Count);

        return result.Single(
            a => a.ChoreType == type || (a.ChoreType.IsGenericType && a.ChoreType.GetGenericTypeDefinition() == type)
        );
    }

    private static GameObject createGameObject() {
        var gameObject = new GameObject();
        gameObject.AddComponent<KMonoBehaviour>();
        Grid.Objects[Grid.PosToCell(gameObject), 0] = gameObject;
        return gameObject;
    }

    private static Harmony SetUpUnityAndGame() {
        var harmony = new Harmony("CreateHostChoreTest");
        var patches = typeof(CreateHostChoreTest).Assembly.GetTypes()
            .Where(type => type.Namespace == typeof(CreateHostChoreTest).Namespace + ".Patches")
            .ToList();
        UnityTestRuntime.Install();
        PatchesSetup.Install(harmony, patches);

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

        return harmony;
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
        game.electricalConduitSystem = new UtilityNetworkManager<ElectricalUtilityNetwork, Wire>(widthInCells, heightInCells, 27);
        game.travelTubeSystem = new UtilityNetworkTubesManager(widthInCells, heightInCells, 35);
        game.gasConduitFlow = new ConduitFlow(ConduitType.Gas, numCells, game.gasConduitSystem, 1f, 0.25f);
        game.liquidConduitFlow = new ConduitFlow(ConduitType.Liquid, numCells, game.liquidConduitSystem, 10f, 0.75f);

        GridSettings.Reset(widthInCells, heightInCells);

        GameScenePartitioner.instance?.OnForcedCleanUp();
        var partitioner = worldGameObject.AddComponent<GameScenePartitioner>();
        partitioner.OnPrefabInit();
    }
}
