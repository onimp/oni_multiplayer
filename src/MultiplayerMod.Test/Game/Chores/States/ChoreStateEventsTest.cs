using System;
using System.Collections.Generic;
using System.Linq;
using Klei.AI;
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

    [Test, TestCaseSource(nameof(EnterTestArgs))]
    public void StateEnter_FiresEvent(
        Type choreType,
        Func<object[]> choreArgsFunc,
        Func<Dictionary<int, object?>> expectedDictionaryFunc,
        StateTransitionConfig config
    ) {
        var chore = CreateChore(choreType, choreArgsFunc.Invoke());
        var smi = (StateMachine.Instance) chore.GetType().GetProperty("smi").GetValue(chore);
        var state = config.GetMonitoredState(smi.stateMachine);
        state.defaultState = null;
        var amountInstance = new AmountInstance(Db.Get().Amounts.Calories, Minion.gameObject);
        amountInstance.maxAttribute.Attribute.BaseValue = 50;
        Minion.gameObject.GetAmounts().ModifierList.Add(amountInstance);
        chore.Begin(
            new Chore.Precondition.Context {
                consumerState = new ChoreConsumerState(Minion.GetComponent<ChoreConsumer>()),
                data = PickupableGameObject.GetComponent<Pickupable>()
            }
        );
        ChoreTransitStateArgs? firedArgs = null;
        ChoreStateEvents.OnStateEnter += args => firedArgs = args;

        smi.GoTo(state);

        var expectedDictionary = expectedDictionaryFunc.Invoke();
        Assert.NotNull(firedArgs);
        Assert.AreEqual(chore, firedArgs!.Chore);
        Assert.AreEqual(
            config.StateToMonitorName != "root" ? "root." + config.StateToMonitorName : config.StateToMonitorName,
            firedArgs!.TargetState
        );
        Assert.AreEqual(expectedDictionary.Keys, firedArgs!.Args.Keys);
        Assert.AreEqual(
            expectedDictionary.Values.Select(it => it?.GetType()).ToArray(),
            firedArgs.Args.Values.Select(it => it?.GetType()).ToArray()
        );
    }

    [Test, TestCaseSource(nameof(UpdateTestArgs))]
    public void StateUpdate_FiresEvent(
        Type choreType,
        Func<object[]> choreArgsFunc,
        Func<Dictionary<int, object?>> expectedDictionaryFunc,
        StateTransitionConfig config
    ) {
        var chore = CreateChore(choreType, choreArgsFunc.Invoke());
        var smi = (StateMachine.Instance) chore.GetType().GetProperty("smi").GetValue(chore);
        var state = config.GetMonitoredState(smi.stateMachine);
        chore.Begin(
            new Chore.Precondition.Context {
                consumerState = new ChoreConsumerState(Minion.GetComponent<ChoreConsumer>()),
                data = PickupableGameObject.GetComponent<Pickupable>()
            }
        );
        ChoreTransitStateArgs? firedArgs = null;
        ChoreStateEvents.OnStateUpdate += args => firedArgs = args;

        smi.GoTo(state);
        // There are 12 buckets and all should be advanced
        for (var i = 0; i < 12; i++) {
            Singleton<StateMachineUpdater>.Instance.simBucketGroups[2].AdvanceOneSubTick(0f);
        }

        var expectedDictionary = expectedDictionaryFunc.Invoke();
        Assert.NotNull(firedArgs);
        Assert.AreEqual(chore, firedArgs!.Chore);
        Assert.AreEqual(expectedDictionary.Keys, firedArgs!.Args.Keys);
        Assert.AreEqual(
            expectedDictionary.Values.Select(it => it?.GetType()).ToArray(),
            firedArgs.Args.Values.Select(it => it?.GetType()).ToArray()
        );
    }

    [Test, TestCaseSource(nameof(EventHandlerTestArgs))]
    public void StateEventHandler_FiresEvent(
        Type choreType,
        Func<object[]> choreArgsFunc,
        Func<Dictionary<int, object?>> expectedDictionaryFunc,
        StateTransitionConfig config
    ) {
        ProgressBarsConfig.Instance.progressBarPrefab = createGameObject();
        var chore = CreateChore(choreType, choreArgsFunc.Invoke());
        var smi = (StateMachine.Instance) chore.GetType().GetProperty("smi").GetValue(chore);
        var state = config.GetMonitoredState(smi.stateMachine);
        smi.stateMachine.GetState("root.sleep")?.enterActions?.Clear();
        var amountInstance = new AmountInstance(Db.Get().Amounts.Calories, Minion.gameObject);
        amountInstance.maxAttribute.Attribute.BaseValue = 50;
        Minion.gameObject.GetAmounts().ModifierList.Add(amountInstance);
        chore.Begin(
            new Chore.Precondition.Context {
                consumerState = new ChoreConsumerState(Minion.GetComponent<ChoreConsumer>()),
                data = PickupableGameObject.GetComponent<Pickupable>()
            }
        );
        ChoreTransitStateArgs? firedArgs = null;
        ChoreStateEvents.OnStateEventHandler += args => firedArgs = args;

        smi.GoTo(state);
        Minion.GetComponent<KMonoBehaviour>().Trigger((int) config.EventGameHash!);

        var expectedDictionary = expectedDictionaryFunc.Invoke();
        Assert.NotNull(firedArgs);
        Assert.AreEqual(chore, firedArgs!.Chore);
        Assert.AreEqual(expectedDictionary.Keys, firedArgs!.Args.Keys);
        Assert.AreEqual(expectedDictionary.Values, firedArgs.Args.Values);
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
        var state = config.GetMonitoredState(smi.stateMachine);
        state.enterActions?.Clear();
        chore.Begin(
            new Chore.Precondition.Context {
                consumerState = new ChoreConsumerState(Minion.GetComponent<ChoreConsumer>()),
                data = PickupableGameObject.GetComponent<Pickupable>()
            }
        );
        ChoreTransitStateArgs? firedArgs = null;
        ChoreStateEvents.OnStateExit += args => firedArgs = args;
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
