using MultiplayerMod.Multiplayer.StateMachines.RuntimeTools;
using NUnit.Framework;

namespace MultiplayerMod.Test.Multiplayer.StateMachines.RuntimeTools;

[TestFixture]
public class StateMachineRuntimeToolsTests : StateMachinesTest {

    [Test]
    public void ControllerIsResolved() {
        var instance = CreateStateMachineInstance<StateMachineContextRuntimeToolsTests.TestStateMachine.Instance>();
        var runtime = StateMachineRuntimeTools.Get(instance);
        var controller = runtime.GetController();
        Assert.That(controller, Is.Not.Null);
    }

}
