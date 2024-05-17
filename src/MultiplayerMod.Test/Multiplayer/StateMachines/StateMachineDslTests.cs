using System;
using System.Collections.Generic;
using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Core.Extensions;
using MultiplayerMod.Core.Patch.ControlFlow;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.ModRuntime.StaticCompatibility;
using MultiplayerMod.Multiplayer.StateMachines;
using MultiplayerMod.Multiplayer.StateMachines.Configuration;
using NUnit.Framework;
using UnityEngine;

namespace MultiplayerMod.Test.Multiplayer.StateMachines;

[TestFixture]
public class StateMachineDslTests : AbstractGameTest {

    private static readonly Harmony harmony = new("StateMachinesDSL");

    [OneTimeSetUp]
    public static void SetUp() {
        Dependencies.Get<DependencyContainer>().Register(
            new DependencyInfo(nameof(ControlFlowCustomizer), typeof(ControlFlowCustomizer), false)
        );
    }

    [TearDown]
    public void TearDownDsl() {
        harmony.UnpatchAll("StateMachinesDSL");
        StateMachineManager.Instance.stateMachines.Clear();
    }

    private static void RunStateMachinesPatcher(params StateMachineConfigurer[] configurers) {
        var dispatcher = new EventDispatcher();
        var patcher = new StateMachinesPatcher(dispatcher, harmony);
        configurers.ForEach(it => patcher.Register(it));
        dispatcher.Dispatch(new RuntimeReadyEvent(Runtime.Instance));
    }

    private static T CreateStateMachineInstance<T>() where T : StateMachine.Instance {
        var go = new GameObject();
        var target = go.AddComponent<TestTarget>();
        target.Awake();
        return (T) Activator.CreateInstance(typeof(T), [target]);
    }

    [Test]
    public void MustNotModifyOriginalStateMachine() {
        RunStateMachinesPatcher();
        var smi = CreateStateMachineInstance<TestStateMachine.Instance>();
        smi.StartSM();
        smi.StopSM("Done");
        Assert.AreEqual(
            expected: new[] { "+A", "-A", "+B", "+B.A", "-B.A", "-B" },
            actual: smi.sm.Trace.Get(smi)
        );
        var secondSmi = CreateStateMachineInstance<SecondTestStateMachine.Instance>();
        secondSmi.StartSM();
        secondSmi.StopSM("Done");
        Assert.AreEqual(
            expected: new[] { "+A", "-A" },
            actual: secondSmi.sm.Trace.Get(secondSmi)
        );
    }

    [Test]
    public void MustSuppressEnterAction() {
        RunStateMachinesPatcher(
            new StateMachineConfigurer<TestStateMachine, TestStateMachine.Instance, TestTarget, object>(
                dsl => { dsl.PreConfigure(sm => { dsl.Suppress(() => sm.StateA.Enter("", null)); }); }
            )
        );
        var smi = CreateStateMachineInstance<TestStateMachine.Instance>();
        smi.StartSM();
        smi.StopSM("Done");
        Assert.AreEqual(
            expected: new[] { "-A", "+B", "+B.A", "-B.A", "-B" },
            actual: smi.sm.Trace.Get(smi)
        );
    }

    [Test]
    public void CheckConfigurationPhases() {
        RunStateMachinesPatcher(
            new StateMachineConfigurer<TestStateMachine, TestStateMachine.Instance, TestTarget, object>(
                dsl => {
                    dsl.PreConfigure(
                        sm => {
                            Assert.IsNull(sm.StateA.enterActions);
                            Assert.IsNull(sm.StateA.exitActions);
                        }
                    );
                    dsl.PostConfigure(
                        sm => {
                            Assert.AreEqual(expected: 2, actual: sm.StateA.enterActions?.Count);
                            Assert.AreEqual(expected: 1, actual: sm.StateA.exitActions?.Count);
                        }
                    );
                }
            )
        );
        var smi = CreateStateMachineInstance<TestStateMachine.Instance>();
        smi.StartSM();
        smi.StopSM("Done");
    }

