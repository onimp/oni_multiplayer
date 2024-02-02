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

    [Test, TestCaseSource(nameof(TransitionTestArgs))]
    public void StateTransition_FiresEvent(
        Type choreType,
        Func<object[]> choreArgsFunc,
        Func<Dictionary<int, object?>> expectedDictionaryFunc,
        StateTransitionConfig config
    ) {
        var chore = CreateChore(choreType, choreArgsFunc.Invoke());
        var smi = (StateMachine.Instance) chore.GetType().GetProperty("smi").GetValue(chore);
        smi.stateMachine.GetState("root").transitions?.Clear();
        var state = config.GetMonitoredState(smi.stateMachine);
        state.enterActions?.Clear();
        state.updateActions?.Clear();
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

    [Test, TestCaseSource(nameof(MoveToTestArgs))]
    public void MoveStateTransition_FiresExitEvent(
        Type choreType,
        Func<object[]> choreArgsFunc,
        Func<Dictionary<int, object?>> expectedDictionaryFunc,
        StateTransitionConfig config
    ) {
        var chore = CreateChore(choreType, choreArgsFunc.Invoke());
        var smi = (StateMachine.Instance) chore.GetType().GetProperty("smi").GetValue(chore);
        var state = config.GetMonitoredState(smi.stateMachine);
        smi.stateMachine.GetState("root").transitions?.Clear();
        state.enterActions?.Clear();
        state.updateActions?.Clear();
        MoveToArgs? firedArgs = null;
        ChoreStateEvents.OnExitMoveTo += args => firedArgs = args;
        smi.GoTo(state);

        smi.GoTo("root");

        Assert.NotNull(firedArgs);
        Assert.AreEqual(chore, firedArgs!.Chore);
        Assert.AreEqual("root", firedArgs!.TargetState);
    }

    [Test, TestCaseSource(nameof(MoveToTestArgs))]
    public void MoveStop_FiresExitEvent(
        Type choreType,
        Func<object[]> choreArgsFunc,
        Func<Dictionary<int, object?>> expectedDictionaryFunc,
        StateTransitionConfig config
    ) {
        var chore = CreateChore(choreType, choreArgsFunc.Invoke());
        var smi = (StateMachine.Instance) chore.GetType().GetProperty("smi").GetValue(chore);
        var state = config.GetMonitoredState(smi.stateMachine);
        smi.stateMachine.GetState("root").transitions?.Clear();
        state.enterActions?.Clear();
        state.updateActions?.Clear();
        MoveToArgs? firedArgs = null;
        ChoreStateEvents.OnExitMoveTo += args => firedArgs = args;
        smi.GoTo(state);

        smi.StopSM("test");

        Assert.NotNull(firedArgs);
        Assert.AreEqual(chore, firedArgs!.Chore);
        Assert.AreEqual(null, firedArgs!.TargetState);
    }

    [Test, TestCaseSource(nameof(MoveToTestArgs))]
    public void MoveStateTransition_FiresEnterEvent(
        Type choreType,
        Func<object[]> choreArgsFunc,
        Func<Dictionary<int, object?>> _,
        StateTransitionConfig config
    ) {
        Minion.transform.position = Grid.CellToPos(50);
        var sensors = Minion.GetComponent<Sensors>();
        sensors.sensors.Remove(sensors.GetSensor<BalloonStandCellSensor>());
        sensors.Add(new FakeBalloonStandCellSensor(sensors));
        sensors.sensors.Remove(sensors.GetSensor<MingleCellSensor>());
        sensors.Add(new FakeMingleCellSensor(sensors));
        sensors.sensors.Remove(sensors.GetSensor<IdleCellSensor>());
        sensors.Add(new FakeIdleCellSensor(sensors));
        var chore = CreateChore(choreType, choreArgsFunc.Invoke());
        var smi = (StateMachine.Instance) chore.GetType().GetProperty("smi").GetValue(chore);
        var state = config.GetMonitoredState(smi.stateMachine);
        state.transitions.Clear();
        smi.stateMachine.GetState("root").transitions?.Clear();
        MoveToArgs? firedArgs = null;
        ChoreStateEvents.OnEnterMoveTo += args => firedArgs = args;

        smi.GoTo(state);

        Assert.NotNull(firedArgs);
        Assert.AreEqual(chore, firedArgs!.Chore);
    }

    [Test, TestCaseSource(nameof(MoveToTestArgs))]
    public void MoveStateTransition_FiresUpdateEvent(
        Type choreType,
        Func<object[]> choreArgsFunc,
        Func<Dictionary<int, object?>> _,
        StateTransitionConfig config
    ) {
        var chore = CreateChore(choreType, choreArgsFunc.Invoke());
        var smi = (StateMachine.Instance) chore.GetType().GetProperty("smi").GetValue(chore);
        var state = config.GetMonitoredState(smi.stateMachine);
        if (!state.updateActions?.Any(it => it.buckets.Any(bucket => bucket.name.Equals("MoveTo()"))) ?? true) {
            return;
        }
        state.transitions.Clear();
        MoveToArgs? firedArgs = null;
        ChoreStateEvents.OnUpdateMoveTo += args => firedArgs = args;
        smi.GoTo(state);

        // There are 12 buckets and all should be advanced
        for (var i = 0; i < 12; i++) {
            Singleton<StateMachineUpdater>.Instance.simBucketGroups[2].AdvanceOneSubTick(0f);
        }

        Assert.NotNull(firedArgs);
        Assert.AreEqual(chore, firedArgs!.Chore);
        Assert.AreEqual(chore, firedArgs!.Chore);
    }

    private class FakeBalloonStandCellSensor : BalloonStandCellSensor {
        public FakeBalloonStandCellSensor(Sensors sensors) : base(sensors) { }

        public override void Update() {
            cell = 0;
        }
    }

    private class FakeMingleCellSensor : MingleCellSensor {
        public FakeMingleCellSensor(Sensors sensors) : base(sensors) { }

        public override void Update() {
            cell = 0;
        }
    }

    private class FakeIdleCellSensor : IdleCellSensor {
        public FakeIdleCellSensor(Sensors sensors) : base(sensors) { }

        public override void Update() {
            cell = 0;
        }
    }
}
