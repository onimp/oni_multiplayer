using System;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.Commands.Registry;

public class MultiplayerCommandConfiguration {

    public Type Type { get;}
    public MultiplayerCommandType CommandType { get;}
    public bool ExecuteOnServer { get;}
    public NetworkLane Lane { get; }

    public MultiplayerCommandConfiguration(
        Type type,
        MultiplayerCommandType commandType,
        bool executeOnServer,
        NetworkLane lane
    ) {
        Type = type;
        CommandType = commandType;
        ExecuteOnServer = executeOnServer;
        Lane = lane;
    }

}
