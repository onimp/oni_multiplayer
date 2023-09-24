using System;

namespace MultiplayerMod.Multiplayer.Commands.Registry;

public class MultiplayerCommandConfiguration {

    public Type Type { get;}
    public MultiplayerCommandType CommandType { get;}

    public MultiplayerCommandConfiguration(Type type, MultiplayerCommandType commandType) {
        Type = type;
        CommandType = commandType;
    }

}
