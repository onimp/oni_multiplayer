using System;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.ModRuntime.StaticCompatibility;
using MultiplayerMod.Multiplayer;
using MultiplayerMod.Multiplayer.Chores.Commands;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Multiplayer.Objects;
using NUnit.Framework;

namespace MultiplayerMod.Test.Multiplayer.Chores;

[TestFixture]
public class CreateChoreTest : ChoreTest {

    // TODO: Extract "SpecialWrap" and isolate serialization tests
    [Test, TestCaseSource(nameof(ChoresInstantiationTestCases))]
    public void CommandMustBeSerializableAndExecutable(ChoreFactory factory) {
        var id = new MultiplayerId(Guid.NewGuid());
        var sourceCommand = new CreateChore(id, factory.Type, factory.GetArguments());
        var command = SerializeDeserializeCommand(sourceCommand);

        Assert.That(command, Is.TypeOf<CreateChore>());
        Assert.That(((CreateChore) command).Arguments, Is.EqualTo(sourceCommand.Arguments));

        var context = new MultiplayerCommandContext(null!, new MultiplayerCommandRuntimeAccessor(Runtime.Instance));
        command.Execute(context);

        var chore = (Chore?) Dependencies.Get<MultiplayerGame>().Objects[id];

        Assert.That(chore, Is.Not.Null);
        Assert.That(chore, Is.TypeOf(factory.Type));
    }

}
