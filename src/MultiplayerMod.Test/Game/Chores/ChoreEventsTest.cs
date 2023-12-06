using System;
using Klei.AI;
using MultiplayerMod.Game.Chores;
using NUnit.Framework;
using UnityEngine;

namespace MultiplayerMod.Test.Game.Chores;

[TestFixture]
public class ChoreEventsTest : AbstractGameTest {

    private static KMonoBehaviour target = null!;
    private static GameObject gameObject = null!;
    private static ChoreType choreType = null!;
    private static KPrefabID kPrefabID = null!;
    private static MedicinalPillWorkable medicinalPillWorkable = null!;
    private static Death death = null!;
    private static Emote emote = null!;

    [SetUp]
    public new void SetUp() {
        patches.Add(typeof(ChoreEvents));
        base.SetUp();

        target = createGameObject().GetComponent<KMonoBehaviour>();
        target.gameObject.AddComponent<ChoreProvider>();
        gameObject = createGameObject();
        choreType = Db.Get().ChoreTypes.Astronaut;
        kPrefabID = createGameObject().AddComponent<KPrefabID>();
        kPrefabID.gameObject.AddComponent<SkillPerkMissingComplainer>(); // required for RancherChore
        medicinalPillWorkable = createGameObject().AddComponent<MedicinalPillWorkable>();
        medicinalPillWorkable.Awake();
        death = Db.Get().Deaths.Frozen;
        emote = Db.Get().Emotes.Minion.Cheer;
    }

    [Test, TestCaseSource(nameof(GetTestArgs))]
    public void TestEventFiring(System.Action createNewChore, Type choreType, Func<object?[]> expectedArgsFunc) {
        CreateNewChoreArgs? firedArgs = null;
        ChoreEvents.CreateNewChore += args => firedArgs = args;

        createNewChore.Invoke();

        Assert.NotNull(firedArgs);
        Assert.AreEqual(choreType, firedArgs!.ChoreType);
        var expected = expectedArgsFunc.Invoke();
        Assert.AreEqual(expected, firedArgs.Args);
    }

    // ReSharper disable ObjectCreationAsStatement
    private static object[][] GetTestArgs() {
        // var cellCallback = new Func<MoveChore.StatesInstance, int>(_ => 5);

        var testArgs = new[] {
            new object[] {
                new System.Action(() => new AttackChore(target, gameObject)),
                typeof(AttackChore),
                new Func<object[]>(() => new object[] { target, gameObject })
            },

            new object[] {
                new System.Action(() => new DeliverFoodChore(target)),
                typeof(DeliverFoodChore),
                new Func<object[]>(() => new object[] { target })
            },
            new object[] {
                new System.Action(() => new DieChore(target, death)),
                typeof(DieChore),
                new Func<object[]>(() => new object[] { target, death })
            },
            new object[] {
                new System.Action(() => new DropUnusedInventoryChore(choreType, target)),
                typeof(DropUnusedInventoryChore),
                new Func<object[]>(() => new object[] { choreType, target })
            },
            // new object[] {
            //     new System.Action(() => new EmoteChore(target, choreType, emote)),
            //     typeof(EmoteChore),
            //     new Func<object?[]>(() => new object?[] { target, choreType, emote, 1, null })
            // },
            // new object[] {
            //     new System.Action(() => new EquipChore(target)),
            //     typeof(EquipChore),
            //     new Func<object[]>(() => new object[] { target })
            // },
            // new object[] {
            //     new System.Action(() => new FixedCaptureChore(kPrefabID)),
            //     typeof(FixedCaptureChore),
            //     new Func<object[]>(() => new object[] { kPrefabID })
            // },
            // new object[] {
            //     new System.Action(() => new MoveChore(target, choreType, cellCallback)),
            //     typeof(MoveChore),
            //     new Func<object[]>(() => new object[] { target, choreType, cellCallback, false })
            // },
            new object[] {
                new System.Action(() => new MoveToQuarantineChore(target, target)),
                typeof(MoveToQuarantineChore),
                new Func<object[]>(() => new object[] { target, target })
            },
            new object[] {
                new System.Action(() => new PartyChore(target, medicinalPillWorkable)),
                typeof(PartyChore),
                new Func<object?[]>(() => new object?[] { target, medicinalPillWorkable, null, null, null })
            },
            new object[] {
                new System.Action(() => new PeeChore(target)),
                typeof(PeeChore),
                new Func<object[]>(() => new object[] { target })
            },
            new object[] {
                new System.Action(() => new PutOnHatChore(target, choreType)),
                typeof(PutOnHatChore),
                new Func<object[]>(() => new object[] { target, choreType })
            },
            // new object[] {
            //     new System.Action(() => new RancherChore(kPrefabID)),
            //     typeof(RancherChore),
            //     new Func<object[]>(() => new object[] { kPrefabID })
            // },
            // new object[] {
            //     new System.Action(
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
            //     typeof(ReactEmoteChore),
            //     new Func<object?[]>(
            //         () => new object?[]
            //             { target, choreType, null, new HashedString(1), null, KAnim.PlayMode.Loop, null }
            //     )
            // },
            // new object[] {
            //     new System.Action(() => new RescueIncapacitatedChore(target, gameObject)),
            //     typeof(RescueIncapacitatedChore),
            //     new Func<object[]>(() => new object[] { target, gameObject })
            // },
            // new object[] {
            //     new System.Action(() => new RescueSweepBotChore(target, gameObject, gameObject)),
            //     typeof(RescueSweepBotChore),
            //     new Func<object[]>(() => new object[] { target, gameObject, gameObject })
            // },
            new object[] {
                new System.Action(() => new SighChore(target)),
                typeof(SighChore),
                new Func<object[]>(() => new object[] { target })
            },
            // new object[] {
            //     new System.Action(
            //         () => new StressEmoteChore(target, choreType, new HashedString(1), null, KAnim.PlayMode.Loop, null)
            //     ),
            //     typeof(StressEmoteChore),
            //     new Func<object?[]>(
            //         () => new object?[] { target, choreType, new HashedString(1), null, KAnim.PlayMode.Loop, null }
            //     )
            // },
            new object[] {
                new System.Action(() => new StressIdleChore(target)),
                typeof(StressIdleChore),
                new Func<object[]>(() => new object[] { target })
            },
            new object[] {
                new System.Action(() => new SwitchRoleHatChore(target, choreType)),
                typeof(SwitchRoleHatChore),
                new Func<object[]>(() => new object[] { target, choreType })
            },
            // new object[] {
            //     new System.Action(() => new TakeMedicineChore(medicinalPillWorkable)),
            //     typeof(TakeMedicineChore),
            //     new Func<object[]>(() => new object[] { medicinalPillWorkable })
            // },
            new object[] {
                new System.Action(() => new TakeOffHatChore(target, choreType)),
                typeof(TakeOffHatChore),
                new Func<object[]>(() => new object[] { target, choreType })
            },
            new object[] {
                new System.Action(() => new UglyCryChore(choreType, target)),
                typeof(UglyCryChore),
                new Func<object?[]>(() => new object?[] { choreType, target, null })
            },
            new object[] {
                new System.Action(() => new WaterCoolerChore(target, medicinalPillWorkable)),
                typeof(WaterCoolerChore),
                new Func<object?[]>(() => new object?[] { target, medicinalPillWorkable, null, null, null })
            },
            // new object[] {
            //     new System.Action(
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
        };
        Assert.AreEqual(ChoreList.DeterministicChores.Count, testArgs.Length);
        return testArgs;
    }
}
