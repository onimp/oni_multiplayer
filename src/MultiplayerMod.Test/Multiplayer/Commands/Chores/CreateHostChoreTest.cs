using System;
using System.Linq;
using MultiplayerMod.Game.Chores;
using MultiplayerMod.Multiplayer.Commands.Chores;
using MultiplayerMod.Network;
using MultiplayerMod.Platform.Steam.Network.Messaging;
using NUnit.Framework;
using UnityEngine;

namespace MultiplayerMod.Test.Multiplayer.Commands.Chores;

[TestFixture]
public class CreateHostChoreTest : AbstractGameTest {

    private static KMonoBehaviour target = null!;
    private static GameObject gameObject = null!;
    private static ChoreType choreType = null!;
    private static Db db = null!;
    private static KPrefabID kPrefabId = null!;
    private static MedicinalPillWorkable medicineWorkable = null!;
    private static Constructable constructable = null!;
 //   private static TestMonoBehaviour testMonoBehaviour = null!;

    [SetUp]
    public new void SetUp() {
        base.SetUp();

        var targetGameObject = createGameObject();
        target = targetGameObject.GetComponent<KMonoBehaviour>();

        db = Db.Get();
        gameObject = createGameObject();
        kPrefabId = createGameObject().AddComponent<KPrefabID>();
        kPrefabId.gameObject.AddComponent<SkillPerkMissingComplainer>(); // required for RancherChore
        choreType = db.ChoreTypes.Astronaut;
        medicineWorkable = createGameObject().AddComponent<MedicinalPillWorkable>();
        medicineWorkable.Awake();
        constructable = createGameObject().AddComponent<Constructable>();
     //   testMonoBehaviour = createGameObject().AddComponent<TestMonoBehaviour>();
    }

    [Test, TestCaseSource(nameof(GetTestArgs))]
    public void ExecutionTest(Type _, Func<CreateNewChoreArgs> getTestArgsFunc) {
        var arg = getTestArgsFunc.Invoke();
        var command = new CreateHostChore(arg);

        command.Execute(null!);
    }

