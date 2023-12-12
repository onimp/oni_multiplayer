using System;
using System.Collections.Generic;
using System.Linq;
using Klei.AI;
using MultiplayerMod.Game.Chores;
using NUnit.Framework;
using UnityEngine;

#pragma warning disable CS8974 // Converting method group to non-delegate type

namespace MultiplayerMod.Test.Game.Chores;

public class AbstractChoreTest : AbstractGameTest {

    private static KMonoBehaviour target = null!;
    private static GameObject gameObject = null!;
    private static ChoreType choreType = null!;
    private static KPrefabID kPrefabID = null!;
    private static MedicinalPillWorkable medicinalPillWorkable = null!;

    private static Death death = null!;

    private static Storage storage = null!;
    private static Constructable constructable = null!;
    private static TestMonoBehaviour testMonoBehaviour = null!;
    private static Db db = null!;

    protected new static void SetUpGame(HashSet<Type>? additionalPatches = null) {
        AbstractGameTest.SetUpGame(additionalPatches);

        target = createGameObject().GetComponent<KMonoBehaviour>();
        var targetGameObject = target.gameObject;
        targetGameObject.AddComponent<ChoreProvider>();
        targetGameObject.AddComponent<KPrefabID>();
        targetGameObject.AddComponent<MeshRenderer>();
        targetGameObject.AddComponent<Prioritizable>();
        targetGameObject.AddComponent<KBatchedAnimController>();
        targetGameObject.AddComponent<Effects>();
        targetGameObject.AddComponent<Modifiers>().Awake();
        targetGameObject.AddComponent<PathProber>();
        targetGameObject.AddComponent<Facing>();
        targetGameObject.AddComponent<StateMachineController>();
        targetGameObject.AddComponent<KSelectable>();
        Assets.PrefabsByTag[(Tag) TargetLocator.ID] = targetGameObject.GetComponent<KPrefabID>();
        Assets.PrefabsByTag[(Tag) MinionAssignablesProxyConfig.ID] =
            createGameObject().AddComponent<MinionAssignablesProxy>().gameObject.AddComponent<KPrefabID>();
        var navigator = targetGameObject.AddComponent<Navigator>();
        navigator.NavGridName = MinionConfig.MINION_NAV_GRID_NAME;
        navigator.CurrentNavType = NavType.Floor;

        navigator.Awake();
        navigator.Start();
        navigator.SetAbilities(new MinionPathFinderAbilities(navigator));
        targetGameObject.AddComponent<MinionIdentity>().Awake();
        targetGameObject.GetComponent<MinionIdentity>().Start();

        var sensors = targetGameObject.AddComponent<Sensors>();
        sensors.Add(new SafeCellSensor(sensors));
        sensors.Add(new IdleCellSensor(sensors));
        sensors.Add(new MingleCellSensor(sensors));

        gameObject = createGameObject();
        gameObject.AddComponent<Pickupable>();
        gameObject.AddComponent<PrimaryElement>();

        db = Db.Get();

        choreType = db.ChoreTypes.Astronaut;
        kPrefabID = createGameObject().AddComponent<KPrefabID>();
        kPrefabID.gameObject.AddComponent<SkillPerkMissingComplainer>(); // required for RancherChore
        medicinalPillWorkable = createGameObject().AddComponent<MedicinalPillWorkable>();
        medicinalPillWorkable.Awake();
        death = db.Deaths.Frozen;

        storage = createGameObject().AddComponent<Storage>();
        storage.Awake();

        constructable = createGameObject().AddComponent<Constructable>();

        testMonoBehaviour = createGameObject().AddComponent<TestMonoBehaviour>();
    }

    [TearDown]
    public void Teardown() {
        KPrefabIDTracker.Instance = null;
    }

    protected static object[][] GetCreationTestArgs() =>
        GetTestArgs().Where(
            testArgs => ChoreList.Config[(Type) testArgs[0]].CreationSync == ChoreList.CreationStatusEnum.On
        ).Select(testArgs => new[] { testArgs[0], testArgs[1] }).ToArray();