    [Test]
    public void MustFailOnIncorrectlyProducedActionPhase() {
        RunStateMachinesPatcher(
            new StateMachineConfigurer<TestStateMachine, TestStateMachine.Instance, TestTarget, object>(
                dsl => {
                    dsl.PreConfigure(
                        sm => {
                            Assert.IsNull(sm.StateA.enterActions);
                            Assert.IsNull(sm.StateA.exitActions);
                        }
                    );
                    dsl.PostConfigure(
                        sm => {
                            Assert.AreEqual(expected: 2, actual: sm.StateA.enterActions?.Count);
                            Assert.AreEqual(expected: 1, actual: sm.StateA.exitActions?.Count);
                        }
                    );
                }
            )
        );
        var smi = CreateStateMachineInstance<TestStateMachine.Instance>();
        smi.StartSM();
        smi.StopSM("Done");
    }

    [Test]
    public void MustAddNewBehavior() {
        RunStateMachinesPatcher(
            new StateMachineConfigurer<TestStateMachine, TestStateMachine.Instance, TestTarget, object>(
                dsl => {
                    dsl.PreConfigure(sm => { sm.StateA.Enter(smi => sm.Trace.Get(smi).Add("+X")); });
                    dsl.PostConfigure(sm => { sm.StateA.Exit(smi => sm.Trace.Get(smi).Add("-X")); });
                }
            )
        );
        var smi = CreateStateMachineInstance<TestStateMachine.Instance>();
        smi.StartSM();
        smi.StopSM("Done");
        Assert.AreEqual(
            expected: new[] { "+X", "+A", "-X", "-A", "+B", "+B.A", "-B.A", "-B" },
            actual: smi.sm.Trace.Get(smi)
        );
    }

    [Test]
    public void MustAggregateInlineConfigurations() {
        RunStateMachinesPatcher(
            new StateMachineConfigurer<SecondTestStateMachine, SecondTestStateMachine.Instance, TestTarget, object>(
                dsl => {
                    dsl.PreConfigure(sm => { sm.StateA.Enter("Test", smi => sm.Trace.Get(smi).Add("+X")); });
                    dsl.PostConfigure(sm => { sm.StateA.Exit("Test", smi => sm.Trace.Get(smi).Add("-X")); });
                }
            ),
            new StateMachineConfigurer<TestStateMachine, TestStateMachine.Instance, TestTarget, object>(
                dsl => {
                    dsl.Inline(
                        new StateMachineConfigurer<SecondTestStateMachine, SecondTestStateMachine.Instance, TestTarget, object>(
                            dsl => { dsl.PreConfigure(sm => { dsl.Suppress(() => sm.StateA.Enter(null, null)); }); }
                        )
                    );
                    dsl.PreConfigure(sm => { sm.StateA.Enter(smi => sm.Trace.Get(smi).Add("+X")); });
                    dsl.PostConfigure(sm => { sm.StateA.Exit(smi => sm.Trace.Get(smi).Add("-X")); });
                }
            )
        );
        var smi = CreateStateMachineInstance<TestStateMachine.Instance>();
        smi.StartSM();
        smi.StopSM("Done");
        Assert.AreEqual(
            expected: new[] { "+X", "+A", "-X", "-A", "+B", "+B.A", "-B.A", "-B" },
            actual: smi.sm.Trace.Get(smi)
        );
        var secondSmi = CreateStateMachineInstance<SecondTestStateMachine.Instance>();
        secondSmi.StartSM();
        secondSmi.StopSM("Done");
        Assert.AreEqual(
            expected: new[] { "+X", "-X", "-A" },
            actual: secondSmi.sm.Trace.Get(secondSmi)
        );
    }

