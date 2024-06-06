using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Core.Extensions;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.Multiplayer.StateMachines;
using MultiplayerMod.Multiplayer.StateMachines.Configuration;
using MultiplayerMod.Test.GameRuntime;
using NUnit.Framework;
using UnityEngine;

namespace MultiplayerMod.Test.Multiplayer.StateMachines;

[TestFixture]
public class StateMachineDslTests : PlayableGameTest {

    private static readonly Harmony harmony = new("StateMachinesDSL");

    [TearDown]
    public void TearDownDsl() {
        harmony.UnpatchAll("StateMachinesDSL");
        StateMachineManager.Instance.stateMachines.Clear();
    }

    private static void RunStateMachinesPatcher(params StateMachineConfigurer[] configurers) {
        var dispatcher = new EventDispatcher();
        var patcher = new StateMachinesPatcher(dispatcher, harmony, DependencyContainer);
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
            new StateMachineConfigurerDsl<TestStateMachine, TestStateMachine.Instance, TestTarget, object>(
                dsl => { dsl.PreConfigure(dsl => { dsl.Suppress(() => dsl.StateMachine.StateA.Enter("", null)); }); }
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
            new StateMachineConfigurerDsl<TestStateMachine, TestStateMachine.Instance, TestTarget, object>(
                root => {
                    root.PreConfigure(pre => {
                        Assert.IsNull(pre.StateMachine.StateA.enterActions);
                        Assert.IsNull(pre.StateMachine.StateA.exitActions);
                    });
                    root.PostConfigure(post => {
                        Assert.AreEqual(expected: 2, actual: post.StateMachine.StateA.enterActions?.Count);
                        Assert.AreEqual(expected: 1, actual: post.StateMachine.StateA.exitActions?.Count);
                    });
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
            new StateMachineConfigurerDsl<TestStateMachine, TestStateMachine.Instance, TestTarget, object>(
                dsl => {
                    dsl.PreConfigure(pre => {
                        Assert.IsNull(pre.StateMachine.StateA.enterActions);
                        Assert.IsNull(pre.StateMachine.StateA.exitActions);
                    });
                    dsl.PostConfigure(post => {
                        Assert.AreEqual(expected: 2, actual: post.StateMachine.StateA.enterActions?.Count);
                        Assert.AreEqual(expected: 1, actual: post.StateMachine.StateA.exitActions?.Count);
                    });
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
            new StateMachineConfigurerDsl<TestStateMachine, TestStateMachine.Instance, TestTarget, object>(
                dsl => {
                    dsl.PreConfigure(pre => { pre.StateMachine.StateA.Enter(smi => pre.StateMachine.Trace.Get(smi).Add("+X")); });
                    dsl.PostConfigure(post => { post.StateMachine.StateA.Exit(smi => post.StateMachine.Trace.Get(smi).Add("-X")); });
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
            new StateMachineConfigurerDsl<SecondTestStateMachine, SecondTestStateMachine.Instance, TestTarget, object>(
                dsl => {
                    dsl.PreConfigure(pre => { pre.StateMachine.StateA.Enter("Test", smi => pre.StateMachine.Trace.Get(smi).Add("+X")); });
                    dsl.PostConfigure(post => { post.StateMachine.StateA.Exit("Test", smi => post.StateMachine.Trace.Get(smi).Add("-X")); });
                }
            ),
            new StateMachineConfigurerDsl<TestStateMachine, TestStateMachine.Instance, TestTarget, object>(
                dsl => {
                    dsl.Inline(
                        new StateMachineConfigurerDsl<SecondTestStateMachine, SecondTestStateMachine.Instance, TestTarget, object>(
                            dsl => { dsl.PreConfigure(dsl => { dsl.Suppress(() => dsl.StateMachine.StateA.Enter(null, null)); }); }
                        )
                    );
                    dsl.PreConfigure(pre => { pre.StateMachine.StateA.Enter(smi => pre.StateMachine.Trace.Get(smi).Add("+X")); });
                    dsl.PostConfigure(post => { post.StateMachine.StateA.Exit(smi => post.StateMachine.Trace.Get(smi).Add("-X")); });
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
            new StateMachineConfigurerDsl<TestStateMachine, TestStateMachine.Instance, TestTarget, object>(
                root => {
                    root.PreConfigure(
                        pre => {
                            root.Inline(
                                new StateMachineConfigurerDsl<SecondTestStateMachine, SecondTestStateMachine.Instance, TestTarget, object>(
                                    dsl => {
                                        dsl.PreConfigure(dsl => { dsl.Suppress(() => dsl.StateMachine.StateA.Enter(null, null)); });
                                    }
                                )
                            );
                            pre.StateMachine.StateA.Enter(smi => pre.StateMachine.Trace.Get(smi).Add("+X"));
                        }
                    );
                    root.PostConfigure(post => { post.StateMachine.StateA.Exit(smi => post.StateMachine.Trace.Get(smi).Add("-X")); });
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
            new StateMachineConfigurerDsl<TestStateMachine, TestStateMachine.Instance, TestTarget, object>(
                root => {
                    root.PostConfigure(_ => {
                        root.PreConfigure(pre => pre.Suppress(() => pre.StateMachine.StateA.Enter("", null)));
                    });
                }
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

    [Test]
    public void MustCreateAndAttachNewState() {
        RunStateMachinesPatcher(
            new StateMachineConfigurerDsl<TestStateMachine, TestStateMachine.Instance, TestTarget, object>(root => {
                root.PreConfigure(pre => {
                    pre.Suppress(() => pre.StateMachine.InitState.Transition(null, null, 0));
                });
                root.PostConfigure(post => {
                    var newState = post.AddState(post.StateMachine.root, "new_state");

                    newState
                        .Enter("Enter", smi => post.StateMachine.Trace.Get(smi).Add("+N"))
                        .Exit(smi => post.StateMachine.Trace.Get(smi).Add("-N"));

                    post.StateMachine.InitState.Transition(newState, _ => true);
                });
            })
        );
        var smi = CreateStateMachineInstance<TestStateMachine.Instance>();

        var state = smi.sm.states.FirstOrDefault(it => it.name == "root.new_state");
        Assert.IsNotNull(state);

        smi.StartSM();
        smi.StopSM("Done");
        Assert.AreEqual(
            expected: new[] { "+N", "-N" },
            actual: smi.sm.Trace.Get(smi)
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
