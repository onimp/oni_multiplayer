using System;
using System.Collections.Generic;
using MultiplayerMod.Game.Chores;
using MultiplayerMod.Game.Chores.States;
using MultiplayerMod.Multiplayer.Objects;
using NUnit.Framework;

namespace MultiplayerMod.Test.Game.Chores.States;

[TestFixture]
public class ChoreStateEventsTest : AbstractChoreTest {

    [SetUp]
    public void SetUp() {
        CreateTestData(new HashSet<Type> { typeof(ChoreStateEvents) });

        Assets.AnimTable[new HashedString(1525736797)] = new KAnimFile {
            data = new KAnimFileData("")
        };
        Grid.BuildMasks[123] = Grid.BuildFlags.Solid;
        StateMachine.Instance.error = false;
    }

    private static IEnumerable<object[]> TestArgs() {
        return GetTransitionTestArgs(ChoreList.StateTransitionConfig.TransitionTypeEnum.Exit);
    }

    [Test, TestCaseSource(nameof(TestArgs))]
    public void TestEventFiring(
        Type choreType,
        Func<object[]> choreArgsFunc,
        Func<Dictionary<int, object?>> expectedDictionaryFunc,
        ChoreList.StateTransitionConfig config
    ) {
        var chore = CreateChore(choreType, choreArgsFunc.Invoke());
        var choreId = new MultiplayerId(123);
        chore.Register(choreId);
        var smi = (StateMachine.Instance) chore.GetType().GetProperty("smi").GetValue(chore);
        smi.stateMachine.GetState("root").enterActions?.Clear();
        smi.stateMachine.GetState("root.delivering")?.enterActions?.Clear();
        var state = smi.stateMachine.GetState("root." + config.StateToMonitorName);
        state.enterActions?.Clear();
        chore.Begin(
            new Chore.Precondition.Context {
                consumerState = new ChoreConsumerState(Minion.GetComponent<ChoreConsumer>()),
                data = PickupableGameObject.GetComponent<Pickupable>()
            }
        );
        ChoreTransitStateArgs? firedArgs = null;
        ChoreStateEvents.OnStateTransition += args => firedArgs = args;
        smi.GoTo(state);

        smi.GoTo("root");

        var expectedDictionary = expectedDictionaryFunc.Invoke();
        Assert.NotNull(firedArgs);
        Assert.AreEqual(chore, firedArgs!.Chore);
        Assert.AreEqual("root", firedArgs!.TargetState);
        Assert.AreEqual(expectedDictionary.Keys, firedArgs!.Args.Keys);
        Assert.AreEqual(expectedDictionary.Values, firedArgs.Args.Values);
    }
}
