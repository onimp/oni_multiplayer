using System;
using System.IO;
using JetBrains.Annotations;
using MultiplayerMod.Multiplayer.StateMachines.RuntimeTools;
using NUnit.Framework;

namespace MultiplayerMod.Test.Multiplayer.StateMachines.RuntimeTools;

[TestFixture]
public class StateMachineContextRuntimeToolsTests : StateMachinesTest {

    [Test]
    public void MustSetContextValue() {
        var instance = CreateStateMachineInstance<TestStateMachine.Instance>();
        var parameter = instance.sm.ParameterBool;
        var runtime = StateMachineContextRuntimeTools.Get(instance.parameterContexts[parameter.idx]);
        var result = runtime.Set(instance, false);
        Assert.That(result, Is.True);
        Assert.That(parameter.Get(instance), Is.False);
    }

    [Test]
    public void MustNotSetInvalidTypedContextValue() {
        var instance = CreateStateMachineInstance<TestStateMachine.Instance>();
        var parameter = instance.sm.ParameterInt;
        var runtime = StateMachineContextRuntimeTools.Get(instance.parameterContexts[parameter.idx]);
        var result = runtime.Set(instance, false);
        Assert.That(result, Is.False);
        Assert.That(parameter.Get(instance), Is.EqualTo(0));
    }

    [Test]
    public void MustNotSetUnsupportedParameterContextValue() {
        var instance = CreateStateMachineInstance<TestStateMachine.Instance>();
        var parameter = instance.sm.ParameterCustom;
        var runtime = StateMachineContextRuntimeTools.Get(instance.parameterContexts[parameter.idx]);
        var result = runtime.Set(instance, false);
        Assert.That(result, Is.False);
    }

    [UsedImplicitly]
    public class TestStateMachine : GameStateMachine<TestStateMachine, TestStateMachine.Instance, TestTarget> {

        public readonly BoolParameter ParameterBool = null!;
        public readonly IntParameter ParameterInt = null!;
        public readonly CustomParameter ParameterCustom = null!;

        [UsedImplicitly]
        public new class Instance(TestTarget master) : GameInstance(master);

        public override void InitializeStates(out BaseState default_state) {
            default_state = root;
        }

        [UsedImplicitly]
        public class CustomParameter : Parameter {
            public override Parameter.Context CreateContext() => new Context(this);
            new class Context(Parameter parameter) : Parameter.Context(parameter) {
                public override void Serialize(BinaryWriter writer) => throw new NotImplementedException();
                public override void Deserialize(IReader reader, StateMachine.Instance smi) => throw new NotImplementedException();
                public override void ShowEditor(StateMachine.Instance base_smi) => throw new NotImplementedException();
                public override void ShowDevTool(StateMachine.Instance base_smi) => throw new NotImplementedException();
            }
        }

    }

}