    protected static object[]?[] GetTransitionTestArgs() =>
        GetTestArgs().Where(
            testArgs => ChoreList.Config[(Type) testArgs[0]].StateTransitionSync.Status ==
                        ChoreList.StateTransitionConfig.SyncStatus.On
        ).ToArray();

    protected static Chore CreateChore(Type choreType, object?[] args) {
        return (Chore) choreType.GetConstructors()[0].Invoke(args);
    }

    private static object[][] GetTestArgs() {
        var notification = new Notification("", NotificationType.Bad);
        var statusItem = new StatusItem("", "");

        var testArgs = new[] {
            new object[] {
                typeof(AttackChore),
                new Func<object[]>(() => new object[] { target, gameObject })
            },
            new object[] {
                typeof(DeliverFoodChore),
                new Func<object[]>(() => new object[] { target })
            },
            new object[] {
                typeof(DieChore),
                new Func<object[]>(() => new object[] { target, death })
            },
            new object[] {
                typeof(DropUnusedInventoryChore),
                new Func<object[]>(() => new object[] { choreType, target })
            },
            new object[] {
                typeof(EquipChore),
                new Func<object[]>(() => new object[] { target })
            },
            new object[] {
                typeof(FixedCaptureChore),
                new Func<object[]>(() => new object[] { kPrefabID })
            },
            new object[] {
                typeof(MoveToQuarantineChore),
                new Func<object[]>(() => new object[] { target, target })
            },
            new object[] {
                typeof(PartyChore),
                new Func<object?[]>(
                    () => new object?[] {
                        target, medicinalPillWorkable, constructable.UpdateBuildState, constructable.UpdateBuildState,
                        constructable.UpdateBuildState
                    }
                )
            },
            new object[] {
                typeof(PeeChore),
                new Func<object[]>(() => new object[] { target })
            },
            new object[] {
                typeof(PutOnHatChore),
                new Func<object[]>(() => new object[] { target, choreType })
            },
            new object[] {
                typeof(RancherChore),
                new Func<object[]>(() => new object[] { kPrefabID })
            },
            new object[] {
                typeof(ReactEmoteChore),
                new Func<object?[]>(
                    () => new object?[]
                        { target, choreType, null, new HashedString(1), null, KAnim.PlayMode.Loop, null }
                )
            },
            new object[] {
                typeof(RescueIncapacitatedChore),
                new Func<object[]>(() => new object[] { target, gameObject })
            },
            new object[] {
                typeof(RescueSweepBotChore),
                new Func<object[]>(() => new object[] { target, gameObject, gameObject })
            },
            new object[] {
                typeof(SighChore),
                new Func<object[]>(() => new object[] { target })
            },
            new object[] {
                typeof(StressIdleChore),
                new Func<object[]>(() => new object[] { target })
            },
            new object[] {
                typeof(SwitchRoleHatChore),
                new Func<object[]>(() => new object[] { target, choreType })
            },
            new object[] {
                typeof(TakeMedicineChore),
                new Func<object[]>(() => new object[] { medicinalPillWorkable })
            },
            new object[] {
                typeof(TakeOffHatChore),
                new Func<object[]>(() => new object[] { target, choreType })
            },
            new object[] {
                typeof(UglyCryChore),
                new Func<object?[]>(() => new object?[] { choreType, target, constructable.UpdateBuildState })
            },
            new object[] {
                typeof(WaterCoolerChore),
                new Func<object?[]>(
                    () => new object?[] {
                        target, medicinalPillWorkable, constructable.UpdateBuildState, constructable.UpdateBuildState,
                        constructable.UpdateBuildState
                    }
                )
            },
            new object[] {
                typeof(WorkChore<>),
                new Func<object?[]>(
                    () => new object?[] {
                        choreType, medicinalPillWorkable, GlobalChoreProvider.Instance, true, null, null, null, true,
                        null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true
                    }
                )
            },
            new object[] {
                typeof(AggressiveChore),
                new Func<object?[]>(() => new object?[] { target, ChoreCallback }),
                new object[] { "root.move_notarget", new Dictionary<int, object?> { { 2, 84 }, { 1, null } } }
            },
            new object[] {
                typeof(BeIncapacitatedChore),
                new Func<object[]>(() => new object[] { target })
            },
            new object[] {
                typeof(BingeEatChore),
                new Func<object?[]>(() => new object?[] { target, ChoreCallback }),
                new object[] { "root.cantFindFood", new Dictionary<int, object?> { { 1, null } } }
            },
            new object[] {
                typeof(EatChore),
                new Func<object[]>(() => new object[] { target })
            },
            new object[] {
                typeof(EmoteChore),
                new Func<object?[]>(
                    () => new object?[]
                        { target, choreType, db.Emotes.Minion.Cheer, 1, testMonoBehaviour.TestStressEmoteFunc }
                )
            },
            new object[] {
                typeof(EntombedChore),
                new Func<object[]>(() => new object[] { target, "override" })
            },
            new object[] {
                typeof(FetchAreaChore),
                new Func<object[]>(() => new object[] { new Chore.Precondition.Context() })
            },
            new object[] {
                typeof(FleeChore),
                new Func<object[]>(() => new object[] { target, gameObject }),
                new object[] { "root.cower", new Dictionary<int, object?> { { 1, null } } }
            },
            new object[] {
                typeof(FoodFightChore),
                new Func<object[]>(() => new object[] { target, gameObject })
            },
            new object[] {
                typeof(MoveChore),
                new Func<object[]>(
                    () => new object[] { target, choreType, new Func<MoveChore.StatesInstance, int>(_ => 15), false }
                )
            },
            new object[] {
                typeof(MournChore),
                new Func<object[]>(() => new object[] { target })
            },
            new object[] {
                typeof(MovePickupableChore),
                new Func<object?[]>(() => new object?[] { target, gameObject, ChoreCallback })
            },
            new object[] {
                typeof(MoveToSafetyChore),
                new Func<object[]>(() => new object[] { target })
            },
            new object[] {
                typeof(RecoverBreathChore),
                new Func<object[]>(() => new object[] { target })
            },
            new object[] {
                typeof(SleepChore),
                new Func<object[]>(() => new object[] { choreType, target, gameObject, false, false })
            },
            new object[] {
                typeof(StressEmoteChore),
                new Func<object?[]>(
                    () => new object?[] {
                        target, choreType, new HashedString(1), new HashedString[] { new(2) },
                        KAnim.PlayMode.Paused, testMonoBehaviour.TestStressEmoteFunc
                    }
                )
            },
            new object[] {
                typeof(BalloonArtistChore),
                new Func<object[]>(() => new object[] { target })
            },
            new object[] {
                typeof(BansheeChore),
                new Func<object?[]>(() => new object?[] { choreType, target, null, null })
            },
            new object[] {
                typeof(FetchChore),
                new Func<object?[]>(
                    () => new object?[] {
                        choreType, storage, 0f, new HashSet<Tag>(), FetchChore.MatchCriteria.MatchTags, new Tag(),
                        Array.Empty<Tag>(), target.GetComponent<ChoreProvider>(), true, ChoreCallback, ChoreCallback,
                        ChoreCallback,
                        Operational.State.Operational, 0
                    }
                )
            },
            new object[] {
                typeof(IdleChore),
                new Func<object[]>(() => new object[] { target })
            },
            new object[] {
                typeof(MingleChore),
                new Func<object[]>(() => new object[] { target })
            },
            new object[] {
                typeof(VomitChore),
                new Func<object?[]>(() => new object?[] { choreType, target, statusItem, notification, null })
            },
        };
        var expectedChoreTypes = ChoreList.Config.Keys
            .OrderBy(a => a.FullName)
            .ToList();
        var actualChoreTypes = testArgs.Select(config => (Type) config[0]).OrderBy(a => a.FullName).ToList();
        Assert.AreEqual(expectedChoreTypes, actualChoreTypes);
        return testArgs;
    }

    private class TestMonoBehaviour : KMonoBehaviour {

        public StatusItem TestStressEmoteFunc() {
            return new StatusItem("", "");
        }
    }

    private static void ChoreCallback(Chore _) { }
}
