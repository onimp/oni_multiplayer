using System;

namespace MultiplayerMod.Multiplayer.Commands.Registry;

public class MultiplayerCommandConfiguration {

    public Type Type { get;}
    public MultiplayerCommandType CommandType { get;}
    public bool ExecuteOnServer { get;}

    public MultiplayerCommandConfiguration(Type type, MultiplayerCommandType commandType, bool executeOnServer) {
        Type = type;
        CommandType = commandType;
        ExecuteOnServer = executeOnServer;
    }

}
