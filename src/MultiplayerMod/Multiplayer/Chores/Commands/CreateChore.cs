using System;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Multiplayer.Chores.Serialization;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Objects.Extensions;

namespace MultiplayerMod.Multiplayer.Chores.Commands;

[Serializable]
public class CreateChore : MultiplayerCommand {

    private static Core.Logging.Logger log = LoggerFactory.GetLogger<CreateChore>();

    public new readonly MultiplayerId Id;
    public readonly Type ChoreType;
    public readonly object?[] Arguments;

    public CreateChore(MultiplayerId id, Type choreType, object?[] arguments) {
        Id = id;
        ChoreType = choreType;
        Arguments = ArgumentUtils.WrapObjects(ChoreArgumentsWrapper.Wrap(ChoreType, arguments));
    }

    public override void Execute(MultiplayerCommandContext context) {
        var args = ChoreArgumentsWrapper.Unwrap(ChoreType, ArgumentUtils.UnWrapObjects(Arguments));
        log.Debug($"Create chore {ChoreType} [id={Id}]");
        var chore = (Chore) ChoreType.GetConstructors()[0].Invoke(args);
        chore.Register(Id);
    }

}
