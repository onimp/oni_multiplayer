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
        SetUpGame(new HashSet<Type> { typeof(ChoreStateEvents) });

        Assets.AnimTable[new HashedString(1525736797)] = new KAnimFile() {
            data = new KAnimFileData("")
        };
        Grid.BuildMasks[123] = Grid.BuildFlags.Solid;
    }

    [Test, TestCaseSource(nameof(GetTransitionTestArgs))]
    public void TestEventFiring(Type choreType, Func<object[]> expectedArgsFunc, object[] expectedArgs) {
        ChoreTransitStateArgs? firedArgs = null;
        ChoreStateEvents.OnStateTransition += args => firedArgs = args;
        var chore = CreateChore(choreType, expectedArgsFunc.Invoke());
        var choreId = new MultiplayerId(123);
        chore.Register(choreId);
        var config = ChoreList.Config[choreType];
        var smi = (StateMachine.Instance) chore.GetType().GetProperty("smi").GetValue(chore);
        chore.Begin(
            new Chore.Precondition.Context {
                consumerState = new ChoreConsumerState(target.GetComponent<ChoreConsumer>())
            }
        );

        smi.GoTo("root." + config.StateTransitionSync.StateToMonitorName);

        Assert.NotNull(firedArgs);
        Assert.AreEqual(choreId, firedArgs!.ChoreId);
        Assert.AreEqual(expectedArgs[0], firedArgs!.TargetState);
        Assert.AreEqual(expectedArgs[1], firedArgs!.Args.Keys);
    }
}
