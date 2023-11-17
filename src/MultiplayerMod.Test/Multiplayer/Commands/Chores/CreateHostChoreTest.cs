using System;
using System.Linq;
using Database;
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
    }

    [Test, TestCaseSource(nameof(GetTestArgs))]
    public void ExecutionTest(Func<CreateNewChoreArgs> getTestArgsFunc) {
        var arg = getTestArgsFunc.Invoke();
        var command = new CreateHostChore(arg);

        command.Execute(null!);
    }

    [Test, TestCaseSource(nameof(GetTestArgs))]
    public void SerializationTest(Func<CreateNewChoreArgs> getTestArgsFunc) {
        var arg = getTestArgsFunc.Invoke();
        var command = new CreateHostChore(arg);
        var messageFactory = new NetworkMessageFactory();

        var handles = messageFactory.Create(command, MultiplayerCommandOptions.SkipHost).ToArray();

        Assert.NotNull(handles);
    }

    private static object[] GetTestArgs() {
        var result = new object[] {
            new Func<CreateNewChoreArgs>(
                () => new CreateNewChoreArgs(typeof(AttackChore), new object[] { target, gameObject })
            ),
            new Func<CreateNewChoreArgs>(
                () => new CreateNewChoreArgs(typeof(DeliverFoodChore), new object[] { target })
            ),
            new Func<CreateNewChoreArgs>(
                () => new CreateNewChoreArgs(typeof(DieChore), new object[] { target, db.Deaths.Frozen })
            ),
            new Func<CreateNewChoreArgs>(
                () => new CreateNewChoreArgs(typeof(DropUnusedInventoryChore), new object[] { choreType, target })
            ),
            // new Func<CreateNewChoreArgs>(
            //     () => new CreateNewChoreArgs(
            //         typeof(EmoteChore),
            //         new object?[] { target, choreType, db.Emotes.Minion.Cheer, 1, null }
            //     )
            // ),
            new Func<CreateNewChoreArgs>(() => new CreateNewChoreArgs(typeof(EquipChore), new object[] { target })),
            new Func<CreateNewChoreArgs>(
                () => new CreateNewChoreArgs(typeof(FixedCaptureChore), new object[] { kPrefabId })
            ),
            new Func<CreateNewChoreArgs>(
                () => new CreateNewChoreArgs(
                    typeof(MoveChore),
                    new object[] { target, choreType, new Func<MoveChore.StatesInstance, int>(_ => 0), false }
                )
            ),
            new Func<CreateNewChoreArgs>(
                () => new CreateNewChoreArgs(typeof(MoveToQuarantineChore), new object[] { target, target })
            ),
            // new Func<CreateNewChoreArgs>(
            //     () => new CreateNewChoreArgs(
            //         typeof(PartyChore),
            //         new object?[] { target, medicineWorkable, null, null, null }
            //     )
            // ),
            new Func<CreateNewChoreArgs>(() => new CreateNewChoreArgs(typeof(PeeChore), new object[] { target })),
            new Func<CreateNewChoreArgs>(
                () => new CreateNewChoreArgs(typeof(PutOnHatChore), new object[] { target, choreType })
            ),
            new Func<CreateNewChoreArgs>(
                () => new CreateNewChoreArgs(typeof(RancherChore), new object[] { kPrefabId })
            ),
            // new Func<CreateNewChoreArgs>(
            //     () => new CreateNewChoreArgs(
            //         typeof(ReactEmoteChore),
            //         new object?[] {
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
            new Func<CreateNewChoreArgs>(
                () => new CreateNewChoreArgs(typeof(RescueIncapacitatedChore), new object[] { target, gameObject })
            ),
            new Func<CreateNewChoreArgs>(
                () => new CreateNewChoreArgs(
                    typeof(RescueSweepBotChore),
                    new object[] { target, gameObject, gameObject }
                )
            ),
            new Func<CreateNewChoreArgs>(() => new CreateNewChoreArgs(typeof(SighChore), new object[] { target })),
            // new Func<CreateNewChoreArgs>(
            //     () => new CreateNewChoreArgs(
            //         typeof(StressEmoteChore),
            //         new object?[] {
            //             target, choreType, new HashedString(1), new HashedString[] { new(2) }, KAnim.PlayMode.Paused,
            //             null
            //         }
            //     )
            // ),
            new Func<CreateNewChoreArgs>(
                () => new CreateNewChoreArgs(typeof(StressIdleChore), new object[] { target })
            ),
            new Func<CreateNewChoreArgs>(
                () => new CreateNewChoreArgs(typeof(SwitchRoleHatChore), new object[] { target, choreType })
            ),
            new Func<CreateNewChoreArgs>(
                () => new CreateNewChoreArgs(typeof(TakeMedicineChore), new object[] { medicineWorkable })
            ),
            new Func<CreateNewChoreArgs>(
                () => new CreateNewChoreArgs(typeof(TakeOffHatChore), new object[] { target, choreType })
            ),
            // new Func<CreateNewChoreArgs>(
            //     () => new CreateNewChoreArgs(typeof(UglyCryChore), new object?[] { choreType, target, null })
            // ),
            // new Func<CreateNewChoreArgs>(
            //     () => new CreateNewChoreArgs(
            //         typeof(WaterCoolerChore),
            //         new object?[] { target, medicineWorkable, null, null, null }
            //     )
            // ),
            // new Func<CreateNewChoreArgs>(
            //     () => new CreateNewChoreArgs(
            //         typeof(WorkChore<MedicinalPillWorkable>),
            //         new object?[] {
            //             choreType, medicineWorkable, createGameObject().AddComponent<ChoreProvider>(), false, null,
            //             null, null, false, db.ScheduleBlockTypes.Work, true,
            //             false, Assets.GetAnim("anim_interacts_medicine_nuclear_kanim"), true, false, false,
            //             PriorityScreen.PriorityClass.high, 6, true, false
            //         }
            //     )
            // )
        };
        Assert.AreEqual(result.Length, ChoreList.DeterministicChores.Count);

        return result;
    }
}