    [Test, TestCaseSource(nameof(GetTestArgs))]
    public void SerializationTest(Type _, Func<CreateNewChoreArgs> getTestArgsFunc) {
        var arg = getTestArgsFunc.Invoke();
        var command = new CreateHostChore(arg);
        var messageFactory = new NetworkMessageFactory();
        var messageProcessor = new NetworkMessageProcessor();
        NetworkMessage? networkMessage = null;

        var handles = messageFactory.Create(command, MultiplayerCommandOptions.SkipHost).ToArray();

        foreach (var messageHandle in handles) {
            networkMessage = messageProcessor.Process(1u, messageHandle);
        }
        Assert.AreEqual(command.GetType(), networkMessage?.Command.GetType());
        Assert.AreEqual(command.ChoreType, ((CreateHostChore) networkMessage!.Command).ChoreType);
        Assert.AreEqual(command.Args, ((CreateHostChore) networkMessage!.Command).Args);
    }

#pragma warning disable CS8974 // Converting method group to non-delegate type
    private static object[] GetTestArgs() {
        var result = new object[] {
            new object[] {
                typeof(AttackChore),
                new Func<CreateNewChoreArgs>(
                    () => new CreateNewChoreArgs(typeof(AttackChore), new object[] { target, gameObject })
                )
            },
            new object[] {
                typeof(DeliverFoodChore),
                new Func<CreateNewChoreArgs>(
                    () => new CreateNewChoreArgs(typeof(DeliverFoodChore), new object[] { target })
                )
            },
            new object[] {
                typeof(DieChore),
                new Func<CreateNewChoreArgs>(
                    () => new CreateNewChoreArgs(typeof(DieChore), new object[] { target, db.Deaths.Frozen })
                )
            },
            new object[] {
                typeof(DropUnusedInventoryChore),
                new Func<CreateNewChoreArgs>(
                    () => new CreateNewChoreArgs(typeof(DropUnusedInventoryChore), new object[] { choreType, target })
                )
            },
            // new object[] {
            //     typeof(EmoteChore),
            //     new Func<CreateNewChoreArgs>(
            //         () => new CreateNewChoreArgs(
            //             typeof(EmoteChore),
            //             new object[]
            //                 { target, choreType, db.Emotes.Minion.Cheer, 1, testMonoBehaviour.TestStressEmoteFunc }
            //         )
            //     )
            // },
            new object[] {
                typeof(EquipChore),
                new Func<CreateNewChoreArgs>(() => new CreateNewChoreArgs(typeof(EquipChore), new object[] { target }))
            },
            new object[] {
                typeof(FixedCaptureChore),
                new Func<CreateNewChoreArgs>(
                    () => new CreateNewChoreArgs(typeof(FixedCaptureChore), new object[] { kPrefabId })
                )
            },
            // new object[] {
            //     typeof(MoveChore),
            //     new Func<CreateNewChoreArgs>(
            //         () => new CreateNewChoreArgs(
            //             typeof(MoveChore),
            //             new object[] { target, choreType, testMonoBehaviour.TestMoveFunc, false }
            //         )
            //     )
            // },
            new object[] {
                typeof(MoveToQuarantineChore),
                new Func<CreateNewChoreArgs>(
                    () => new CreateNewChoreArgs(typeof(MoveToQuarantineChore), new object[] { target, target })
                )
            },
            new object[] {
                typeof(PartyChore),
                new Func<CreateNewChoreArgs>(
                    () => new CreateNewChoreArgs(
                        typeof(PartyChore),
                        new object[] {
                            target, medicineWorkable, constructable.UpdateBuildState, constructable.UpdateBuildState,
                            constructable.UpdateBuildState
                        }
                    )
                )
            },
            new object[] {
                typeof(PeeChore),
                new Func<CreateNewChoreArgs>(() => new CreateNewChoreArgs(typeof(PeeChore), new object[] { target }))
            },
            new object[] {
                typeof(PutOnHatChore),
                new Func<CreateNewChoreArgs>(
                    () => new CreateNewChoreArgs(typeof(PutOnHatChore), new object[] { target, choreType })
                )
            },
            new object[] {
                typeof(RancherChore),
                new Func<CreateNewChoreArgs>(
                    () => new CreateNewChoreArgs(typeof(RancherChore), new object[] { kPrefabId })
                )
            },
            // new Func<CreateNewChoreArgs>(
            //     () => new CreateNewChoreArgs(
            //         typeof(ReactEmoteChore),
            //         new object[] {
            //             target, choreType,
            //             new EmoteReactable(
            //                 gameObject,
            //                 (HashedString) "WorkPasserbyAcknowledgement",
            //                 Db.Get().ChoreTypes.Emote,
            //                 5,
            //                 5,
            //                 localCooldown: 600f
            //             ),
            //             new HashedString(1), new HashedString[] { new(2) },
            //             KAnim.PlayMode.Loop, null
            //         }
            //     )
            // ),
            new object[] {
                typeof(RescueIncapacitatedChore),
                new Func<CreateNewChoreArgs>(
                    () => new CreateNewChoreArgs(typeof(RescueIncapacitatedChore), new object[] { target, gameObject })
                )
            },
            new object[] {
                typeof(RescueSweepBotChore),
                new Func<CreateNewChoreArgs>(
                    () => new CreateNewChoreArgs(
                        typeof(RescueSweepBotChore),
                        new object[] { target, gameObject, gameObject }
                    )
                )
            },
            new object[] {
                typeof(SighChore),
                new Func<CreateNewChoreArgs>(() => new CreateNewChoreArgs(typeof(SighChore), new object[] { target }))
            },
            // new object[] {
            //     typeof(StressEmoteChore),
            //     new Func<CreateNewChoreArgs>(
            //         () => new CreateNewChoreArgs(
            //             typeof(StressEmoteChore),
            //             new object[] {
            //                 target, choreType, new HashedString(1), new HashedString[] { new(2) },
            //                 KAnim.PlayMode.Paused, testMonoBehaviour.TestStressEmoteFunc
            //             }
            //         )
            //     )
            // },
            new object[] {
                typeof(StressIdleChore),
                new Func<CreateNewChoreArgs>(
                    () => new CreateNewChoreArgs(typeof(StressIdleChore), new object[] { target })
                )
            },
            new object[] {
                typeof(SwitchRoleHatChore),
                new Func<CreateNewChoreArgs>(
                    () => new CreateNewChoreArgs(typeof(SwitchRoleHatChore), new object[] { target, choreType })
                )
            },
            new object[] {
                typeof(TakeMedicineChore),
                new Func<CreateNewChoreArgs>(
                    () => new CreateNewChoreArgs(typeof(TakeMedicineChore), new object[] { medicineWorkable })
                )
            },
            new object[] {
                typeof(TakeOffHatChore),
                new Func<CreateNewChoreArgs>(
                    () => new CreateNewChoreArgs(typeof(TakeOffHatChore), new object[] { target, choreType })
                )
            },

            new object[] {
                typeof(UglyCryChore),
                new Func<CreateNewChoreArgs>(
                    () => new CreateNewChoreArgs(
                        typeof(UglyCryChore),
                        new object[] { choreType, target, constructable.UpdateBuildState }
                    )
                )
            },
            new object[] {
                typeof(WaterCoolerChore),
                new Func<CreateNewChoreArgs>(
                    () => new CreateNewChoreArgs(
                        typeof(WaterCoolerChore),
                        new object[] {
                            target, medicineWorkable, constructable.UpdateBuildState, constructable.UpdateBuildState,
                            constructable.UpdateBuildState
                        }
                    )
                )
            },
            new object[] {
                typeof(WorkChore<MedicinalPillWorkable>),
                new Func<CreateNewChoreArgs>(
                    () => new CreateNewChoreArgs(
                        typeof(WorkChore<MedicinalPillWorkable>),
                        new object[] {
                            choreType, medicineWorkable, createGameObject().AddComponent<ChoreProvider>(), false,
                            constructable.UpdateBuildState, constructable.UpdateBuildState,
                            constructable.UpdateBuildState,
                            false,
                            db.ScheduleBlockTypes.Work, true, false,
                            Assets.GetAnim("anim_interacts_medicine_nuclear_kanim"), true, false, false,
                            PriorityScreen.PriorityClass.high, 6, true, false
                        }
                    )
                )
            }
        };
        Assert.AreEqual(result.Length, ChoreList.DeterministicChores.Count);

        return result;
    }
#pragma warning restore CS8974 // Converting method group to non-delegate type

    // private class TestMonoBehaviour : KMonoBehaviour {
    //
    //     public int TestMoveFunc(object obj) {
    //         return 0;
    //     }
    //
    //     public StatusItem TestStressEmoteFunc() {
    //         return new StatusItem("", "");
    //     }
    // }

}