    [Test]
    public void MustFailOnImproperUseOfInlineConfiguration() {
        RunStateMachinesPatcher(
            new StateMachineConfigurer<TestStateMachine, TestStateMachine.Instance, TestTarget, object>(
                dsl => {
                    dsl.PreConfigure(
                        sm => {
                            dsl.Inline(
                                new StateMachineConfigurer<SecondTestStateMachine, SecondTestStateMachine.Instance, TestTarget, object>(
                                    dsl => {
                                        dsl.PreConfigure(sm => { dsl.Suppress(() => sm.StateA.Enter(null, null)); });
                                    }
                                )
                            );
                            sm.StateA.Enter(smi => sm.Trace.Get(smi).Add("+X"));
                        }
                    );
                    dsl.PostConfigure(sm => { sm.StateA.Exit(smi => sm.Trace.Get(smi).Add("-X")); });
                }
            )
        );

        Assert.That(
            () => {
                var smi = CreateStateMachineInstance<TestStateMachine.Instance>();
                smi.StartSM();
                smi.StopSM("Done");
            },
            Throws.InnerException.InstanceOf<ConfigurationContextLockedException>()
        );
    }

    [Test]
    public void MustFailOnPhaseInconsistency() {
        RunStateMachinesPatcher(
            new StateMachineConfigurer<TestStateMachine, TestStateMachine.Instance, TestTarget, object>(
                dsl => { dsl.PostConfigure(sm => { dsl.Suppress(() => sm.StateA.Enter("", null)); }); }
            )
        );

        Assert.That(
            () => {
                var smi = CreateStateMachineInstance<TestStateMachine.Instance>();
                smi.StartSM();
                smi.StopSM("Done");
            },
            Throws.InnerException.InstanceOf<InvalidConfigurationPhaseException>()
        );
    }

    public class TestTarget : KMonoBehaviour;

    public class TestStateMachine : GameStateMachine<TestStateMachine, TestStateMachine.Instance, TestTarget, object> {

        public class SubStates : State {
            [UsedImplicitly]
            public State StateA = null!;
        }

        [UsedImplicitly]
        public State InitState = null!;

        [UsedImplicitly]
        public State StateA = null!;

        [UsedImplicitly]
        public SubStates StateB = null!;

        [UsedImplicitly]
        public ObjectParameter<List<string>> Trace = null!;

        // ReSharper disable once InconsistentNaming
        public override void InitializeStates(out BaseState default_state) {
            InitState
                .Enter(smi => Trace.Set([], smi))
                .Transition(StateA, _ => true);

            StateA
                .Enter("Enter", smi => Trace.Get(smi).Add("+A"))
                .Transition(StateB, _ => true)
                .Exit(smi => Trace.Get(smi).Add("-A"));

            StateB
                .DefaultState(StateB.StateA)
                .Enter(smi => Trace.Get(smi).Add("+B"))
                .Exit(smi => Trace.Get(smi).Add("-B"));

            StateB.StateA
                .Enter(smi => Trace.Get(smi).Add("+B.A"))
                .Exit(smi => Trace.Get(smi).Add("-B.A"));

            default_state = InitState;
        }

        [UsedImplicitly]
        public new class Instance(TestTarget master) : GameInstance(master);

    }

    public class SecondTestStateMachine : GameStateMachine<SecondTestStateMachine, SecondTestStateMachine.Instance,
        TestTarget, object> {

        public class SubStates : State {
            [UsedImplicitly]
            public State StateA = null!;
        }

        [UsedImplicitly]
        public State InitState = null!;

        [UsedImplicitly]
        public State StateA = null!;

        [UsedImplicitly]
        public ObjectParameter<List<string>> Trace = null!;

        // ReSharper disable once InconsistentNaming
        public override void InitializeStates(out BaseState default_state) {
            InitState
                .Enter(smi => Trace.Set([], smi))
                .Transition(StateA, _ => true);

            StateA
                .Enter("Enter", smi => Trace.Get(smi).Add("+A"))
                .Exit(smi => Trace.Get(smi).Add("-A"));

            default_state = InitState;
        }

        [UsedImplicitly]
        public new class Instance(TestTarget master) : GameInstance(master);

    }

}
