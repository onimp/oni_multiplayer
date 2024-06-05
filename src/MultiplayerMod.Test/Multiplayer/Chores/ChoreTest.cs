using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using Klei.AI;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Extensions;
using MultiplayerMod.Multiplayer.Chores;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Multiplayer.Objects.Extensions;
using MultiplayerMod.Multiplayer.StateMachines;
using MultiplayerMod.Multiplayer.StateMachines.Configuration;
using MultiplayerMod.Network;
using MultiplayerMod.Platform.Steam.Network.Messaging;
using MultiplayerMod.Test.Environment.Patches;
using MultiplayerMod.Test.GameRuntime;
using MultiplayerMod.Test.GameRuntime.Patches;
using NUnit.Framework;
using UnityEngine;
using Attribute = System.Attribute;

#pragma warning disable CS8974 // Converting method group to non-delegate type

namespace MultiplayerMod.Test.Multiplayer.Chores;

public abstract class ChoreTest : PlayableGameTest {
    private Harmony? harmony;

    protected static KMonoBehaviour Minion = null!;
    protected static GameObject PickupableGameObject = null!;
    private static GameObject sleepableGameObject = null!;
    private static ChoreType astronautChoreType = null!;
    private static MedicinalPillWorkable medicinalPillWorkable = null!;

    private static Death death = null!;

    private static Storage storage = null!;
    private static Constructable constructable = null!;
    private static FetchOrder2 fetchOrder2 = null!;
    private static TestMonoBehaviour testMonoBehaviour = null!;
    private static Db db = null!;

    [SetUp]
    public void AbstractSetUp() {
        Singleton<StateMachineManager>.Instance.Clear();
        Singleton<StateMachineUpdater>.Instance.Clear();
        StateMachine.Instance.error = false;
        CreateTestData();
    }

    [TearDown]
    public void AbstractTearDown() {
        Assert.False(StateMachine.Instance.error, "There was state machine error. Check log.");
    }

    [ConfigureDependencies, UsedImplicitly]
    private static void SetUp(DependencyContainerBuilder builder) {
        builder
            .AddType<StateMachinesPatcher>()
            .AddType<ChoresPatcher>();
    }

    protected class TestCasePatchesAttribute : Attribute;
    private static TestCustomizer testCasePatches = ResolveCustomizationProviders<TestCasePatchesAttribute>(
        methods => methods
            .Where(it => it.GetParameters().Length == 0)
            .Where(it => typeof(IEnumerable<Type>).IsAssignableFrom(it.ReturnType))
    );

    protected void CreateTestData() {
        var patches = testCasePatches.Invoke<IEnumerable<Type>>([]).SelectMany(it => it).ToHashSet();
        if (patches.Count > 0) {
            harmony = new Harmony(nameof(ChoreTest));
            PatchesSetup.Install(harmony, patches);
        }

        PickupableGameObject = CreatePickupableGameObject();
        sleepableGameObject = createGameObject();
        sleepableGameObject.AddComponent<Sleepable>();
        Minion = CreateMinion();
        db = Db.Get();
        astronautChoreType = db.ChoreTypes.Astronaut;
        medicinalPillWorkable = createGameObject().AddComponent<MedicinalPillWorkable>();
        medicinalPillWorkable.Awake();
        death = db.Deaths.Frozen;
        storage = createGameObject().AddComponent<Storage>();
        storage.Awake();
        constructable = createGameObject().AddComponent<Constructable>();
        testMonoBehaviour = createGameObject().AddComponent<TestMonoBehaviour>();

        constructable.fetchList = new FetchList2(storage, null!) {
            OnComplete = constructable.OnFetchListComplete
        };
        fetchOrder2 = new FetchOrder2(
            null!,
            [new("copper")],
            FetchChore.MatchCriteria.MatchTags,
            null!,
            null!,
            null!,
            2f
        ) {
            OnComplete = constructable.fetchList.OnFetchOrderComplete
        };
        constructable.fetchList.FetchOrders.Add(fetchOrder2);
    }

