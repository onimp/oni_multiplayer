using System;
using System.Collections.Generic;
using MultiplayerMod.Game.Chores.States;
using MultiplayerMod.Game.Chores.Types;
using NUnit.Framework;
using UnityEngine;

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
        Minion.transform.position = new Vector3(12, 0.3f, 0);
    }

    [Test, TestCaseSource(nameof(ExitTestArgs))]
    public void StateExit_FiresEvent(
        Type choreType,
        Func<object[]> choreArgsFunc,
        Func<Dictionary<int, object?>> expectedDictionaryFunc,
        StateTransitionConfig config
    ) {
        var chore = CreateChore(choreType, choreArgsFunc.Invoke());
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
