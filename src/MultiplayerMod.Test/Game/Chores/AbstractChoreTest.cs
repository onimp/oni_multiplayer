using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Klei.AI;
using MultiplayerMod.Game.Chores;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Test.Environment.Patches;
using MultiplayerMod.Test.Multiplayer.Commands.Chores.Patches;
using NUnit.Framework;
using UnityEngine;

#pragma warning disable CS8974 // Converting method group to non-delegate type

namespace MultiplayerMod.Test.Game.Chores;

public class AbstractChoreTest : AbstractGameTest {
    private Harmony? harmony;

    protected static KMonoBehaviour Minion = null!;
    protected static GameObject PickupableGameObject = null!;
    private static ChoreType astronautChoreType = null!;
    private static MedicinalPillWorkable medicinalPillWorkable = null!;

    private static Death death = null!;

    private static Storage storage = null!;
    private static Constructable constructable = null!;
    private static TestMonoBehaviour testMonoBehaviour = null!;
    private static Db db = null!;

    protected void CreateTestData(HashSet<Type>? additionalPatches = null) {
        if (additionalPatches != null) {
            harmony = new Harmony("AbstractChoreTest");
            PatchesSetup.Install(harmony, additionalPatches);
        }

        PickupableGameObject = CreatePickupableGameObject();
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

    private static KMonoBehaviour CreateMinion() {
        var minion = createGameObject().GetComponent<KMonoBehaviour>();
        var targetGameObject = minion.gameObject;
        targetGameObject.AddComponent<ChoreProvider>();
        targetGameObject.AddComponent<ChoreDriver>();
        targetGameObject.AddComponent<User>();
        targetGameObject.AddComponent<StateMachineController>();
        new RationMonitor.Instance(minion);
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
        targetGameObject.AddComponent<ConsumableConsumer>().forbiddenTagSet = new HashSet<Tag>();
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
        targetGameObject.AddComponent<OxygenBreather>();
        targetGameObject.AddComponent<MinionBrain>().Awake();
        targetGameObject.AddComponent<SkillPerkMissingComplainer>();

        var sensors = targetGameObject.AddComponent<Sensors>();
        sensors.Add(new SafeCellSensor(sensors));
        sensors.Add(new IdleCellSensor(sensors));
        sensors.Add(new MingleCellSensor(sensors));
        sensors.Add(new PickupableSensor(sensors));
        sensors.Add(new ClosestEdibleSensor(sensors));

        return minion;
    }

    [TearDown]
    public void Teardown() {
        KPrefabIDTracker.Instance = null;
        AssetsPatch.Clear();
        ChoreObjects.Clear();
        if (harmony != null) {
            PatchesSetup.Uninstall(harmony);
        }
    }

    protected static object[][] GetCreationTestArgs() =>
        GetTestArgs().Where(
            testArgs => ChoreList.Config[((Type) testArgs[0]).IsGenericType
                ? ((Type) testArgs[0]).GetGenericTypeDefinition()
                : ((Type) testArgs[0])].CreationSync == ChoreList.CreationStatusEnum.On
        ).Select(testArgs => new[] { testArgs[0], testArgs[1] }).ToArray();

    protected static IEnumerable<object[]> GetTransitionTestArgs(
        ChoreList.StateTransitionConfig.TransitionTypeEnum transitionTypeEnum
    ) =>
        GetTestArgs()
            .SelectMany(
                testArgs => {
                    if (testArgs.Length == 2) return Array.Empty<object[]>();

                    return ((object[]) testArgs[2]).Where(
                        it => ((ChoreList.StateTransitionConfig) ((object[]) it)[0]).TransitionType ==
                              transitionTypeEnum
                    ).Select(
                        it => new[] {
                            testArgs[0], testArgs[1], ((object[]) it)[1], ((object[]) it)[0]
                        }
                    ).ToArray();
                }
            );

    protected static Chore CreateChore(Type choreType, object[] args) {
        var result = (Chore) choreType.GetConstructors()[0].Invoke(args);
        result.Register();
        return result;
    }

    private static object[][] GetTestArgs() {
        var notification = new Notification("", NotificationType.Bad);
        var statusItem = new StatusItem("", "");

        var testArgs = new[] {
            new object[] {
                typeof(AttackChore),
                new Func<object[]>(() => new object[] { Minion, PickupableGameObject })
            },
            new object[] {
                typeof(DeliverFoodChore),
                new Func<object[]>(() => new object[] { Minion })
            },
            new object[] {
                typeof(DieChore),
                new Func<object[]>(() => new object[] { Minion, death })
            },
            new object[] {
                typeof(DropUnusedInventoryChore),
                new Func<object[]>(() => new object[] { astronautChoreType, Minion })
            },
            new object[] {
                typeof(EquipChore),
                new Func<object[]>(() => new object[] { Minion })
            },
            new object[] {
                typeof(FixedCaptureChore),
                new Func<object[]>(() => new object[] { Minion.GetComponent<KPrefabID>() })
            },
            new object[] {
                typeof(MoveToQuarantineChore),
                new Func<object[]>(() => new object[] { Minion, Minion })
            },
            new object[] {
                typeof(PartyChore),
                new Func<object[]>(
                    () => new object[] {
                        Minion, medicinalPillWorkable, constructable.UpdateBuildState, constructable.UpdateBuildState,
                        constructable.UpdateBuildState
                    }
                )
            },
            new object[] {
                typeof(PeeChore),
                new Func<object[]>(() => new object[] { Minion })
            },
            new object[] {
                typeof(PutOnHatChore),
                new Func<object[]>(() => new object[] { Minion, astronautChoreType })
            },
            new object[] {
                typeof(RancherChore),
                new Func<object[]>(() => new object[] { Minion.GetComponent<KPrefabID>() })
            },
            new object[] {
                typeof(ReactEmoteChore),
                new Func<object[]>(
                    () => new object[] {
                        Minion, astronautChoreType, null!, new HashedString(1),
                        new[] { new HashedString(2) }, KAnim.PlayMode.Loop, testMonoBehaviour.TestStressEmoteFunc
                    }
                )
            },
            new object[] {
                typeof(RescueIncapacitatedChore),
                new Func<object[]>(() => new object[] { Minion, PickupableGameObject })
            },
            new object[] {
                typeof(RescueSweepBotChore),
                new Func<object[]>(() => new object[] { Minion, PickupableGameObject, PickupableGameObject })
            },
            new object[] {
                typeof(SighChore),
                new Func<object[]>(() => new object[] { Minion })
            },
            new object[] {
                typeof(StressIdleChore),
                new Func<object[]>(() => new object[] { Minion })
            },
            new object[] {
                typeof(SwitchRoleHatChore),
                new Func<object[]>(() => new object[] { Minion, astronautChoreType })
            },
            new object[] {
                typeof(TakeMedicineChore),
                new Func<object[]>(() => new object[] { medicinalPillWorkable })
            },
            new object[] {
                typeof(TakeOffHatChore),
                new Func<object[]>(() => new object[] { Minion, astronautChoreType })
            },
            new object[] {
                typeof(UglyCryChore),
                new Func<object[]>(() => new object[] { astronautChoreType, Minion, constructable.UpdateBuildState })
            },
            new object[] {
                typeof(WaterCoolerChore),
                new Func<object[]>(
                    () => new object[] {
                        Minion, medicinalPillWorkable, constructable.UpdateBuildState, constructable.UpdateBuildState,
                        constructable.UpdateBuildState
                    }
                )
            },
            new object[] {
                typeof(WorkChore<MedicinalPillWorkable>),
                new Func<object[]>(
                    () => new object[] {
                        astronautChoreType, medicinalPillWorkable, Minion.GetComponent<ChoreProvider>(), true,
                        ChoreCallback,
                        ChoreCallback, ChoreCallback, true, db.ScheduleBlockTypes.Eat, false, true,
                        Assets.GetAnim("test"), false, true, true, PriorityScreen.PriorityClass.basic,
                        5, false, true
                    }
                )
            },
            new object[] {
                typeof(AggressiveChore),
                new Func<object[]>(() => new object[] { Minion, ChoreCallback }),
                new object[] {
                    new object[] {
                        ChoreList.Config[typeof(AggressiveChore)].StatesTransitionSync.StateTransitionConfigs[0],
                        new Func<Dictionary<int, object?>>(
                            () => new Dictionary<int, object?> { { 2, 0 }, { 1, null } }
                        )
                    }
                }
            },
            new object[] {
                typeof(BeIncapacitatedChore),
                new Func<object[]>(() => new object[] { Minion }),
                new object[] {
                    new object[] {
                        ChoreList.Config[typeof(BeIncapacitatedChore)].StatesTransitionSync.StateTransitionConfigs[0],
                        new Func<Dictionary<int, object?>>(
                            () => new Dictionary<int, object?> { { 0, null } }
                        )
                    }
                }
            },
            new object[] {
                typeof(BingeEatChore),
                new Func<object[]>(() => new object[] { Minion, ChoreCallback }),
                new object[] {
                    new object[] {
                        ChoreList.Config[typeof(BingeEatChore)].StatesTransitionSync.StateTransitionConfigs[0],
                        new Func<Dictionary<int, object?>>(
                            () => new Dictionary<int, object?> { { 1, null } }
                        )
                    }
                }
            },
            new object[] {
                typeof(EatChore),
                new Func<object[]>(() => new object[] { Minion }),
                new object[] {
                    new object[] {
                        ChoreList.Config[typeof(EatChore)].StatesTransitionSync.StateTransitionConfigs[0],
                        new Func<Dictionary<int, object?>>(() => new Dictionary<int, object?>())
                    },
                    new object[] {
                        ChoreList.Config[typeof(EatChore)].StatesTransitionSync.StateTransitionConfigs[1],
                        new Func<Dictionary<int, object?>>(() => new Dictionary<int, object?>())
                    },
                    new object[] {
                        ChoreList.Config[typeof(EatChore)].StatesTransitionSync.StateTransitionConfigs[2],
                        new Func<Dictionary<int, object?>>(() => new Dictionary<int, object?>())
                    }
                }
            },
            new object[] {
                typeof(EmoteChore),
                new Func<object[]>(
                    () => new object[]
                        { Minion, astronautChoreType, db.Emotes.Minion.Cheer, 1, testMonoBehaviour.TestStressEmoteFunc }
                )
            },
            new object[] {
                typeof(EntombedChore),
                new Func<object[]>(() => new object[] { Minion, "override" })
            },
            new object[] {
                typeof(FetchAreaChore),
                new Func<object[]>(
                    () => {
                        var fetchChore = new FetchChore(
                            astronautChoreType,
                            storage,
                            0f,
                            new HashSet<Tag>(),
                            FetchChore.MatchCriteria.MatchTags,
                            new Tag()
                        );
                        fetchChore.Register();
                        return new object[] {
                            new Chore.Precondition.Context {
                                chore = fetchChore,
                                consumerState = new ChoreConsumerState(Minion.GetComponent<ChoreConsumer>()) {
                                    choreProvider = Minion.GetComponent<ChoreProvider>()
                                },
                                masterPriority = new PrioritySetting(PriorityScreen.PriorityClass.basic, 5)
                            }
                        };
                    }
                ),
                new object[] {
                    new object[] {
                        ChoreList.Config[typeof(FetchAreaChore)].StatesTransitionSync.StateTransitionConfigs[0],
                        new Func<Dictionary<int, object?>>(
                            () => new Dictionary<int, object?> { { 4, null }, { 5, null } }
                        )
                    },
                    new object[] {
                        ChoreList.Config[typeof(FetchAreaChore)].StatesTransitionSync.StateTransitionConfigs[1],
                        new Func<Dictionary<int, object?>>(
                            () => new Dictionary<int, object?> { { 1, null }, { 2, null }, { 3, new float() } }
                        )
                    }
                }
            },
            new object[] {
                typeof(FleeChore),
                new Func<object[]>(() => new object[] { Minion, PickupableGameObject }),
                new object[] {
                    new object[] {
                        ChoreList.Config[typeof(FleeChore)].StatesTransitionSync.StateTransitionConfigs[0],
                        new Func<Dictionary<int, object?>>(() => new Dictionary<int, object?> { { 1, null! } })
                    }
                }
            },
            new object[] {
                typeof(MournChore),
                new Func<object[]>(
                    () => {
                        var grave = createGameObject().AddComponent<Grave>();
                        grave.burialTime = 0;
                        grave.transform.position = new Vector3(20, 0.5f, 0);
                        Components.Graves.Clear();
                        Components.Graves.Add(grave);
                        return new object[] { Minion };
                    }
                ),
                new object[] {
                    new object[] {
                        ChoreList.Config[typeof(MournChore)].StatesTransitionSync.StateTransitionConfigs[0],
                        new Func<Dictionary<int, object?>>(() => new Dictionary<int, object?> { { 1, null } })
                    }
                }
            },
            new object[] {
                typeof(FoodFightChore),
                new Func<object[]>(() => new object[] { Minion, PickupableGameObject }),
                new object[] {
                    new object[] {
                        ChoreList.Config[typeof(FoodFightChore)].StatesTransitionSync.StateTransitionConfigs[0],
                        new Func<Dictionary<int, object?>>(() => new Dictionary<int, object?> { { 3, null } })
                    }
                }
            },
            new object[] {
                typeof(MoveChore),
                new Func<object[]>(
                    () => new object[]
                        { Minion, astronautChoreType, new Func<MoveChore.StatesInstance, int>(_ => 15), false }
                )
            },
            new object[] {
                typeof(MovePickupableChore),
                new Func<object[]>(() => new object[] { Minion, PickupableGameObject, ChoreCallback }),
                new object[] {
                    new object[] {
                        ChoreList.Config[typeof(MovePickupableChore)].StatesTransitionSync.StateTransitionConfigs[0],
                        new Func<Dictionary<int, object?>>(
                            () => new Dictionary<int, object?>
                                { { 5, 1.0f }, { 1, PickupableGameObject }, { 4, 1.0f } }
                        )
                    }
                }
            },
            new object[] {
                typeof(MoveToSafetyChore),
                new Func<object[]>(() => new object[] { Minion }),
                new object[] {
                    new object[] {
                        ChoreList.Config[typeof(MoveToSafetyChore)].StatesTransitionSync.StateTransitionConfigs[0],
                        new Func<Dictionary<int, object?>>(() => new Dictionary<int, object?>())
                    }
                }
            },
            new object[] {
                typeof(RecoverBreathChore),
                new Func<object[]>(() => new object[] { Minion }),
                new object[] {
                    new object[] {
                        ChoreList.Config[typeof(RecoverBreathChore)].StatesTransitionSync.StateTransitionConfigs[0],
                        new Func<Dictionary<int, object?>>(() => new Dictionary<int, object?>())
                    }
                }
            },
            new object[] {
                typeof(SleepChore),
                new Func<object[]>(
                    () => new object[] { astronautChoreType, Minion, PickupableGameObject, false, false }
                ),
                new object[] {
                    new object[] {
                        ChoreList.Config[typeof(SleepChore)].StatesTransitionSync.StateTransitionConfigs[0],
                        new Func<Dictionary<int, object?>>(() => new Dictionary<int, object?>())
                    }
                }
            },
            new object[] {
                typeof(StressEmoteChore),
                new Func<object[]>(
                    () => new object[] {
                        Minion, astronautChoreType, new HashedString(1), new HashedString[] { new(2) },
                        KAnim.PlayMode.Paused, testMonoBehaviour.TestStressEmoteFunc
                    }
                ),
            },
            new object[] {
                typeof(BalloonArtistChore),
                new Func<object[]>(() => new object[] { Minion }),
                new object[] {
                    new object[] {
                        ChoreList.Config[typeof(BalloonArtistChore)].StatesTransitionSync.StateTransitionConfigs[0],
                        new Func<Dictionary<int, object?>>(() => new Dictionary<int, object?>())
                    }
                }
            },
            new object[] {
                typeof(BansheeChore),
                new Func<object[]>(() => new object[] { astronautChoreType, Minion, notification, ChoreCallback }),
                new object[] {
                    new object[] {
                        ChoreList.Config[typeof(BansheeChore)].StatesTransitionSync.StateTransitionConfigs[0],
                        new Func<Dictionary<int, object?>>(
                            () => new Dictionary<int, object?> { { 1, 0 } }
                        )
                    },
                    new object[] {
                        ChoreList.Config[typeof(BansheeChore)].StatesTransitionSync.StateTransitionConfigs[1],
                        new Func<Dictionary<int, object?>>(() => new Dictionary<int, object?>())
                    }
                }
            },
            new object[] {
                typeof(FetchChore),
                new Func<object[]>(
                    () => new object[] {
                        astronautChoreType, storage, 0f, new HashSet<Tag>(), FetchChore.MatchCriteria.MatchTags,
                        new Tag(),
                        Array.Empty<Tag>(), Minion.GetComponent<ChoreProvider>(), true, ChoreCallback, ChoreCallback,
                        ChoreCallback,
                        Operational.State.Operational, 0
                    }
                )
            },
            new object[] {
                typeof(IdleChore),
                new Func<object[]>(() => new object[] { Minion }),
                new object[] {
                    new object[] {
                        ChoreList.Config[typeof(IdleChore)].StatesTransitionSync.StateTransitionConfigs[0],
                        new Func<Dictionary<int, object?>>(() => new Dictionary<int, object?>())
                    }
                }
            },
            new object[] {
                typeof(MingleChore),
                new Func<object[]>(() => new object[] { Minion }),
                new object[] {
                    new object[] {
                        ChoreList.Config[typeof(MingleChore)].StatesTransitionSync.StateTransitionConfigs[0],
                        new Func<Dictionary<int, object?>>(() => new Dictionary<int, object?>())
                    },
                    new object[] {
                        ChoreList.Config[typeof(MingleChore)].StatesTransitionSync.StateTransitionConfigs[1],
                        new Func<Dictionary<int, object?>>(() => new Dictionary<int, object?>())
                    }
                }
            },
            new object[] {
                typeof(VomitChore),
                new Func<object[]>(
                    () => new object[] { astronautChoreType, Minion, statusItem, notification, ChoreCallback }
                ),
                new object[] {
                    new object[] {
                        ChoreList.Config[typeof(VomitChore)].StatesTransitionSync.StateTransitionConfigs[0],
                        new Func<Dictionary<int, object?>>(() => new Dictionary<int, object?>())
                    }
                }
            },
        };
        var expectedChoreTypes = ChoreList.Config.Keys
            .OrderBy(a => a.FullName)
            .ToList();
        var actualChoreTypes = testArgs.Select(config => (Type) config[0])
            .Select(type => type.IsGenericType ? type.GetGenericTypeDefinition() : type)
            .OrderBy(a => a.FullName).ToList();
        Assert.AreEqual(expectedChoreTypes, actualChoreTypes);
        foreach (var testArg in testArgs) {
            var choreType = (Type) testArg[0];
            var config = ChoreList.Config[choreType.IsGenericType ? choreType.GetGenericTypeDefinition() : choreType];
            if (config.StatesTransitionSync.Status == ChoreList.StatesTransitionConfig.SyncStatus.Off) {
                Assert.True(testArg.Length == 2, $"{testArg[0]} must contain 2 elements if state sync is off.");
            } else {
                Assert.True(testArg.Length == 3, $"{testArg[0]} must contain 3 elements if state sync is on.");
                var stateSyncTypes = ((object[]) testArg[2]).Select(it => ((object[]) it)[0]).ToArray();
                Assert.AreEqual(
                    config.StatesTransitionSync.StateTransitionConfigs,
                    stateSyncTypes,
                    $"{testArg[0]} must have all state sync args in the exact order."
                );
            }
        }
        return testArgs;
    }

    private class TestMonoBehaviour : KMonoBehaviour {

        public StatusItem TestStressEmoteFunc() {
            return new StatusItem("", "");
        }
    }

    private static void ChoreCallback(Chore _) { }
}