    private static GameObject CreatePickupableGameObject() {
        const int cell = 11;

        var gameObject = createGameObject();
        gameObject.AddComponent<KPrefabID>().AddTag(GameTags.Edible);
        gameObject.AddComponent<PrimaryElement>();
        gameObject.AddComponent<Edible>().foodInfo = new EdiblesManager.FoodInfo("", "", 10f, 1, 0f, 0f, 0f, false);
        var pickupable = gameObject.AddComponent<Pickupable>();
        OffsetTableTracker.navGrid.NavTable.SetValid(cell, NavType.Floor, true);
        pickupable.Awake();
        pickupable.UpdateCachedCell(cell);
        global::Game.Instance.fetchManager.Add(pickupable);
        return gameObject;
    }

    [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
    private static KMonoBehaviour CreateMinion() {
        var minion = createGameObject().GetComponent<KMonoBehaviour>();
        var targetGameObject = minion.gameObject;
        targetGameObject.AddComponent<ChoreProvider>();
        targetGameObject.AddComponent<ChoreDriver>();
        targetGameObject.AddComponent<User>();
        targetGameObject.AddComponent<StateMachineController>();
        new RationMonitor.Instance(minion);
        new BreathMonitor.Instance(minion);
        targetGameObject.AddComponent<ChoreConsumer>().Awake();
        targetGameObject.AddComponent<KPrefabID>();
        targetGameObject.AddComponent<MeshRenderer>();
        targetGameObject.AddComponent<Prioritizable>();
        targetGameObject.AddComponent<KBatchedAnimController>();
        targetGameObject.AddComponent<Effects>();
        targetGameObject.AddComponent<Modifiers>().Awake();
        targetGameObject.GetComponent<Modifiers>().attributes.Add(Db.Get().Attributes.CarryAmount);
        targetGameObject.AddComponent<PathProber>();
        targetGameObject.AddComponent<Facing>();
        targetGameObject.AddComponent<KSelectable>();
        targetGameObject.AddComponent<ConsumableConsumer>().forbiddenTagSet = [];
        targetGameObject.AddComponent<Worker>();

        Assets.PrefabsByTag[(Tag) TargetLocator.ID] = targetGameObject.GetComponent<KPrefabID>();
        Assets.PrefabsByTag[(Tag) MinionAssignablesProxyConfig.ID] =
            createGameObject().AddComponent<MinionAssignablesProxy>().gameObject.AddComponent<KPrefabID>();
        var locatorGameObject = createGameObject();
        locatorGameObject.AddComponent<Approachable>();
        locatorGameObject.AddComponent<KPrefabID>();
        Assets.PrefabsByTag[(Tag) ApproachableLocator.ID] = locatorGameObject.GetComponent<KPrefabID>();
        var navigator = targetGameObject.AddComponent<Navigator>();
        navigator.NavGridName = MinionConfig.MINION_NAV_GRID_NAME;
        navigator.CurrentNavType = NavType.Floor;
        navigator.Awake();
        navigator.Start();
        navigator.SetAbilities(new MinionPathFinderAbilities(navigator));
        minion.GetComponent<Navigator>().NavGrid.NavTable.SetValid(19, NavType.Floor, true);

        targetGameObject.AddComponent<MinionIdentity>().Awake();
        targetGameObject.GetComponent<MinionIdentity>().Start();
        var ownables = targetGameObject.GetComponent<MinionIdentity>().assignableProxy.Get().FindOrAdd<Ownables>();
        ownables.slots.Add(new OwnableSlotInstance(ownables, (OwnableSlot) Db.Get().AssignableSlots.MessStation));
        targetGameObject.AddComponent<OxygenBreather>();
        targetGameObject.AddComponent<MinionBrain>().Awake();
        targetGameObject.AddComponent<SkillPerkMissingComplainer>();

        var sensors = targetGameObject.AddComponent<Sensors>();
        sensors.Add(new SafeCellSensor(sensors));
        sensors.Add(new IdleCellSensor(sensors));
        sensors.Add(new MingleCellSensor(sensors));
        sensors.Add(new PickupableSensor(sensors));
        sensors.Add(new ClosestEdibleSensor(sensors));
        sensors.Add(new BalloonStandCellSensor(sensors));

        return minion;
    }

    [TearDown]
    public void Teardown() {
        KPrefabIDTracker.Instance = null;
        AssetsPatch.Clear();
        ResetGrid(Grid.WidthInCells, Grid.HeightInCells);
        if (harmony != null) {
            PatchesSetup.Uninstall(harmony);
        }
    }

    protected static ICollection<StateMachineConfiguration> StateMachineConfigurations;
    protected static HashSet<Type> SupportedChoreTypes;

    static ChoreTest() {
        var context = new StateMachineConfigurationContext(Dependencies);
        ChoresMultiplayerConfiguration.Configuration
            .Select(it => it.StatesConfigurer)
            .NotNull()
            .ForEach(it => it.Configure(context));
        StateMachineConfigurations = context.Configurations;
        SupportedChoreTypes = ChoresMultiplayerConfiguration.Configuration.Select(it => it.ChoreType).ToHashSet();
    }

    protected static object[][] ChoresInstantiationTestCases() => GetFactories()
        .Where(it => SupportedChoreTypes.Contains(it.Type))
        .Select(it => new object[] { it })
        .ToArray();

    // @formatter:off
    [SuppressMessage("ReSharper", "ArrangeObjectCreationWhenTypeNotEvident")]
    private static ChoreFactory[] GetFactories() {
        var notification = new Notification("", NotificationType.Bad);
        var statusItem = new StatusItem("", "");

        return [
            new(typeof(AttackChore), () => [Minion, PickupableGameObject]),
            new(typeof(DeliverFoodChore), () => [Minion]),
            new(typeof(DieChore), () => [Minion, death]),
            new(typeof(DropUnusedInventoryChore), () => [astronautChoreType, Minion]),
            new(typeof(EquipChore), () => [Minion]),
            new(typeof(FixedCaptureChore), () => [Minion.GetComponent<KPrefabID>()]),
            new(typeof(MoveToQuarantineChore), () => [Minion, Minion]),
            new(typeof(PartyChore), () => [
                Minion, medicinalPillWorkable, constructable.UpdateBuildState,
                constructable.UpdateBuildState,
                constructable.UpdateBuildState
            ]),
            new(typeof(PeeChore), () => [Minion]),
            new(typeof(PutOnHatChore), () => [Minion, astronautChoreType]),
            new(typeof(RancherChore), () => [Minion.GetComponent<KPrefabID>()]),
            new(typeof(ReactEmoteChore), () => [
                Minion, astronautChoreType, null!, new HashedString(1),
                new[] { new HashedString(2) }, KAnim.PlayMode.Loop, testMonoBehaviour.TestStressEmoteFunc
            ]),
            new(typeof(RescueIncapacitatedChore), () => [Minion, PickupableGameObject]),
            new (typeof(RescueSweepBotChore), () => [Minion, PickupableGameObject, PickupableGameObject]),
            new(typeof(SighChore), () => [Minion]),
            new(typeof(StressIdleChore), () => [Minion]),
            new(typeof(SwitchRoleHatChore), () => [Minion, astronautChoreType]),
            new(typeof(TakeMedicineChore), () => [medicinalPillWorkable]),
            new(typeof(TakeOffHatChore), () => [Minion, astronautChoreType]),
            new(typeof(UglyCryChore), () => [astronautChoreType, Minion, constructable.UpdateBuildState]),
            new(typeof(WaterCoolerChore), () => [
                Minion, medicinalPillWorkable, constructable.UpdateBuildState,
                constructable.UpdateBuildState,
                constructable.UpdateBuildState
            ]),
            new(typeof(WorkChore<MedicinalPillWorkable>), () => [
                astronautChoreType, medicinalPillWorkable, Minion.GetComponent<ChoreProvider>(), true,
                ChoreCallback,
                ChoreCallback, ChoreCallback, true, db.ScheduleBlockTypes.Eat, false, true,
                Assets.GetAnim("test"), false, true, true, PriorityScreen.PriorityClass.basic,
                5, false, true
            ]),
            new(typeof(AggressiveChore), () => [Minion, ChoreCallback]),
            new(typeof(BeIncapacitatedChore), () => [Minion]),
            new(typeof(BingeEatChore), () => [Minion, ChoreCallback]),
            new(typeof(EatChore), () => [Minion]),
            new(typeof(EmoteChore), () => [
                Minion, astronautChoreType, db.Emotes.Minion.Cheer, 1, testMonoBehaviour.TestStressEmoteFunc
            ]),
            new(typeof(EntombedChore), () => [Minion, "override"]),
            new(typeof(FetchAreaChore), () => {
                var fetchChore = new FetchChore(
                    astronautChoreType,
                    storage,
                    0f,
                    [],
                    FetchChore.MatchCriteria.MatchTags,
                    new Tag()
                );
                fetchChore.Register();
                return [
                    new Chore.Precondition.Context {
                        chore = fetchChore,
                        consumerState = new ChoreConsumerState(Minion.GetComponent<ChoreConsumer>()) {
                            choreProvider = Minion.GetComponent<ChoreProvider>()
                        },
                        masterPriority = new PrioritySetting(PriorityScreen.PriorityClass.basic, 5)
                    }
                ];
            }),
            new(typeof(FleeChore), () => [Minion, PickupableGameObject]),
            new(typeof(MournChore), () => {
                var grave = createGameObject().AddComponent<Grave>();
                grave.burialTime = 0;
                grave.transform.position = new Vector3(20, 0.5f, 0);
                Components.Graves.Clear();
                Components.Graves.Add(grave);
                return [Minion];
            }),
            new(typeof(FoodFightChore), () => [Minion, PickupableGameObject]),
            new(typeof(MoveChore), () => [
                Minion, astronautChoreType, new Func<MoveChore.StatesInstance, int>(_ => 15), false
            ]),
            new(typeof(MovePickupableChore), () => [Minion, PickupableGameObject, ChoreCallback]),
            new(typeof(MoveToSafetyChore), () => [Minion]),
            new(typeof(RecoverBreathChore), () => [Minion]),
            new(typeof(SleepChore), () => [astronautChoreType, Minion, sleepableGameObject, false, false]),
            new(typeof(StressEmoteChore), () => [
                Minion, astronautChoreType, new HashedString(1), new HashedString[] { new(2) },
                KAnim.PlayMode.Paused, testMonoBehaviour.TestStressEmoteFunc
            ]),
            new(typeof(BalloonArtistChore), () => [Minion]),
            new(typeof(BansheeChore), () => [astronautChoreType, Minion, notification, ChoreCallback]),
            new(typeof(FetchChore), () => [
                astronautChoreType, storage, 0f, new HashSet<Tag>(), FetchChore.MatchCriteria.MatchTags,
                new Tag(),
                Array.Empty<Tag>(),
                Minion.GetComponent<ChoreProvider>(),
                true,
                new Action<Chore>(fetchOrder2.OnFetchChoreComplete),
                new Action<Chore>(fetchOrder2.OnFetchChoreBegin),
                new Action<Chore>(fetchOrder2.OnFetchChoreEnd),
                Operational.State.Operational,
                0
            ]),
            new(typeof(IdleChore), () => [Minion]),
            new(typeof(MingleChore), () => [Minion]),
            new(typeof(VomitChore), () => [astronautChoreType, Minion, statusItem, notification, ChoreCallback])
        ];
    }
    // @formatter:on

    protected IMultiplayerCommand SerializeDeserializeCommand(IMultiplayerCommand command) {
        var messageFactory = new NetworkMessageFactory();
        var messageProcessor = new NetworkMessageProcessor();
        NetworkMessage? networkMessage = null;

        foreach (var messageHandle in messageFactory.Create(command, MultiplayerCommandOptions.SkipHost).ToArray()) {
            networkMessage = messageProcessor.Process(1u, messageHandle);
        }
        return networkMessage?.Command!;
    }

    private class TestMonoBehaviour : KMonoBehaviour {
        public StatusItem TestStressEmoteFunc() {
            return new StatusItem("", "");
        }
    }

    private static void ChoreCallback(Chore _) { }

    public record ChoreFactory(Type Type, Func<object[]> GetArguments) {

        public Chore Create() {
            var result = (Chore) Type.GetConstructors()[0].Invoke(GetArguments());
            result.Register();
            return result;
        }

        public override string ToString() => Type.GetSignature();

    }

}
