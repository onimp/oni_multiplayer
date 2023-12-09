using System;
using System.Linq;
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

    //   private static Emote emote = null!;
    private static Storage storage = null!;
    private static Constructable constructable = null!;
    private static TestMonoBehaviour testMonoBehaviour = null!;
    private static Db db = null!;

    [SetUp]
    public new void SetUp() {
        base.SetUp();

        target = createGameObject().GetComponent<KMonoBehaviour>();
        target.gameObject.AddComponent<ChoreProvider>();
        target.gameObject.AddComponent<KPrefabID>();
        target.gameObject.AddComponent<MeshRenderer>();
        target.gameObject.AddComponent<Prioritizable>();

        var sensors = target.gameObject.AddComponent<Sensors>();
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
        //   emote = db.Emotes.Minion.Cheer;

        storage = createGameObject().AddComponent<Storage>();
        storage.Awake();

        constructable = createGameObject().AddComponent<Constructable>();

        testMonoBehaviour = createGameObject().AddComponent<TestMonoBehaviour>();
    }

    protected Chore CreateChore(Type choreType, object?[] args) {
        return (Chore) choreType.GetConstructors()[0].Invoke(args);
    }

    protected static object[][] GetTestArgs() {
        // var notification = new Notification("", NotificationType.Bad);
        // var statusItem = new StatusItem("", "");

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
            // new object[] {
            //     new Func<Chore>(() => new EquipChore(target)),
            //     typeof(EquipChore),
            //     new Func<object[]>(() => new object[] { target })
            // },
            // new object[] {
            //     new Func<Chore>(() => new FixedCaptureChore(kPrefabID)),
            //     typeof(FixedCaptureChore),
            //     new Func<object[]>(() => new object[] { kPrefabID })
            // },
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
            // new object[] {
            //     new Func<Chore>(() => new RancherChore(kPrefabID)),
            //     typeof(RancherChore),
            //     new Func<object[]>(() => new object[] { kPrefabID })
            // },
            // new object[] {
            //     typeof(ReactEmoteChore),
            //     new Func<Chore>(
            //         () => new ReactEmoteChore(
            //             target,
            //             choreType,
            //             null,
            //             new HashedString(1),
            //             null,
            //             KAnim.PlayMode.Loop,
            //             null
            //         )
            //     ),
            //     new Func<object?[]>(
            //         () => new object?[]
            //             { target, choreType, null, new HashedString(1), null, KAnim.PlayMode.Loop, null }
            //     )
            // },
            // new object[] {
            //     new Func<Chore>(() => new RescueIncapacitatedChore(target, gameObject)),
            //     typeof(RescueIncapacitatedChore),
            //     new Func<object[]>(() => new object[] { target, gameObject })
            // },
            // new object[] {
            //     new Func<Chore>(() => new RescueSweepBotChore(target, gameObject, gameObject)),
            //     typeof(RescueSweepBotChore),
            //     new Func<object[]>(() => new object[] { target, gameObject, gameObject })
            // },
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
            // new object[] {
            //     new Func<Chore>(() => new TakeMedicineChore(medicinalPillWorkable)),
            //     typeof(TakeMedicineChore),
            //     new Func<object[]>(() => new object[] { medicinalPillWorkable })
            // },
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
            // new object[] {
            //     new Func<Chore>(
            //         () => new WorkChore<MedicinalPillWorkable>(
            //             choreType,
            //             medicinalPillWorkable,
            //             GlobalChoreProvider.Instance
            //         )
            //     ),
            //     typeof(WorkChore<MedicinalPillWorkable>),
            //     new Func<object?[]>(
            //         () => new object?[] {
            //             choreType, medicinalPillWorkable, GlobalChoreProvider.Instance, true, null, null, null, true,
            //             null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true
            //         }
            //     )
            // }
            // new object[] {
            //     typeof(AggressiveChore),
            //     new Func<Chore>(() => new AggressiveChore(target, ChoreCallback)),
            //     new Func<object?[]>(() => new object?[] { target, ChoreCallback })
            // },
            // new object[] {
            //     typeof(BeIncapacitatedChore),
            //     new Func<Chore>(() => new BeIncapacitatedChore(target)),
            //     new Func<object[]>(() => new object[] { target })
            // },
            // new object[] {
            //     typeof(BingeEatChore),
            //     new Func<Chore>(() => new BingeEatChore(target, ChoreCallback)),
            //     new Func<object?[]>(() => new object?[] { target, ChoreCallback })
            // },
            // new object[] {
            //     typeof(EatChore),
            //     new Func<Chore>(() => new EatChore(target)),
            //     new Func<object[]>(() => new object[] { target })
            // },
            // new object[] {
            //     typeof(EmoteChore),
            //     new Func<Chore>(
            //         () => new EmoteChore(
            //             target,
            //             choreType,
            //             db.Emotes.Minion.Cheer,
            //             1,
            //             testMonoBehaviour.TestStressEmoteFunc
            //         )
            //     ),
            //     new Func<object?[]>(
            //         () => new object?[]
            //             { target, choreType, db.Emotes.Minion.Cheer, 1, testMonoBehaviour.TestStressEmoteFunc }
            //     )
            // },
            new object[] {
                typeof(EntombedChore),
                new Func<object[]>(() => new object[] { target, "override" })
            },
            // new object[] {
            //     new Func<Chore>(() => new FetchAreaChore(new Chore.Precondition.Context())),
            //     typeof(FetchAreaChore),
            //     new Func<object[]>(() => new object[] { new Chore.Precondition.Context() })
            // },
            // new object[] {
            //     typeof(FleeChore),
            //     new Func<Chore>(() => new FleeChore(target, gameObject)),
            //     new Func<object[]>(() => new object[] { target, gameObject })
            // },
            // new object[] {
            //     typeof(FoodFightChore),
            //     new Func<Chore>(() => new FoodFightChore(target, gameObject)),
            //     new Func<object[]>(() => new object[] { target, gameObject })
            // },
            // new object[] {
            //     typeof(MoveChore),
            //     new Func<Chore>(() => new MoveChore(target, choreType, _ => 15)),
            //     new Func<object[]>(() => new object[] { target, choreType, 15, false })
            // },
            // new object[] {
            //     typeof(MournChore),
            //     new Func<Chore>(() => new MournChore(target)),
            //     new Func<object[]>(() => new object[] { target })
            // },
            // new object[] {
            //     typeof(MovePickupableChore),
            //     new Func<Chore>(() => new MovePickupableChore(target, gameObject, ChoreCallback)),
            //     new Func<object?[]>(() => new object?[] { target, gameObject, ChoreCallback })
            // },
            // new object[] {
            //     typeof(MoveToSafetyChore),
            //     new Func<Chore>(() => new MoveToSafetyChore(target)),
            //     new Func<object[]>(() => new object[] { target })
            // },
            // new object[] {
            //     typeof(RecoverBreathChore),
            //     new Func<Chore>(() => new RecoverBreathChore(target)),
            //     new Func<object[]>(() => new object[] { target })
            // },
            // new object[] {
            //     typeof(SleepChore),
            //     new Func<Chore>(() => new SleepChore(choreType, target, gameObject, false, false)),
            //     new Func<object[]>(() => new object[] { choreType, target, gameObject, false, false })
            // },
            // new object[] {
            //     typeof(StressEmoteChore),
            //     new Func<Chore>(
            //         () => new StressEmoteChore(
            //             target,
            //             choreType,
            //             new HashedString(1),
            //             new HashedString[] { new(2) },
            //             KAnim.PlayMode.Paused,
            //             testMonoBehaviour.TestStressEmoteFunc
            //         )
            //     ),
            //     new Func<object?[]>(
            //         () => new object?[] {
            //             target, choreType, new HashedString(1), new HashedString[] { new(2) },
            //             KAnim.PlayMode.Paused, testMonoBehaviour.TestStressEmoteFunc
            //         }
            //     )
            // },
            // new object[] {
            //     typeof(BalloonArtistChore),
            //     new Func<Chore>(() => new BalloonArtistChore(target)),
            //     new Func<object[]>(() => new object[] { target })
            // },
            // new object[] {
            //     typeof(BansheeChore),
            //     new Func<Chore>(() => new BansheeChore(choreType, target, null!)),
            //     new Func<object?[]>(() => new object?[] { choreType, target, null, null })
            // },
            // new object[] {
            //     typeof(FetchChore),
            //     new Func<Chore>(
            //         () => new FetchChore(
            //             choreType,
            //             storage,
            //             0f,
            //             new HashSet<Tag>(),
            //             FetchChore.MatchCriteria.MatchTags,
            //             new Tag(),
            //             Array.Empty<Tag>(),
            //             target.GetComponent<ChoreProvider>(),
            //             true,
            //             ChoreCallback,
            //             ChoreCallback,
            //             ChoreCallback,
            //             Operational.State.Operational,
            //             0
            //         )
            //     ),
            //     new Func<object?[]>(
            //         () => new object?[] {
            //             choreType, storage, 0f, new HashSet<Tag>(), FetchChore.MatchCriteria.MatchTags, new Tag(),
            //             Array.Empty<Tag>(), target.GetComponent<ChoreProvider>(), true, ChoreCallback, ChoreCallback,
            //             ChoreCallback,
            //             Operational.State.Operational, 0
            //         }
            //     )
            // },
            // new object[] {
            //     typeof(IdleChore),
            //     new Func<Chore>(() => new IdleChore(target)),
            //     new Func<object[]>(() => new object[] { target })
            // },
            // new object[] {
            //     typeof(MingleChore),
            //     new Func<Chore>(() => new MingleChore(target)),
            //     new Func<object[]>(() => new object[] { target })
            // },
            // new object[] {
            //     typeof(VomitChore),
            //     new Func<Chore>(() => new VomitChore(choreType, target, statusItem, notification)),
            //     new Func<object?[]>(() => new object?[] { choreType, target, statusItem, notification, null })
            // },
        };
        var expectedChoreTypes = ChoreList.Config
            .Where(config => config.Value.CreationSync == ChoreList.CreationStatusEnum.On)
            .Select(config => config.Key)
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
